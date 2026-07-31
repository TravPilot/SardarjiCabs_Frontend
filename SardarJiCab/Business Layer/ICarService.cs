using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface ICarService
    {
       public Task<CarModel> GetCarByIdAsync(int carId);
        
    }
}
