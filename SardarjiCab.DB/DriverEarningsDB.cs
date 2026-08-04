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
    public class DriverEarningsDB : IDriverEarningsDB
    {
        private readonly string _connectionString;

        public DriverEarningsDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<DriverEarnings> GetEarningsSummaryAsync(int driverId, DateTime? fromDate, DateTime? toDate)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetEarningsSummary", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime2) { Value = (object)fromDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime2) { Value = (object)toDate ?? DBNull.Value });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new DriverEarnings
                {
                    TotalEarnings = (decimal)reader["TotalEarnings"],
                    TotalFare = (decimal)reader["TotalFare"],
                    TotalTrips = (int)reader["TotalTrips"],
                    CashEarnings = (decimal)reader["CashEarnings"],
                    OnlineEarnings = (decimal)reader["OnlineEarnings"]
                };
            }
            return new DriverEarnings();
        }

        public async Task<List<EarningsTrip>> GetEarningsTripsAsync(int driverId, DateTime? fromDate, DateTime? toDate)
        {
            var trips = new List<EarningsTrip>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetEarningsTrips", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime2) { Value = (object)fromDate ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime2) { Value = (object)toDate ?? DBNull.Value });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                trips.Add(new EarningsTrip
                {
                    BookingId = (long)reader["BookingId"],
                    BookingNo = reader["BookingNo"] as string,
                    PickupArea = reader["PickupAddress"] as string,
                    DropArea = reader["DropAddress"] as string,
                    TotalFare = (decimal)reader["TotalFare"],
                    NetPayable = (decimal)reader["NetPayable"],
                    PaymentMethod = reader["PaymentMethod"] as string,
                    CompletedOn = reader["CompletedOn"] != DBNull.Value ? (DateTime?)reader["CompletedOn"] : null
                });
            }
            return trips;
        }
    }
}
