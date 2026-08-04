using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface ILogInService
    {
        public Task<CustomerVM> customerLogin(CustomerVM customer);
    }
}
