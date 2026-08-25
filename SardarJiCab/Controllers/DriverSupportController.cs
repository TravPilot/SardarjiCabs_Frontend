using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverSupportController : Controller
    {
        private readonly IDriverSupportBL _driverSupportBL;

        public DriverSupportController(IDriverSupportBL driverSupportBL)
        {
            _driverSupportBL = driverSupportBL;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverSupportBL.GetSupportPageAsync(driverId.Value);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTicket(string category, string subject, string message)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _driverSupportBL.SubmitTicketAsync(driverId.Value, category, subject, message);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
