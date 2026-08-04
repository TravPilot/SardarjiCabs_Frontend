using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverEarningsController : Controller
    {
        private readonly IDriverEarningsBL _driverEarningsBL;

        public DriverEarningsController(IDriverEarningsBL driverEarningsBL)
        {
            _driverEarningsBL = driverEarningsBL;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string period = "all")
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverEarningsBL.GetEarningsAsync(driverId.Value, period);
            return View(model);
        }
    }
}
