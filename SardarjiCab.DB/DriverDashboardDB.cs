using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SardarjiCab.DB.Interface;
using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB
{
    public class DriverDashboardDB : IDriverDashboardDB
    {
        private readonly string _connectionString;

        public DriverDashboardDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DriverDashboard> GetDriverSummaryAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_GetDashboardSummary", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new DriverDashboard
                {
                    Id = (Int64)reader["Id"],
                    FullName = reader["FullName"] as string,
                    ProfilePhotoUrl = reader["Photo"] as string,
                    IsOnline = (bool)reader["IsAvailable"],
                    //ApprovalStatus = reader["ApprovalStatus"] as string,
                    //RejectionReason = reader["RejectionReason"] as string,
                    //TotalTrips = (int)reader["TotalTrips"],
                    //AverageRating = (decimal)reader["AverageRating"],
                    //TotalCo2SavedKg = (decimal)reader["TotalCo2SavedKg"]
                };
            }
            return null;
        }

        public async Task<decimal> GetTodayEarningsAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_GetTodayEarnings", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? (decimal)result : 0m;
        }

        public async Task<List<RecentTrip>> GetRecentTripsAsync(int driverId, int top)
        {
            var trips = new List<RecentTrip>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_GetRecentTrips", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = top });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new RecentTrip
                {
                    PickupArea = reader["PickupArea"] as string,
                    DropArea = reader["DropArea"] as string,
                    Fare = (decimal)reader["Fare"],
                    Status = reader["Status"] as string,
                    CompletedAt = (DateTime)reader["CompletedAt"]
                });
            }
            return trips;
        }

        public async Task<bool> UpdateOnlineStatusAsync(int driverId, bool isOnline)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_SetOnlineStatus", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@IsOnline", SqlDbType.Bit) { Value = isOnline });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return (int)rowsAffected.Value > 0;
        }

        public async Task<int> GetUnreadNotificationCountAsync(int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_GetUnreadNotificationCount", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? (int)result : 0;
        }
    }
}
