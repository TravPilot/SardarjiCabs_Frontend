using Microsoft.AspNetCore.Mvc;

namespace SardarJi_Cab_Booking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
