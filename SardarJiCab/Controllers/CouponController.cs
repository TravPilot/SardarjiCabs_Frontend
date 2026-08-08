using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Controllers
{
    [Route("Coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        [HttpPost("ApplyAjax")]
        public async Task<IActionResult> ApplyAjax([FromBody] ApplyCouponRequestDto dto)
        {
            try
            {
                var userId = HttpContext.Session.GetObject<CustomerVM>("customer");


                if (userId.Id < 0)
                {
                    return Json(new ApplyCouponResponseDto
                    {
                        IsValid = false,
                        Message = "Please log in."
                    });
                }
                string userIdd = Convert.ToString(userId.Id);
                bool isFirstRideForUser = false;

                var result = await _couponService.ValidateCouponAsync(
                    dto.Code,
                    userIdd,
                    dto.FareAmount,
                    dto.RideType,
                    isFirstRideForUser);

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }





        [HttpGet("Apply")]
        public IActionResult Apply(decimal fareAmount, string? rideType)
        {
            ViewBag.FareAmount = fareAmount;
            ViewBag.RideType = rideType;
            return View(new ApplyCouponResponseDto());
        }

        [HttpGet("AvailableCoupons")]
        public async Task<IActionResult> AvailableCoupons(decimal fareAmount, string? rideType)
        {
            var coupons = await _couponService.GetAvailableCouponsAsync(fareAmount, rideType);
            var json = JsonConvert.SerializeObject(coupons);
            return Json(coupons);
        }

      
    }
}
