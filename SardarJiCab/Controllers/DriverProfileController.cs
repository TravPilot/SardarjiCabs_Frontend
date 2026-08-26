using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverProfileController : Controller
    {
        private readonly IDriverProfileBL _driverProfileBL;
        private readonly IWebHostEnvironment _env;
        public DriverProfileController(IDriverProfileBL driverProfileBL, IWebHostEnvironment env)
        {
            _driverProfileBL = driverProfileBL;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverProfileBL.GetProfileAsync(driverId.Value);
            if (model == null) return RedirectToAction("Index", "DriverLogIn");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string email, string gender,
            string city, string pinCode, DateTime? dateOfBirth, string address)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _driverProfileBL.UpdateProfileAsync(
                driverId.Value, fullName, email, gender, city, pinCode, dateOfBirth, address);

            if (result.Success)
                HttpContext.Session.SetString("DriverName", fullName ?? "");

            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhoto(IFormFile photo)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            if (photo == null || photo.Length == 0)
                return Json(new { success = false, message = "No photo selected." });

            if (photo.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "Photo must be under 5MB." });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (Array.IndexOf(allowedTypes, photo.ContentType) < 0)
                return Json(new { success = false, message = "Only JPG, PNG or WEBP images are allowed." });

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "drivers");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{driverId}_{Guid.NewGuid():N}{Path.GetExtension(photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var photoUrl = $"/uploads/drivers/{fileName}";
            var result = await _driverProfileBL.UpdatePhotoAsync(driverId.Value, photoUrl);

            if (!result.Success)
                return Json(new { success = false, message = result.Message });

            return Json(new { success = true, photoUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            var result = await _driverProfileBL.ChangePasswordAsync(driverId.Value, currentPassword, newPassword);
            return Json(new { success = result.Success, message = result.Message });
        }
        public IActionResult Update()
        {
            return View();
        }
    }
}
