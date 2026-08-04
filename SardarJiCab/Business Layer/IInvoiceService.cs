using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IInvoiceService
    {
        byte[] GenerateInvoicePdf(BookingListItem booking);
    }
}
