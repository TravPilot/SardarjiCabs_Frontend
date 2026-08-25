using Microsoft.AspNetCore.Connections;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<ApplyCouponResponseDto> ValidateCouponAsync(
            string code, string userId, decimal fareAmount, string? rideType, bool isFirstRideForUser)
        {
            var (coupon, error) = await ValidateInternalAsync(code, userId, fareAmount, rideType, isFirstRideForUser);
            if (coupon == null)
                return Fail(error!);

            var discount = CalculateDiscount(coupon, fareAmount);
            return new ApplyCouponResponseDto
            {
                IsValid = true,
                Message = "Coupon applied successfully.",
                DiscountAmount = discount,
                FinalFareAmount = fareAmount - discount,
                CouponCode = coupon.Code
            };
        }

        public async Task<ApplyCouponResponseDto> RedeemCouponAsync(
            string code, string userId, int bookingId, decimal fareAmount, string? rideType, bool isFirstRideForUser)
        {
            var (coupon, error) = await ValidateInternalAsync(code, userId, fareAmount, rideType, isFirstRideForUser);
            if (coupon == null)
                return Fail(error!);

            var discount = CalculateDiscount(coupon, fareAmount);

            await _couponRepository.RedeemAsync(coupon.Id, new CouponUsage
            {
                CouponId = coupon.Id,
                UserId = userId,
                BookingId = bookingId,
                DiscountAmount = discount,
                UsedAt = DateTime.UtcNow
            });

            return new ApplyCouponResponseDto
            {
                IsValid = true,
                Message = "Coupon redeemed successfully.",
                DiscountAmount = discount,
                FinalFareAmount = fareAmount - discount,
                CouponCode = coupon.Code
            };
        }

        private async Task<(Coupon? coupon, string? error)> ValidateInternalAsync(
            string code, string userId, decimal fareAmount, string? rideType, bool isFirstRideForUser)
        {
            if (string.IsNullOrWhiteSpace(code))
                return (null, "Coupon code is required.");

            var coupon = await _couponRepository.GetByCodeAsync(code.Trim());

            if (coupon == null) return (null, "Invalid coupon code.");
            if (!coupon.IsActive) return (null, "This coupon is no longer active.");

            var now = DateTime.UtcNow;
            if (now < coupon.ValidFrom) return (null, "This coupon is not yet valid.");
            if (now > coupon.ValidTo) return (null, "This coupon has expired.");

            if (coupon.MinFareAmount.HasValue && fareAmount < coupon.MinFareAmount.Value)
                return (null, $"Minimum fare of {coupon.MinFareAmount.Value:C} required for this coupon.");

            if (coupon.TotalUsageLimit.HasValue && coupon.CurrentUsageCount >= coupon.TotalUsageLimit.Value)
                return (null, "This coupon has reached its usage limit.");

            if (coupon.IsFirstRideOnly && !isFirstRideForUser)
                return (null, "This coupon is valid only on your first ride.");

            if (!string.IsNullOrWhiteSpace(coupon.ApplicableRideTypes) && !string.IsNullOrWhiteSpace(rideType))
            {
                var allowedTypes = coupon.ApplicableRideTypes
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (!allowedTypes.Any(t => t.Equals(rideType, StringComparison.OrdinalIgnoreCase)))
                    return (null, $"This coupon is not applicable for {rideType} rides.");
            }

            if (coupon.UsageLimitPerUser.HasValue)
            {
                var userUsageCount = await _couponRepository.GetUserUsageCountAsync(coupon.Id, userId);
                if (userUsageCount >= coupon.UsageLimitPerUser.Value)
                    return (null, "You have already used this coupon the maximum number of times.");
            }

            return (coupon, null);
        }

        private static decimal CalculateDiscount(Coupon coupon, decimal fareAmount)
        {
            decimal discount = coupon.DiscountType == DiscountType.Percentage
                ? Math.Round(fareAmount * (coupon.DiscountValue / 100m), 2)
                : coupon.DiscountValue;

            if (coupon.MaxDiscountAmount.HasValue)
                discount = Math.Min(discount, coupon.MaxDiscountAmount.Value);

            
            return Math.Min(discount, fareAmount);
        }

        private static ApplyCouponResponseDto Fail(string message) => new ApplyCouponResponseDto
        {
            IsValid = false,
            Message = message,
            DiscountAmount = 0,
            FinalFareAmount = 0
        };

        public async Task<List<CouponListItemDto>> GetAvailableCouponsAsync(decimal fareAmount, string? rideType)
        {
            var activeCoupons = await _couponRepository.GetActiveCouponsAsync();
            fareAmount = 400;
            var eligible = activeCoupons.Where(c =>
            {
                if (c.MinFareAmount.HasValue && fareAmount < c.MinFareAmount.Value)
                    return false;

                if (!string.IsNullOrWhiteSpace(c.ApplicableRideTypes) && !string.IsNullOrWhiteSpace(rideType))
                {
                    var allowedTypes = c.ApplicableRideTypes
                        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (!allowedTypes.Any(t => t.Equals(rideType, StringComparison.OrdinalIgnoreCase)))
                        return false;
                }

               
                return true;
            });

            return eligible.Select(c => new CouponListItemDto
            {
                Id = c.Id,
                Code = c.Code,
                Description = c.Description,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                ValidFrom = c.ValidFrom,
                ValidTo = c.ValidTo,
                TotalUsageLimit = c.TotalUsageLimit,
                CurrentUsageCount = c.CurrentUsageCount,
                IsActive = c.IsActive
            }).ToList();
        }

    }
}
