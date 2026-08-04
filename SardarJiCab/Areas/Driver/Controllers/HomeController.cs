using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Areas.Driver.Controllers
{
    [Area("Driver")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel();
            homeIndexViewModel.GoogleMapsApiKey = "AIzaSyBSt5wVEMcgLP5DoXVna8_DmybbJj0hHdI";
            return View(homeIndexViewModel);
        }
    }
}
