using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class CustomerController : Controller
    {


        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IBookingService _booking;
        private readonly IInvoiceService _invoiceService;


        public CustomerController(ICustomerService customerService, IConfiguration config, IBookingService booking, IInvoiceService invoiceService)
        {

            _customerService = customerService;
            _config = config;
            _booking = booking;
            _invoiceService = invoiceService;
        }




        public async Task<IActionResult> Index()
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");

            if (customer == null)
            {
                return RedirectToAction("Index", "Customer");
            }

            CustomerProfile customerrr = await _customerService.GetCustomerProfile(customer.Id);

            TempData["Mobile"] = customerrr.Mobile;
            TempData["Email"] = customerrr.Email;

            HttpContext.Session.SetObject("customerProfile", customerrr);

            
            return View(customerrr);
        }


      
        public async Task<JsonResult> GetCities(string StateId)
        {
            ViewBag.States = null;
            
            List<CitiesVM> Cities = new List<CitiesVM>();
           
            Cities = await _customerService.GetCities(StateId);


            return Json(new
            {
                Cities
            });


        }
        public async Task<JsonResult> GetStates(string CountryId)
        {
            ViewBag.States = null;
            
            List<StatesVM> State = new List<StatesVM>();
            State = await _customerService.GetStates(CountryId);

            ViewBag.States = State;

            return Json(new
            {
                State
            });



        }

        public async Task<JsonResult> UpdateProfile(CustomerProfile profile)
        {
            profile.ClientId = Convert.ToInt64(Convert.ToInt64(_config["ClientId"]));
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            string email = profile.Email;
            string mobile = profile.Mobile;
            string firstName = profile.FirstName;
            string lastName = profile.LastName;

            var result = await _customerService.UpdateProfile(profile);

            return Json(new
            {
                success = result != null,
                data = result,
                message = result != null ? "Profile updated successfully." : "Profile update failed."
            });
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<JsonResult> VerifyAccount(CustomerProfile profile)
        {
            profile.WebsiteUrl = UriHelper.BuildAbsolute(HttpContext.Request.Scheme, HttpContext.Request.Host);

            profile.ClientId = (Convert.ToInt64(_config["ClientId"]));
            profile = await _customerService.ForgotPassword(profile);

            long ClientId =(Convert.ToInt64(_config["ClientId"]));

            QuotationEmailSettings quotationemailsettings = new QuotationEmailSettings();
           
            quotationemailsettings = await _customerService.GetQuotationDetails(ClientId);


            if (!string.IsNullOrEmpty(profile.Body))
            {
                profile.Body = profile.Body.Replace(
                    "https://bookyourtripz.com/Content/images/23891556799905703.png",
                    "https://backendportal.traviyo.com/Images/ProfileSettingss/Gaurav_73577/638956970954989030_temp.png"
                );
            }

            QueryVM query = new QueryVM
            {
                Sendto = profile.Email,
                From = quotationemailsettings.FromEmail,
                
                Subject = "Reset Password",
                Body = profile.Body,
                DisplayName = quotationemailsettings.DisplayName,
                ClientId = ClientId,
                RecordStatusId = 1,
                Host = quotationemailsettings.Host,
                Port = quotationemailsettings.Port,
                UserId = quotationemailsettings.UserId,
                Password = quotationemailsettings.Password,
                FilePath = ""
            };

            Sendmailstoall sendmailstoall = new Sendmailstoall();
            sendmailstoall.SendMail(query);

            return Json(
                new { Message = profile.Message, Success = profile.IsSuccess }
               
            );
        }














    }
}
