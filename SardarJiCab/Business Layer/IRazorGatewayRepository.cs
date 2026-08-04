using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IRazorGatewayRepository
    {
        public Task<PaymentGatewaySettings> GetPaymentGatewaySettings(long clientId);
        Task SaveTransactionDetails(string id, string amt, string customerSession);

        Task<RazorPayVM> ValidateOrderId(string id);
    }
}
