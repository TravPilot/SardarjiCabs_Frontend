using Microsoft.AspNetCore.Mvc;
using SardarJiCab.BL.Interface;

namespace SardarJi_Cab_Booking.Controllers
{
    public class DriverDocumentsController : Controller
    {
        private readonly IDriverDocumentsBL _driverDocumentsBL;
        private readonly IWebHostEnvironment _env;
        private static readonly string[] AllowedTypes = { "image/jpeg", "image/png", "image/webp" };

        public DriverDocumentsController(IDriverDocumentsBL driverDocumentsBL, IWebHostEnvironment env)
        {
            _driverDocumentsBL = driverDocumentsBL;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null) return RedirectToAction("Index", "DriverLogIn");

            var model = await _driverDocumentsBL.GetDocumentsAsync(driverId.Value);
            if (model == null) return RedirectToAction("Index", "DriverLogIn");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(string docType, string documentNumber, string expiryDate, IFormFile file)
        {
            var driverId = HttpContext.Session.GetInt32("DriverId");
            if (driverId == null)
                return Json(new { success = false, message = "Session expired. Please log in again." });

            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Please select a file to upload." });

            if (file.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "File must be under 5MB." });

            if (Array.IndexOf(AllowedTypes, file.ContentType) < 0)
                return Json(new { success = false, message = "Only JPG, PNG or WEBP images are allowed." });

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{driverId}_{docType}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photoUrl = $"/uploads/documents/{fileName}";
            DateTime? parsedExpiry = DateTime.TryParse(expiryDate, out var d) ? d : (DateTime?)null;

            var result = docType?.ToLowerInvariant() switch
            {
                "license" => await _driverDocumentsBL.UpdateLicenseAsync(driverId.Value, documentNumber, parsedExpiry, photoUrl),
                "aadhaar" => await _driverDocumentsBL.UpdateAadhaarAsync(driverId.Value, documentNumber, photoUrl),
                "rc" => await _driverDocumentsBL.UpdateRcAsync(driverId.Value, documentNumber, photoUrl),
                _ => new SardarJiCab.Model.StatusUpdateResult { Success = false, Message = "Unknown document type." }
            };
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
