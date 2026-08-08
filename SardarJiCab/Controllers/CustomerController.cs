using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class CustomerController : Controller
    {


        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IBookingService _booking;
        private readonly IInvoiceService _invoiceService;
        private readonly IGeocodingService _geocodingService;
        

        public CustomerController(ICustomerService customerService, IConfiguration config, IBookingService booking, IInvoiceService invoiceService, IGeocodingService geocodingService)
        {

            _customerService = customerService;
            _config = config;
            _booking = booking;
            _invoiceService = invoiceService;
            _geocodingService = geocodingService;
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

        public async Task<JsonResult> UpdateProfile(ProfileUpdateRequest profile)
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            if (customer == null)
            {
                return Json(new { success = false, message = "Session expired. Please log in again." });
            }

            long clientId = Convert.ToInt64(_config["ClientId"]);

            var result = await _customerService.UpdateProfileFields(
                customer.Id, clientId, profile);

            return Json(new
            {
                success = result?.Success ?? false,
                message = result?.Message ?? "Profile update failed."
               
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
                FilePath = null
            };

            Sendmailstoall sendmailstoall = new Sendmailstoall();
            sendmailstoall.SendMail(query);

            return Json(
                new { Message = profile.Message, Success = profile.IsSuccess }
               
            );
        }
        public ActionResult GetWalletBalance()
        {
           // WalletVM walletDetails = await _customerService.GetWalletDetails(customer.Id);

            return View();
        }


        #region Driver Loction 


        private const bool UseMockLiveLocation = false;


        private static readonly (double Lat, double Lng)[] DemoDriverPath = new[]
        {
        (28.6120, 77.3705),
        (28.6080, 77.3660),
        (28.6035, 77.3600),
        (28.5990, 77.3540),
        (28.5945, 77.3480),
        (28.5900, 77.3420),
        (28.5875, 77.3360),
        (28.5860, 77.3300),
        (28.5845, 77.3240),
        (28.5837, 77.3178),
    };

        private const int DemoStepSeconds = 2;
        private const int DemoHoldSeconds = 8;


        private static readonly ConcurrentDictionary<int, DateTime> DemoStartTimes = new();

        private (double Lat, double Lng, string DriverName) GetMockDriverLocation(int bookingId)
        {
            var startedAt = DemoStartTimes.GetOrAdd(bookingId, _ => DateTime.UtcNow);
            var elapsedSeconds = (DateTime.UtcNow - startedAt).TotalSeconds;


            var travelSeconds = (DemoDriverPath.Length - 1) * DemoStepSeconds;
            var cycleSeconds = 2 * (travelSeconds + DemoHoldSeconds);
            var t = elapsedSeconds % cycleSeconds;

            int stepIndex;
            if (t < travelSeconds)
            {
                stepIndex = (int)(t / DemoStepSeconds);
            }
            else if (t < travelSeconds + DemoHoldSeconds)
            {
                stepIndex = DemoDriverPath.Length - 1;
            }
            else if (t < 2 * travelSeconds + DemoHoldSeconds)
            {
                var back = t - (travelSeconds + DemoHoldSeconds);
                stepIndex = DemoDriverPath.Length - 1 - (int)(back / DemoStepSeconds);
            }
            else
            {
                stepIndex = 0;
            }

            stepIndex = Math.Clamp(stepIndex, 0, DemoDriverPath.Length - 1);
            var point = DemoDriverPath[stepIndex];
            return (point.Lat, point.Lng, "Raju Sharma (test)");
        }

        [HttpGet]
        public async Task<IActionResult> TrackRide(int bookingId)
        {
            double lat, lng;
            string driverName;

            if (UseMockLiveLocation)
            {
                var mock = GetMockDriverLocation(bookingId);
                lat = mock.Lat;
                lng = mock.Lng;
                driverName = mock.DriverName;
            }
            else
            {
                var rideLocations = await _booking.GetLiveLocation(bookingId);
                if (rideLocations == null || !rideLocations.Any())
                {
                    return View("RideUnavailable", bookingId);
                }

                var latestLocation = rideLocations
                    .OrderByDescending(x => x.UpdatedAt)
                    .First();

                lat = latestLocation.Latitude;
                lng = latestLocation.longitute;
                driverName = latestLocation.DriverName;
            }

            var vm = new TrackRideViewModel
            {
                BookingId = bookingId,
                Latitude = lat,
                Longitude = lng,
                DriverName = driverName,
                GoogleMapsApiKey = _config["GoogleMapsApiKey"],


                PickupLatitude = UseMockLiveLocation ? DemoDriverPath.Last().Lat : (double?)null,
                PickupLongitude = UseMockLiveLocation ? DemoDriverPath.Last().Lng : (double?)null,


            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> LiveLocationJson(int bookingId)
        {
            if (UseMockLiveLocation)
            {
                var mock = GetMockDriverLocation(bookingId);
                return Json(new
                {
                    latitude = (double?)mock.Lat,
                    longitude = (double?)mock.Lng,
                    driverName = mock.DriverName
                });
            }

            var rideLocations = await _booking.GetLiveLocation(bookingId);
            var rideLocation = rideLocations?
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefault();

            if (rideLocation == null)
            {
                return Json(new { latitude = (double?)null, longitude = (double?)null, driverName = (string)null });
            }

            return Json(new
            {
                latitude = rideLocation.Latitude,
                longitude = rideLocation.longitute,
                driverName = rideLocation.DriverName
            });
        }



        #endregion  Driver Loction 












        public async Task<IActionResult> Contactus()
        {
            //var customer = HttpContext.Session.GetObject<CustomerVM>("customer");

            //if (customer == null)
            //{
            //    return RedirectToAction("Index", "Customer");
            //}

            //CustomerProfile customerrr = await _customerService.GetCustomerProfile(customer.Id);

            //TempData["Mobile"] = customerrr.Mobile;
            //TempData["Email"] = customerrr.Email;

            //HttpContext.Session.SetObject("customerProfile", customerrr);


            return View();
        }





















    }

   








}

