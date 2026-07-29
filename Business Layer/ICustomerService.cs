using SardarJi_Cab_Booking.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface ICustomerService
    {
        public Task<CustomerProfile> GetCustomerProfile(long Id);
        public Task<List<CitiesVM>> GetCities(string stateId);
        public Task<List<StatesVM>> GetStates(string CountryId);
        public Task<CustomerProfile> UpdateProfile( CustomerProfile profile);
        public Task<QuotationEmailSettings> GetQuotationDetails(long Id);
        public Task<CustomerProfile> ForgotPassword(CustomerProfile profile);
        public Task<WalletVM> GetWalletDetails(long Id);
        public Task<ProfileSettings> GetProfileSettings(long clientId);
    }
}
