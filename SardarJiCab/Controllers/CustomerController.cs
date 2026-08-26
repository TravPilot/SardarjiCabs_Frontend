using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly IHttpClientFactory _httpClientFactory;


        public CustomerController(ICustomerService customerService, IConfiguration config, IBookingService booking, IInvoiceService invoiceService, IGeocodingService geocodingService, IHttpClientFactory httpClientFactory)
        {

            _customerService = customerService;
            _config = config;
            _booking = booking;
            _invoiceService = invoiceService;
            _geocodingService = geocodingService;
            _httpClientFactory = httpClientFactory;
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
            //if (profile.ProfileImageFile != null && profile.ProfileImageFile.Length > 0)
            //{
            //    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profile");
            //    Directory.CreateDirectory(uploadsFolder);

            //    var fileName = $"{customer.Id}_{Guid.NewGuid()}{Path.GetExtension(profile.ProfileImageFile.FileName)}";
            //    var filePath = Path.Combine(uploadsFolder, fileName);

            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await profile.ProfileImageFile.CopyToAsync(stream);
            //    }

                
            //    customerEntity.LogoPath = $"/uploads/profile/{fileName}";
            //}

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

<<<<<<< HEAD
            return View();
        }


        #region Driver Loction 
=======
>>>>>>> parent of b370c3a (commit chage)

        [HttpGet]
        public async Task<IActionResult> TrackRide(int bookingId)
        {
<<<<<<< HEAD
            double lat;
            double lng;
            string driverName;
            
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");

            if (customer == null)
            {
                return RedirectToAction("Index", "LogIn");
            }

            TravelSummaryViewModel bookinglist =
                await _booking.GetBookingList(customer.Id);

            bookinglist.TodayRide = bookinglist.Bookings
                .FirstOrDefault(x => x.BookingId == bookingId);

            if (bookinglist.TodayRide == null)
            {
                return View("RideUnavailable", bookingId);
            }

            var rideLocations = await _booking.GetLiveLocation(bookingId);

            if (rideLocations == null || !rideLocations.Any())
            {
                return View("RideUnavailable", bookingId);
            }

            var latestLocation = rideLocations
                .OrderByDescending(x => x.UpdatedAt)
                .First();

            lat = latestLocation.Latitude;
            lng = latestLocation.Longitude;
            driverName = latestLocation.DriverName;

            // Pickup location
            double? pickupLat = null;
            double? pickupLng = null;

            if (!string.IsNullOrWhiteSpace(bookinglist.TodayRide.PickupAddress))
            {
                var pickupCoords = await GeocodeAsync(
                    bookinglist.TodayRide.PickupAddress
                );

                pickupLat = pickupCoords.lat;
                pickupLng = pickupCoords.lng;
            }

            // Drop location
            double? dropLat = null;
            double? dropLng = null;

            if (!string.IsNullOrWhiteSpace(bookinglist.TodayRide.DropAddress))
            {
                var dropCoords = await GeocodeAsync(
                    bookinglist.TodayRide.DropAddress
                );

                dropLat = dropCoords.lat;
                dropLng = dropCoords.lng;
            }

           
            string ridePhase = "toPickup";

            if (!string.IsNullOrWhiteSpace(bookinglist.TodayRide.Status))
            {
                var statusLower = bookinglist.TodayRide.Status.ToLowerInvariant();

                if (statusLower.Contains("progress") || statusLower.Contains("picked"))
                {
                    ridePhase = "toDrop";
                }
                else if (statusLower.Contains("complete"))
                {
                    ridePhase = "completed";
                }
            }

            var vm = new TrackRideViewModel
            {
                BookingId = bookingId,

                Latitude = lat,
                Longitude = lng,

                DriverName = driverName,

                GoogleMapsApiKey = _config["GoogleMapsApiKey"],

                PickupAddress = bookinglist.TodayRide?.PickupAddress,

                PickupLatitude = pickupLat,
                PickupLongitude = pickupLng,

                DropAddress = bookinglist.TodayRide?.DropAddress,

                DropLatitude = dropLat,
                DropLongitude = dropLng,

                RidePhase = ridePhase,

                Status = bookinglist.TodayRide?.Status
=======
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
>>>>>>> parent of b370c3a (commit chage)
            };

            return View(vm);

        }


        private async Task<(double lat, double lng)> GeocodeAsync(string address)
        {
            // Default location
            const double defaultLat = 28.6139;
            const double defaultLng = 77.2090;

            if (string.IsNullOrWhiteSpace(address))
            {
                return (defaultLat, defaultLng);
            }

            var apiKey = _config["GoogleMapsApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return (defaultLat, defaultLng);
            }

            var url =
                "https://maps.googleapis.com/maps/api/geocode/json" +
                $"?address={Uri.EscapeDataString(address)}" +
                $"&key={apiKey}";

            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);

                var root = doc.RootElement;

                var status = root
                    .GetProperty("status")
                    .GetString();

                if (status != "OK")
                {
                    return (defaultLat, defaultLng);
                }

                var results = root.GetProperty("results");

                if (results.GetArrayLength() == 0)
                {
                    return (defaultLat, defaultLng);
                }

                var location = results[0]
                    .GetProperty("geometry")
                    .GetProperty("location");

                var latitude = location
                    .GetProperty("lat")
                    .GetDouble();

                var longitude = location
                    .GetProperty("lng")
                    .GetDouble();

                return (latitude, longitude);
            }
            catch
            {
                return (defaultLat, defaultLng);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LiveLocationJson(int bookingId)
        {
<<<<<<< HEAD
            var rideLocations = await _booking.GetLiveLocation(bookingId);
            var rideLocation = rideLocations?
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefault();

            if (rideLocation == null)
            {
                return Json(new
                {
                    latitude = (double?)null,
                    longitude = (double?)null,
                    driverName = (string)null,
                    ridePhase = (string)null
                });
            }
=======
            LiveLocation rideLocation = await _booking.GetLiveLocation(bookingId);

            if (rideLocation == null)
                return NotFound();
>>>>>>> parent of b370c3a (commit chage)

            string ridePhase = null;

            return Json(new
            {
                latitude = rideLocation.Latitude,
                longitude = rideLocation.Longitude,
                driverName = rideLocation.DriverName,
                ridePhase = ridePhase
            });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRideFeedback([FromBody] RideFeedbackDto dto)
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            dto.UserId = customer.Id;
            if (customer == null)
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            if (dto == null || dto.BookingId <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }
            var booking = await _booking.SaveRatingDetails(dto);
            
            return Json(new { success = true });
        }

<<<<<<< HEAD
        public class RideFeedbackDto
        {
            public int BookingId { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public long UserId { get; set; }
        }
=======

>>>>>>> parent of b370c3a (commit chage)

        #endregion  Driver Loction
















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

