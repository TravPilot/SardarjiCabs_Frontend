using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverTripInProgressController : Controller
    {
        private readonly IDriverTripsBL _driverTripsBL;
        private readonly IConfiguration _config;

        public DriverTripInProgressController(IDriverTripsBL driverTripsBL, IConfiguration config)
        {
            _driverTripsBL = driverTripsBL;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long bookingId)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverTripsBL.GetActiveTripAsync(bookingId, driverId.Value);
            model.GoogleMapsApiKey = _config["GoogleMapsApiKey"];
            if (model == null) return RedirectToAction("Index", "DriverTrips");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTrip(long bookingId)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _driverTripsBL.CompleteTripAsync(bookingId, driverId.Value);
            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, redirectUrl = Url.Action("Index", "DriverTrips") });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLocation(long bookingId, double latitude, double longitude)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return Json(new { success = false });

            await _driverTripsBL.UpdateLiveLocationAsync(bookingId, driverId.Value, latitude, longitude);
            return Json(new { success = true });
        }
    }
}
