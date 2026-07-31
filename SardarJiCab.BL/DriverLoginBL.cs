using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using SardarjiCab.DB.Interface;
using SardarJiCab.BL.Interface;
using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL
{
    public class DriverLoginBL : IDriverLoginBL
    {
        private readonly IDriverLoginDB _driverLoginDB;
        private readonly IMemoryCache _cache;
        //private readonly ISmsSender _smsSender;
        private readonly PasswordHasher<Driver> _passwordHasher = new();

        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan OtpValidity = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan OtpResendCooldown = TimeSpan.FromSeconds(30);

        public DriverLoginBL(IDriverLoginDB driverLoginDB, IMemoryCache cache/*, ISmsSender smsSender*/)
        {
            _driverLoginDB = driverLoginDB;
            _cache = cache;
            //_smsSender = smsSender;
        }

        public async Task<DriverLoginResult> LoginWithPasswordAsync(string mobile, string password, string ipAddress)
        {
            mobile = (mobile ?? "").Trim();

            if (string.IsNullOrWhiteSpace(mobile) || string.IsNullOrWhiteSpace(password))
                return Fail("Enter your mobile number and password.");

            var driver = await _driverLoginDB.GetDriverByMobileAsync(mobile);
            if (driver == null || !driver.IsActive)
                return Fail("We couldn't find an active driver account with that number.");

            if (driver.LockedUntil.HasValue && driver.LockedUntil.Value > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")))
            {
                var minutesLeft = Math.Ceiling((driver.LockedUntil.Value - TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))).TotalMinutes);
                return Fail($"Too many failed attempts. Try again in {minutesLeft} minute(s).");
            }

            if (string.IsNullOrEmpty(driver.PasswordHash))
                return Fail("This account has no password set. Please sign in using OTP instead.");

            try
            {
                var verifyResult = _passwordHasher.VerifyHashedPassword(driver, driver.PasswordHash, password);
                if (verifyResult == PasswordVerificationResult.Failed)
                {
                    var attempts = driver.FailedLoginAttempts + 1;
                    DateTime? lockUntil = attempts >= MaxFailedAttempts
                        ? DateTime.UtcNow.Add(LockoutDuration)
                        : (DateTime?)null;

                    await _driverLoginDB.RecordFailedLoginAsync(driver.Id, attempts, lockUntil);

                    return Fail(lockUntil.HasValue
                        ? $"Too many failed attempts. Your account is locked for {LockoutDuration.TotalMinutes:0} minutes."
                        : "Incorrect password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                string hash = _passwordHasher.HashPassword(driver, "123456");
                driver.Message = ex.Message;
                return new DriverLoginResult
                {
                    Success = false,
                    Driver = driver
                };
            }

            if (!string.Equals(driver.ApprovalStatus, "Approved", StringComparison.OrdinalIgnoreCase))
                return Fail("Your driver account is still pending approval.");

            await _driverLoginDB.UpdateLastLoginAsync(driver.Id, ipAddress);

            return new DriverLoginResult { Success = true, Driver = driver };
        }

        public async Task<OtpRequestResult> RequestOtpAsync(string mobile)
        {
            mobile = (mobile ?? "").Trim();

            if (mobile.Length != 10 || !mobile.All(char.IsDigit))
                return new OtpRequestResult { Success = false, Message = "Enter a valid 10-digit mobile number." };

            var cooldownKey = $"driver_otp_cooldown_{mobile}";
            if (_cache.TryGetValue(cooldownKey, out _))
                return new OtpRequestResult { Success = false, Message = "Please wait before requesting another code." };

            var driver = await _driverLoginDB.GetDriverByMobileAsync(mobile);
            if (driver == null || !driver.IsActive)
                return new OtpRequestResult { Success = false, Message = "No active driver account found for this number." };

            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            var otpKey = $"driver_otp_{mobile}";

            _cache.Set(otpKey, otp, OtpValidity);
            _cache.Set(cooldownKey, true, OtpResendCooldown);

            //await _smsSender.SendAsync(mobile, $"Your Sardar Ji EV driver login OTP is {otp}. Valid for 5 minutes.");

            return new OtpRequestResult { Success = true, Message = "OTP sent." };
        }

        public async Task<DriverLoginResult> VerifyOtpAsync(string mobile, string otp, string ipAddress)
        {
            mobile = (mobile ?? "").Trim();
            otp = (otp ?? "").Trim();
            var otpKey = $"driver_otp_{mobile}";

            if (!_cache.TryGetValue(otpKey, out string cachedOtp) || cachedOtp != otp)
                return Fail("Invalid or expired OTP.");

            var driver = await _driverLoginDB.GetDriverByMobileAsync(mobile);
            if (driver == null || !driver.IsActive)
                return Fail("Driver account not found.");

            if (!string.Equals(driver.ApprovalStatus, "Approved", StringComparison.OrdinalIgnoreCase))
                return Fail("Your driver account is still pending approval.");

            _cache.Remove(otpKey);
            await _driverLoginDB.UpdateLastLoginAsync(driver.Id, ipAddress);

            return new DriverLoginResult { Success = true, Driver = driver };
        }

        private static DriverLoginResult Fail(string message) => new DriverLoginResult { Success = false, Message = message };
    }
}
