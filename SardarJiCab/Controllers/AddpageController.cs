using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Controllers
{
    public class AddpageController : Controller
    {


        private readonly IConfiguration _config;
        private readonly IAddPageRepository _adpg;

        public AddpageController(IAddPageRepository adpg ,IConfiguration config)
        {

            _adpg = adpg;
             _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Addpage(string seourl)
        {
            try
            {
                Int64 ClientId = Convert.ToInt64(_config["ClientId"]);



                AddPageVM ap = new AddPageVM();

                ap.SeoUrl = seourl;
                ap.ClientId = ClientId;

                return View(_adpg.GetAddPageDetailAsync(ap));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
