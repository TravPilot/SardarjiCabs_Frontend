using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Controllers
{
    public class PayNowController : Controller
    {
        private readonly ICustomerService _customerService;
        public PayNowController(ICustomerService customerService)
        {
            _customerService = customerService;
            
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentCancel()
        {
            return View();
        }


        public async Task<IActionResult> WalletdetailsAsync()
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            WalletVM walletDetails = await _customerService.GetWalletDetails(customer.Id);
            return View();
        }
    }
}
