using Dapper;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Models;
using System.Data;
using System.Linq.Dynamic.Core;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class BookingService : IBookingService
    {
        private readonly string _connectionString;

        public BookingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public async Task<TravelSummaryViewModel> SaveBookingDetails(TravelSummaryViewModel details)
        {
            try
            {
                string CabRoute = details.Pickup + " ⟶ " + details.Drop;
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@BookingId", details.BookingId);
                p.Add("@BookingNo", details.RideCode);
                p.Add("@ClientId", details.ClientId);
                p.Add("@UserId", details.UserId);
                p.Add("@CabRoute", CabRoute);
                p.Add("@PickupAddress", details.Pickup);
                p.Add("@DropAddress", details.Drop);
                p.Add("@JourneyDate", details.RideDate);
                p.Add("@VehicleNumber", details.RegistrationNo);
                p.Add("@CarImage", details.CarImage);
                p.Add("@JourneyTime", details.RideTime);
                p.Add("@TotalDistanceKm", details.DistanceKm);
                p.Add("@PassengerName", details.PassengerName);
                p.Add("@ContactNumber", details.PassengerContact);
                p.Add("@CarId", details.CarId);
               
                p.Add("@VehicleName", details.CarName);
                p.Add("@VehicleModelYear", details.CarModelName);
                p.Add("@VehicleColor", details.Color);
                p.Add("@VehicleFuelType", details.FuelType);
                p.Add("@PaymentMethod", details.PaymentMethod);
                
                p.Add("@TotalFare", details.Cost);
                p.Add("@NetPayable", details.Cost);
                p.Add("@BarcodeInfo", details.BarcodeCaption);
                p.Add("@Status", "Confirmed");
                p.Add("@NewBookingId", dbType: DbType.Int64, direction: ParameterDirection.Output);

                await conn.ExecuteAsync(
                    "dbo.SardarjiSaveCabBooking",
                    p,
                    commandType: CommandType.StoredProcedure);

                details.BookingId = p.Get<long>("@NewBookingId");

                return details;
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<TravelSummaryViewModel> GetBookingList(long Id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);

                var p = new DynamicParameters();
                p.Add("@UserId", Id);


                var bookings = await conn.QueryAsync<BookingListItem>(
                    "dbo.SardarjiGetBookingListByUserId",
                    p,
                    commandType: CommandType.StoredProcedure);

                var result = new TravelSummaryViewModel
                {
                    UserId = Id,
                    Bookings = bookings.AsList()
                };

                return result;
            }
            catch (SqlException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<TravelSummaryViewModel> GetBookingList(long Id)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(_connectionString);

        //        var p = new DynamicParameters();
        //        p.Add("@UserId", Id);


        //        var customerProfile = await conn.QueryFirstOrDefaultAsync<TravelSummaryViewModel>(
        //            "dbo.SardarjiGetBookingListByUserId",
        //            p,
        //            commandType: CommandType.StoredProcedure);

        //        return customerProfile;
        //    }
        //    catch (SqlException ex)
        //    {


        //        throw;
        //    }
        //    catch (Exception ex)
        //    {


        //        throw;
        //    }
        //}


        #region Get Booking List   



        public async Task<Models.PagedResult<BookingListItem>> GetBookingsAsync(BookingFilter filter)
        {
            using var conn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@ClientId", filter.ClientId);
            p.Add("@UserId", filter.UserId);
            p.Add("@Status", string.IsNullOrWhiteSpace(filter.Status) ? "All" : filter.Status);
            p.Add("@FromDate", filter.FromDate);
            p.Add("@ToDate", filter.ToDate);
            p.Add("@SearchText", filter.SearchText);
            p.Add("@PageNumber", filter.PageNumber);
            p.Add("@PageSize", filter.PageSize);
            p.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var rows = await conn.QueryAsync<BookingListItem>(
                "dbo.SardarjiBooking_GetList",
                p,
                commandType: CommandType.StoredProcedure);

            return new Models.PagedResult<BookingListItem>
            {
                Items = rows.AsList(),
                TotalRecords = p.Get<int>("@TotalRecords"),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<BookingListItem> GetBookingByIdAsync(long newBookingId)
        {
            using var conn = new SqlConnection(_connectionString);

            return await conn.QueryFirstOrDefaultAsync<BookingListItem>(
                "dbo.SardarjiBooking_GetById",
                new { BookingId = newBookingId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStatusAsync(long newBookingId, string status)
        {
            using var conn = new SqlConnection(_connectionString);

            await conn.ExecuteAsync(
                "dbo.SardarjiBooking_UpdateStatus",
                new { NewBookingId = newBookingId, Status = status },
                commandType: CommandType.StoredProcedure);
        }

        #endregion  end  Get Booking List   







    }
}
