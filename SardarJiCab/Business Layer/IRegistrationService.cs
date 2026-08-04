using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IRegistrationService
    {
        public Task<CustomerVM> CustomerSignUp(SignUpVM customer);
    }
}
