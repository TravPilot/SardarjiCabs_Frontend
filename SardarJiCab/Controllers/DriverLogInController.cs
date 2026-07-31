//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Caching.Memory;
//using SardarJiCab.BL.Interface;
//using SardarJiCab.Model.SardarJiEV.Models;

//namespace SardarJi_Cab_Booking.Controllers
//{
//    public class DriverLogInController : Controller
//    {
//        private readonly IDriverLoginBL _driverLoginBL;

//        public DriverLogInController(IDriverLoginBL driverLoginBL)
//        {
//            _driverLoginBL = driverLoginBL;
//        }

//        // GET: /DriverLogIn
//        [HttpGet]
//        public IActionResult Index()
//        {
//            if (HttpContext.Session.GetInt32("DriverId") != null)
//                return RedirectToAction("Index", "DriverDashboard");

//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(string mobile, string password, bool remember = false)
//        {
//            var result = await _driverLoginBL.LoginWithPasswordAsync(
//                mobile, password, HttpContext.Connection.RemoteIpAddress?.ToString());

//            if (!result.Success)
//                return Json(new { success = false, message = result.Message });

//            SignInDriver(result.Driver, remember);
//            return Json(new { success = true, redirectUrl = Url.Action("Index", "DriverDashboard") });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SendOtp(string mobile)
//        {
//            var result = await _driverLoginBL.RequestOtpAsync(mobile);
//            return Json(new { success = result.Success, message = result.Message });
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> VerifyOtp(string mobile, string otp)
//        {
//            var result = await _driverLoginBL.VerifyOtpAsync(
//                mobile, otp, HttpContext.Connection.RemoteIpAddress?.ToString());

//            if (!result.Success)
//                return Json(new { success = false, message = result.Message });

//            SignInDriver(result.Driver, remember: true);
//            return Json(new { success = true, redirectUrl = Url.Action("Index", "DriverDashboard") });
//        }

//        [HttpGet]
//        public IActionResult Logout()
//        {
//            HttpContext.Session.Clear();
//            return RedirectToAction("Index");
//        }

//        private void SignInDriver(Driver driver, bool remember)
//        {
//            HttpContext.Session.SetInt32("DriverId", Convert.ToInt32(driver.Id));
//            HttpContext.Session.SetString("DriverName", driver.FullName ?? "");
//            HttpContext.Session.SetString("DriverMobile", driver.MobileNumber);
//        }
//    }
//}
