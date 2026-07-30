using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverDashboardController : Controller
    {
        private readonly IDriverDashboardBL _dashboardBL;

        public DriverDashboardController(IDriverDashboardBL dashboardBL)
        {
            _dashboardBL = dashboardBL;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return RedirectToAction("Index", "DriverLogIn");

            var model = await _dashboardBL.GetDashboardAsync(driverId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetOnlineStatus(bool isOnline)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _dashboardBL.SetOnlineStatusAsync(driverId.Value, isOnline);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
