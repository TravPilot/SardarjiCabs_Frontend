using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface ICouponService
    {
        Task<ApplyCouponResponseDto> ValidateCouponAsync(string code, string userId, decimal fareAmount, string? rideType, bool isFirstRideForUser);
        Task<List<CouponListItemDto>> GetAvailableCouponsAsync(decimal fareAmount, string? rideType);

        Task<ApplyCouponResponseDto> RedeemCouponAsync(string code, string userId, int bookingId, decimal fareAmount, string? rideType, bool isFirstRideForUser);
    }
}
