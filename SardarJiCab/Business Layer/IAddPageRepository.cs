using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IAddPageRepository
    {
        Task<AddPageVM?> GetAddPageDetailAsync(AddPageVM addPage);
    }
}
