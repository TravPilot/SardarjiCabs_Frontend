using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class PrivacyController : Controller
    {

        private readonly IConfiguration _config;
        private readonly IAddPageRepository _adpg;

        public PrivacyController(IAddPageRepository adpg, IConfiguration config)
        {

            _adpg = adpg;
            _config = config;
        }
        public async Task<IActionResult> Services(string seourl)
        {
            Int64 ClientId = Convert.ToInt64(_config["ClientId"]);
            AddPageVM ap = new AddPageVM();
            ap.SeoUrl = seourl;
            ap.ClientId = ClientId;
            //AddPageVM jskskks = await _adpg.GetAddPageDetailAsync(ap);
            return View(await _adpg.GetAddPageDetailAsync(ap));
           
        }


    }
}
