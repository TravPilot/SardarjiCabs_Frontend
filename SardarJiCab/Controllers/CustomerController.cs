using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
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


        [HttpGet]
        public async Task<IActionResult> TrackRide(int bookingId)
        {
            LiveLocation rideLocation = await _booking.GetLiveLocation(bookingId);

            if (rideLocation == null)
                return NotFound();

            var vm = new TrackRideViewModel
            {
                BookingId = rideLocation.bookingid,
                Latitude = rideLocation.Latitude,
                Longitude = rideLocation.longitute,
                DriverName = rideLocation.DriverName,
                GoogleMapsApiKey = _config["GoogleMapsApiKey"] ?? ""
            };

            return View(vm);

        }

        [HttpGet]
        public async Task<IActionResult> LiveLocationJson(int bookingId)
        {
            LiveLocation rideLocation = await _booking.GetLiveLocation(bookingId);

            if (rideLocation == null)
                return NotFound();

            return Json(new
            {
                latitude = rideLocation.Latitude,
                longitude = rideLocation.longitute,
                driverName = rideLocation.DriverName
            });
        }





















        [HttpGet]
        public async Task<IActionResult> TrackkkkRide(int bookingId)
        {
            
            var ride = await GetBookingByIdAsync(bookingId); 
            if (ride == null) return NotFound();

            double dropLat = ride.DropLat;
            double dropLng = ride.DropLng;

            
            if (dropLat == 0 && dropLng == 0)
            {
                var geocoded = await _geocodingService.GeocodeAsync(ride.DropAddress);
                if (geocoded != null)
                {
                    dropLat = geocoded.Value.Lat;
                    dropLng = geocoded.Value.Lng;
                    // ride.DropLat = dropLat; ride.DropLng = dropLng; await _db.SaveChangesAsync();
                }
            }

            var vm = new TrackRideViewModel
            {
                BookingId = ride.BookingId,
                BookingNo = ride.BookingNo,
                PickupAddress = ride.PickupAddress,
                DropAddress = ride.DropAddress,
                DriverName = ride.DriverName,
                VehicleNumber = ride.VehicleNumber,
                Status = ride.Status,
                CarImage = ride.CarImage,
                DropLat = dropLat,
                DropLng = dropLng,
                GoogleMapsApiKey = _config["GoogleMaps:GoogleMapsApiKey"] ?? ""
            };

            return View(vm);
        }

        
        private Task<BookingPlaceholder?> GetBookingByIdAsync(int bookingId)
        {
            var ride = new BookingPlaceholder
            {
                BookingId = bookingId,
                BookingNo = "SJ-7377",
                PickupAddress = "TraviYo, F Block, Sector 6, Noida, Uttar Pradesh, India",
                DropAddress = "Sector 12, Vasundhara, Ghaziabad, Uttar Pradesh 201012, India",
                DriverName = "Shivam thapa",
                VehicleNumber = "UP13DC0008",
                Status = "Confirmed",
                CarImage = "/images/bmw-i7.png",
                DropLat = 0,
                DropLng = 0
            };
            return Task.FromResult<BookingPlaceholder?>(ride);
        }





    }

   








}

