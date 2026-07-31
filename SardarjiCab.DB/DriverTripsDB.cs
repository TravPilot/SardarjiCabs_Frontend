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
    public class DriverTripsDB: IDriverTripsDB
    {
        private readonly string _connectionString;

        public DriverTripsDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Task<List<CabBooking>> GetUpcomingTripsAsync(int driverId) =>
            RunBookingQuery("USP_Driver_GetUpcomingTrips", driverId);

        public Task<List<CabBooking>> GetCompletedTripsAsync(int driverId) =>
            RunBookingQuery("USP_Driver_GetCompletedTrips", driverId);
        public Task<List<CabBooking>> GetOnGoingTripsAsync(int driverId) =>
            RunBookingQuery("Driver_GetOnGoingTrips", driverId);

        private async Task<List<CabBooking>> RunBookingQuery(string procName, int driverId)
        {
            var rows = new List<CabBooking>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rows.Add(new CabBooking
                {
                    BookingId = (long)reader["BookingId"],
                    BookingNo = reader["BookingNo"] as string,
                    JourneyDate = (DateTime)reader["JourneyDate"],
                    JourneyTime = reader["JourneyTime"] as string,
                    PassengerName = reader["PassengerName"] as string,
                    ContactNumber = reader["ContactNumber"] as string,
                    PickupAddress = reader["PickupAddress"] as string,
                    DropAddress = reader["DropAddress"] as string,
                    TotalDistanceKm = reader["TotalDistanceKm"] != DBNull.Value ? (decimal)reader["TotalDistanceKm"] : 0,
                    VehicleName = reader["VehicleName"] as string,
                    VehicleColor = reader["VehicleColor"] as string,
                    VehicleNumber = reader["VehicleNumber"] as string,
                    CarImage = reader["CarImage"] as string,
                    PaymentMethod = reader["PaymentMethod"] as string,
                    NetPayable = reader["NetPayable"] != DBNull.Value ? (decimal)reader["NetPayable"] : 0,
                    Status = reader["Status"] as string,
                    Otp = reader["OTP"] as string,
                    CompletedOn = reader["CompletedOn"] != DBNull.Value ? (DateTime?)reader["CompletedOn"] : null
                });
            }
            return rows;
        }

        public async Task<int> StartTripAsync(long bookingId, int driverId, string enteredOtp)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("USP_Driver_StartTrip", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.BigInt) { Value = bookingId });
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@EnteredOtp", SqlDbType.VarChar, 50) { Value = (object)enteredOtp ?? DBNull.Value });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rowsAffected.Value;
        }




        public async Task<CabBooking> GetActiveTripAsync(long bookingId, int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_GetActiveTripDetails", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.BigInt) { Value = bookingId });
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new CabBooking
            {
                BookingId = (long)reader["BookingId"],
                BookingNo = reader["BookingNo"] as string,
                PickupAddress = reader["PickupAddress"] as string,
                DropAddress = reader["DropAddress"] as string,
                PassengerName = reader["PassengerName"] as string,
                ContactNumber = reader["ContactNumber"] as string,
                NetPayable = reader["NetPayable"] != DBNull.Value ? (decimal)reader["NetPayable"] : 0
            };
        }

        public async Task<int> CompleteTripAsync(long bookingId, int driverId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_CompleteTrip", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.BigInt) { Value = bookingId });
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            var rowsAffected = new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(rowsAffected);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return (int)rowsAffected.Value;
        }

        public async Task UpdateLiveLocationAsync(long bookingId, int driverId, double latitude, double longitude)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("Driver_UpdateLiveLocation", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@BookingId", SqlDbType.BigInt) { Value = bookingId });
            cmd.Parameters.Add(new SqlParameter("@DriverId", SqlDbType.Int) { Value = driverId });
            cmd.Parameters.Add(new SqlParameter("@Latitude", SqlDbType.Decimal) { Value = latitude, Precision = 9, Scale = 6 });
            cmd.Parameters.Add(new SqlParameter("@Longitude", SqlDbType.Decimal) { Value = longitude, Precision = 9, Scale = 6 });

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
