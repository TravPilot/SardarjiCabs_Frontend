using Microsoft.AspNetCore.Identity;
using SardarjiCab.DB.Interface;
using SardarJiCab.BL.Interface;
using SardarJiCab.Model;
using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL
{
    public class DriverProfileBL: IDriverProfileBL
    {
        private readonly IDriverProfileDB _driverProfileDB;
        private readonly PasswordHasher<Driver> _passwordHasher = new();

        public DriverProfileBL(IDriverProfileDB driverProfileDB)
        {
            _driverProfileDB = driverProfileDB;
        }

        public async Task<DriverProfile> GetProfileAsync(int driverId)
        {
            var driver = await _driverProfileDB.GetByIdAsync(driverId);
            if (driver == null) return null;

            return driver;
        }

        public async Task<StatusUpdateResult> UpdateProfileAsync(int driverId, string fullName, string email, string gender,
            string city, string pinCode, DateTime? dateOfBirth, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return new StatusUpdateResult { Success = false, Message = "Full name is required." };

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
                return new StatusUpdateResult { Success = false, Message = "Enter a valid email address." };

            var rows = await _driverProfileDB.UpdateProfileAsync(
                driverId, fullName.Trim(), email?.Trim(), gender, city?.Trim(), pinCode?.Trim(), dateOfBirth, address?.Trim());

            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Profile updated." }
                : new StatusUpdateResult { Success = false, Message = "Could not update profile. Please try again." };
        }

        public async Task<StatusUpdateResult> UpdatePhotoAsync(int driverId, string photoUrl)
        {
            var rows = await _driverProfileDB.UpdatePhotoAsync(driverId, photoUrl);
            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Photo updated." }
                : new StatusUpdateResult { Success = false, Message = "Could not update photo." };
        }

        public async Task<StatusUpdateResult> ChangePasswordAsync(int driverId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                return new StatusUpdateResult { Success = false, Message = "New password must be at least 8 characters." };

            var existingHash = await _driverProfileDB.GetPasswordHashAsync(driverId);
            if (string.IsNullOrEmpty(existingHash))
                return new StatusUpdateResult { Success = false, Message = "No password is set on this account yet." };

            var dummyDriver = new Driver { Id = driverId };
            var verifyResult = _passwordHasher.VerifyHashedPassword(dummyDriver, existingHash, currentPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
                return new StatusUpdateResult { Success = false, Message = "Current password is incorrect." };

            var newHash = _passwordHasher.HashPassword(dummyDriver, newPassword);
            var rows = await _driverProfileDB.UpdatePasswordAsync(driverId, newHash);

            return rows > 0
                ? new StatusUpdateResult { Success = true, Message = "Password updated." }
                : new StatusUpdateResult { Success = false, Message = "Could not update password. Please try again." };
        }
    }
}
