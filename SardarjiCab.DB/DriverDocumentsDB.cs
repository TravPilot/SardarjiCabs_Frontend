using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SardarjiCab.DB.Interface;
using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB
{
    public class DriverDocumentsDB: IDriverDocumentsDB
    {
        private readonly string _connectionString;

        public DriverDocumentsDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Driver> GetDocumentsAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetDocuments", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new Driver
            {
                Id = (int)reader["DriverId"],
                ApprovalStatus = reader["ApprovalStatus"] as string,
                RejectionReason = reader["RejectionReason"] as string,
                LicenseNumber = reader["LicenseNumber"] as string,
                LicenseExpiryDate = reader["LicenseExpiryDate"] != DBNull.Value ? Convert.ToDateTime(reader["LicenseExpiryDate"]) : null,
                LicensePhotoUrl = reader["LicensePhotoUrl"] as string,
                AadhaarNumber = reader["AadhaarNumber"] as string,
                AadhaarPhotoUrl = reader["AadhaarPhotoUrl"] as string,
                RcNumber = reader["RcNumber"] as string,
                RcPhotoUrl = reader["RcPhotoUrl"] as string
            };
        }

        public async Task<int> UpdateLicenseAsync(int driverId, string licenseNumber, DateTime? expiryDate, string photoUrl)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdateLicense", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@LicenseNumber", SqlDbType.NVarChar, 30) { Value = (object)licenseNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@LicenseExpiryDate", SqlDbType.DateTime2) { Value = (object)expiryDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@LicensePhotoUrl", SqlDbType.NVarChar, -1) { Value = photoUrl });
            var rows = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rows);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rows.Value;
        }

        public async Task<int> UpdateAadhaarAsync(int driverId, string aadhaarNumber, string photoUrl)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_UpdateAadhaar", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@AadhaarNumber", SqlDbType.NVarChar, 20) { Value = (object)aadhaarNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AadhaarPhotoUrl", SqlDbType.NVarChar, -1) { Value = photoUrl });
            var rows = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rows);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rows.Value;
        }

        public async Task<int> UpdateRcAsync(int driverId, string rcNumber, string photoUrl)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdateRc", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@RcNumber", SqlDbType.NVarChar, 30) { Value = (object)rcNumber ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@RcPhotoUrl", SqlDbType.NVarChar, -1) { Value = photoUrl });
            var rows = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rows);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rows.Value;
        }
    }
}
