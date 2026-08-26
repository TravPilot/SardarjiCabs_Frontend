using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SardarjiCab.DB.Interface;
using SardarJiCab.Model;
using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB
{
    public class DriverProfileDB : IDriverProfileDB
    {
        private readonly string _connectionString;

        public DriverProfileDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DriverProfile> GetByIdAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetProfile", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            try
            {
                DriverProfile driverProfile = new DriverProfile
                {
                    Id = (Int64)reader["Id"],
                    FullName = reader["FullName"] as string,
                    MobileNumber = reader["MobileNumber"] as string,
                    Email = reader["Email"] as string,
                    ProfilePhotoUrl = reader["ProfilePhotoUrl"] as string,
                    Gender = reader["Gender"] as string,
                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime?)reader["DateOfBirth"] : null,
                    Address = reader["Address"] as string,
                    City = reader["City"] as string,
                    PinCode = reader["PinCode"] as string,
                    LicenseNumber = reader["LicenseNumber"] as string,
                    LicenseExpiryDate = reader["LicenseExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["LicenseExpiryDate"]) : null,
                    LicensePhotoUrl = reader["LicensePhotoUrl"] as string,
                    IsVerified = (bool)reader["IsVerified"],
                    //VerifiedOn = reader["VerifiedOn"] != DBNull.Value ? Convert.ToDateTime(reader["VerifiedOn"]) : null,
                    VehicleNumber = reader["VehicleNumber"] as string,
                    VehicleModel = reader["VehicleModel"] as string,
                    VehicleType = reader["VehicleType"] as string,
                    IsElectricVehicle = (bool)reader["IsElectricVehicle"],
                    RcNumber = reader["RcNumber"] as string,
                    RcPhotoUrl = reader["RcPhotoUrl"] as string,
                    ApprovalStatus = reader["ApprovalStatus"] as string,
                    RejectionReason = reader["RejectionReason"] as string,
                    AverageRating = (decimal)reader["AverageRating"],
                    TotalTrips = (int)reader["TotalTrips"],
                    TotalEarnings = (decimal)reader["TotalEarnings"],
                    TotalCo2SavedKg = (decimal)reader["TotalCo2SavedKg"]
                };
                return driverProfile;
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public async Task<int> UpdateProfileAsync(int driverId, string fullName, string email, string gender,
            string city, string pinCode, DateTime? dateOfBirth, string address)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdateProfile", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 120) { Value = fullName });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = (object)email ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.NVarChar, 20) { Value = (object)gender ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar, 100) { Value = (object)city ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PinCode", SqlDbType.NVarChar, 10) { Value = (object)pinCode ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.DateTime2) { Value = (object)dateOfBirth ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.NVarChar, 300) { Value = (object)address ?? DBNull.Value });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rowsAffected.Value;
        }

        public async Task<int> UpdatePhotoAsync(int driverId, string photoUrl)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdatePhoto", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@ProfilePhotoUrl", SqlDbType.NVarChar, -1) { Value = photoUrl });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rowsAffected.Value;
        }

        public async Task<string> GetPasswordHashAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetPasswordHash", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result as string;
        }

        public async Task<int> UpdatePasswordAsync(int driverId, string newPasswordHash)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdatePassword", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = newPasswordHash });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rowsAffected.Value;
        }
    }
}
