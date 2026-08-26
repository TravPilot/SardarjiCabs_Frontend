using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using CabBookingMVC.Helper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarService _carService;
        private readonly DapperContext _db;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(DapperContext db, IConfiguration config, IHttpClientFactory httpClientFactory, ICarService carService)
        {
            _db = db;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _carService = carService; ;
        }
      

        public async Task<IActionResult> Index()
        {
            using var conn = _db.CreateConnection();

            var allCategories = await conn.QueryAsync<CabCategory>(
                "dbo.usp_CabCategory_GetAll", commandType: CommandType.StoredProcedure);

            var activeCategories = allCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.CategoryId)
                .ToList();

            var uploadsBaseUrl = _config["AdminUploadsBaseUrl"] ?? "https://adminsardarji.traviyo.in";
            foreach (var cat in activeCategories)
            {
                if (!string.IsNullOrEmpty(cat.Icon) && !string.IsNullOrEmpty(uploadsBaseUrl))
                {
                    cat.Icon = uploadsBaseUrl.TrimEnd('/') + cat.Icon;
                }
            }

          
            var vm = new HomeIndexViewModel
            {
                Categories = activeCategories,
                GoogleMapsApiKey = _config["GoogleMapsApiKey"]
            };

            return View(vm);
        }

        public IActionResult Dateview()
        {
            
            return View();
        }
 
        [HttpGet]
        public async Task<IActionResult> select_ride_map(string pickup, string drop, string distanceKm, string date, string time, int? categoryId)
        {

            string pickupState = await GetStateFromAddress(pickup);
            string dropState = await GetStateFromAddress(drop);
            double.TryParse(distanceKm, out double distKm);

            DateTime? journeyDateTime = null;
            if (!string.IsNullOrWhiteSpace(date) && !string.IsNullOrWhiteSpace(time)
                && DateTime.TryParse($"{date} {time}", out var parsed))
            {
                journeyDateTime = parsed;
            }

            using var conn = _db.CreateConnection();
            var p = new DynamicParameters();
            p.Add("CategoryId", categoryId);
            p.Add("JourneyDate", journeyDateTime);
            p.Add("BufferMinutes", 60);

            var cars = (await conn.QueryAsync<AvailableCarWithFare>(
                "dbo.usp_CarMaster_GetAvailableByCategory", p, commandType: CommandType.StoredProcedure)).ToList();

            foreach (var car in cars)
            {
                decimal stateTax = pickupState.Equals(dropState, StringComparison.OrdinalIgnoreCase) ? 0 : (car.StateTax ?? 0);
                decimal estimatedCost = (car.PricePerKm ?? 0) * (decimal)distKm;
                TimeSpan bookingTime = DateTime.Now.TimeOfDay;
                if (bookingTime >= car.PeakHourFromTime &&
                    bookingTime <= car.PeakHourToTime)
                {
                    estimatedCost += (car.PeakHourCharge ?? 0);
                }
                estimatedCost += stateTax;
                car.EstimatedCost = estimatedCost;
               
            }

            var pickupCoords = await GeocodeAsync(pickup);
            var dropCoords = await GeocodeAsync(drop);

            var vm = new SelectRideViewModel
            {
                Pickup = pickup,
                Drop = drop,
                GoogleMapsApiKey = _config["GoogleMapsApiKey"],
                statetax = cars.Count > 0 ? cars[0].StateTax : 0,
                DistanceKm = distKm,
                RideDate = date,
                RideTime = time,
                CategoryId = categoryId,
                Cars = cars,
                PickupLat = pickupCoords.lat,
                PickupLng = pickupCoords.lng,
                DropLat = dropCoords.lat,
                DropLng = dropCoords.lng
            };

            return View(vm);
        }

        private async Task<(double lat, double lng)> GeocodeAsync(string address)
        {
            
            if (string.IsNullOrWhiteSpace(address))
                return (28.6139, 77.2090);

            var apiKey = _config["GoogleMapsApiKey"];

            var url = $"https://maps.googleapis.com/maps/api/geocode/json" +
                      $"?address={Uri.EscapeDataString(address)}" +
                      $"&key={apiKey}";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetString() != "OK")
                return (28.6139, 77.2090);

            var location = root.GetProperty("results")[0]
                               .GetProperty("geometry")
                               .GetProperty("location");

            return (
                location.GetProperty("lat").GetDouble(),
                location.GetProperty("lng").GetDouble()
            );
        }


        [HttpGet]
        public async Task<IActionResult> GetDistance(string origin, string destination)
        {
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
                return BadRequest("Origin and destination are required.");

            var apiKey = _config["GoogleDistanceApiKey"];

            var client = _httpClientFactory.CreateClient();

            
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
            client.DefaultRequestHeaders.Add("X-Goog-FieldMask", "routes.distanceMeters,routes.duration");

            var requestBody = new
            {
                origin = new
                {
                    address = origin
                },
                destination = new
                {
                    address = destination
                },
                travelMode = "DRIVE",
                routingPreference = "TRAFFIC_UNAWARE"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://routes.googleapis.com/directions/v2:computeRoutes",
                content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(responseContent);
            }

            using var document = JsonDocument.Parse(responseContent);

            var root = document.RootElement;

            if (!root.TryGetProperty("routes", out JsonElement routes) ||
                routes.GetArrayLength() == 0)
            {
                return BadRequest("No route found.");
            }

            var route = routes[0];

            double distanceMeters = route.GetProperty("distanceMeters").GetDouble();

            string duration = route.GetProperty("duration").GetString();

            return Ok(new
            {
                distanceKm = Math.Round(distanceMeters / 1000, 2),
                distanceMeters = distanceMeters,
                duration = duration
            });
        }




        public async Task<IActionResult> Travel_summary_ticket(string pickup, string drop, double distanceKm, string date, string time, int carId, string carName, decimal cost,decimal statetax)
        {
            CustomerVM customer = HttpContext.Session.GetObject<CustomerVM>("customer");

            if (customer == null)
            {
                return RedirectToAction("Index", "LogIn");
            }
            var car = await _carService.GetCarByIdAsync(carId);

            string pickupState = await GetStateFromAddress(pickup);
            string dropState = await GetStateFromAddress(drop);


            decimal stateTax = pickupState.Equals(dropState, StringComparison.OrdinalIgnoreCase) ? 0 : (statetax);


            var model = new TravelSummaryViewModel
            {
                RideCode = "SJ-" + new Random().Next(1000, 9999),

                Pickup = pickup,
                Drop = drop,
                DistanceKm = distanceKm,
                RideDate = date,
                RideTime = time,
                CarId = carId,
                CarName = carName,
                Cost = cost,
               RegistrationNo=car.RegistrationNo,
                CarImage = string.IsNullOrEmpty(car?.Image)
                    ? "https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?w=400&q=80"
                    : "https://adminsardarji.traviyo.in" + car.Image,
                CarModelName = car?.Model,
                FuelType = car?.FuelType,
                Color = car?.Color,

                PassengerName = customer.FirstName +" "+ customer.LastName ?? "Guest",
                PassengerContact = customer.Mobile,

                Discount = 0,
                TollCharges = 0,
                StateCharges = stateTax,
                BarcodeCaption = $"SJ · {DateTime.Now:HHmm} · {pickup}–{drop}"
            };
            if (model != null)
            {
                HttpContext.Session.SetObject("Cardetails", model);
            }

            return View(model);
        }

        public IActionResult Select_ride_map()
        {

            return View();
        }
        public IActionResult Confirmation()
        {

            return View();
        }
        public async Task<string> GetStateFromAddress(string address)
        {
            string apiKey = _config["GoogleMapsApiKey"];

            using (HttpClient client = new HttpClient())
            {
                string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

                var response = await client.GetStringAsync(url);

                var result = JsonConvert.DeserializeObject<GoogleGeoResponse>(response);

                if (result != null && result.results.Any())
                {
                    var state = result.results[0].address_components
                        .FirstOrDefault(x => x.types.Contains("administrative_area_level_1"));

                    return state?.long_name;
                }
            }

            return "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CarbonImpact()
        {

            return View();
        }


    }
}


