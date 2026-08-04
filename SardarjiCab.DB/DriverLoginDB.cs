using SardarjiCab.DB.Interface;

using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SardarJiCab.Model.SardarJiEV.Models;

namespace SardarjiCab.DB
{
    public class DriverLoginDB : IDriverLoginDB
    {
        private readonly string _connectionString;

        public DriverLoginDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Driver> GetDriverByMobileAsync(string mobile)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetDriver_ByMobile", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@MobileNumber", SqlDbType.NVarChar, 10) { Value = mobile });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapDriver(reader);

            return null;
        }

        public async Task UpdateLastLoginAsync(Int64 driverId, string ipAddress)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetDriver_UpdateLastLogin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@LastLoginIp", SqlDbType.NVarChar, 45)
            {
                Value = (object)ipAddress ?? DBNull.Value
            });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RecordFailedLoginAsync(Int64 driverId, int failedAttempts, DateTime? lockedUntil)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_RecordFailedLogin", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@FailedAttempts", SqlDbType.Int) { Value = failedAttempts });
            cmd.Parameters.Add(new SqlParameter("@LockedUntil", SqlDbType.DateTime2)
            {
                Value = (object)lockedUntil ?? DBNull.Value
            });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private static Driver MapDriver(SqlDataReader reader)
        {
            try
            {
                return new Driver
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    MobileNumber = reader["MobileNumber"] as string,
                    PasswordHash = reader["PasswordHash"] as string,
                    Email = reader["Email"] as string,
                    FullName = reader["FullName"] as string,
                    IsActive = (bool)reader["IsActive"],
                    IsVerified = (bool)reader["IsVerified"],
                    ApprovalStatus = reader["ApprovalStatus"] as string,
                    FailedLoginAttempts = reader["FailedLoginAttempts"] != DBNull.Value ? (int)reader["FailedLoginAttempts"] : 0,
                    LockedUntil = reader["LockedUntil"] != DBNull.Value ? (DateTime?)reader["LockedUntil"] : null,
                    LastLoginAt = reader["LastLoginAt"] != DBNull.Value ? (DateTime?)reader["LastLoginAt"] : null
                };
            }
            catch (Exception ex)
            {

                return new Driver { Id = -1, Success = false, Message = ex.Message };
            }
        }
    }
}