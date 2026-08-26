using SardarJi_Cab_Booking.Models;
using System.Linq.Dynamic.Core;
using static SardarJi_Cab_Booking.Controllers.CustomerController;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IBookingService
    {
        public Task<TravelSummaryViewModel> SaveBookingDetails(TravelSummaryViewModel details);
        //public Task<TravelSummaryViewModel> GetBookingList(long Id);

        Task<Models.PagedResult<BookingListItem>> GetBookingsAsync(BookingFilter filter);
        Task<BookingListItem> GetBookingByIdAsync(long newBookingId);
        Task UpdateStatusAsync(long newBookingId, string status);
        Task<TravelSummaryViewModel> GetBookingList(long Id);
        Task<List<LiveLocation>> GetLiveLocation(int bookingId);
         Task<bool> SaveRatingDetails(RideFeedbackDto dto);

        Task<SupportTicketViewModel> SaveTicketDetails(SupportTicketViewModel model);

    }
}
