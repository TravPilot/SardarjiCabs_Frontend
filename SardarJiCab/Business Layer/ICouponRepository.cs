using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface ICouponRepository
    {
        Task RedeemAsync(int couponId, CouponUsage usage);
        Task<Coupon?> GetByCodeAsync(string code);
        Task<int> GetUserUsageCountAsync(int couponId, string userId);
        Task<List<Coupon>> GetActiveCouponsAsync();
    }
}
