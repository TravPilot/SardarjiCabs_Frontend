using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverTripsController : Controller
    {
        private readonly IDriverTripsBL _driverTripsBL;

        public DriverTripsController(IDriverTripsBL driverTripsBL)
        {
            _driverTripsBL = driverTripsBL;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverTripsBL.GetTripsAsync(driverId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartTrip(long bookingId, string otp)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _driverTripsBL.StartTripAsync(bookingId, driverId.Value, otp);

            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action("Index", "DriverTripInProgress", new { bookingId })
            });
        }
    }
}
