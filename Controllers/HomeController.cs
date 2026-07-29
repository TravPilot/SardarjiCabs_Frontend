using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using CabBookingMVC.Helper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
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
      

     

        //public async Task<IActionResult> Index()
        //{
        //    using var conn = _db.CreateConnection();


        //    var allCategories = await conn.QueryAsync<CabCategory>(
        //        "dbo.usp_CabCategory_GetAll", commandType: CommandType.StoredProcedure);
        //    var allLocations = await conn.QueryAsync<LocationMaster>(
        //        "dbo.usp_LocationMaster_GetAll", commandType: CommandType.StoredProcedure);

        //    var activeCategories = allCategories
        //        .Where(c => c.IsActive)
        //        .OrderBy(c => c.CategoryId)
        //        .ToList();


        //    var uploadsBaseUrl = _config["AdminUploadsBaseUrl"] ?? "https://localhost:7209";
        //    foreach (var cat in activeCategories)
        //    {
        //        if (!string.IsNullOrEmpty(cat.Icon) && !string.IsNullOrEmpty(uploadsBaseUrl))
        //        {
        //            cat.Icon = uploadsBaseUrl.TrimEnd('/') + cat.Icon;
        //        }
        //    }

        //    var locations = allLocations
        //        .OrderBy(l => l.LocationName)
        //        .ToList();

        //    var vm = new HomeIndexViewModel
        //    {
        //        Categories = activeCategories,
        //        Locations = locations
        //    };

        //    return View(vm);
        //}

        //public async Task<IActionResult> Index()
        //{
        //    using var conn = _db.CreateConnection();

        //    var allCategories = await conn.QueryAsync<CabCategory>(
        //        "dbo.usp_CabCategory_GetAll", commandType: CommandType.StoredProcedure);
        //    var allLocations = await conn.QueryAsync<LocationMaster>(
        //        "dbo.usp_LocationMaster_GetAll", commandType: CommandType.StoredProcedure);

        //    var activeCategories = allCategories
        //        .Where(c => c.IsActive)
        //        .OrderBy(c => c.CategoryId)
        //        .ToList();

        //    var uploadsBaseUrl = _config["AdminUploadsBaseUrl"] ?? "https://localhost:7209";
        //    foreach (var cat in activeCategories)
        //    {
        //        if (!string.IsNullOrEmpty(cat.Icon) && !string.IsNullOrEmpty(uploadsBaseUrl))
        //        {
        //            cat.Icon = uploadsBaseUrl.TrimEnd('/') + cat.Icon;
        //        }
        //    }

        //    var locations = allLocations
        //        .OrderBy(l => l.LocationName)
        //        .ToList();

        //    var vm = new HomeIndexViewModel
        //    {
        //        Categories = activeCategories,
        //        Locations = locations,
        //        GoogleMapsApiKey = _config["GoogleMapsApiKey"] 
        //    };

        //    return View(vm);
        //}

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
                //car.EstimatedCost = (car.BaseFare ?? 0)
                //    + ((car.PricePerKm ?? 0) * (decimal)distKm)
                //    + (car.DriverAllowance ?? 0);
                car.EstimatedCost = ((car.PricePerKm ?? 0) * (decimal)distKm);


            }

            var pickupCoords = await GeocodeAsync(pickup);
            var dropCoords = await GeocodeAsync(drop);

            var vm = new SelectRideViewModel
            {
                Pickup = pickup,
                Drop = drop,
                
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

            var json = JsonSerializer.Serialize(requestBody);

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


        #region old code
        //[HttpGet]
        //public async Task<IActionResult> select_ride_map(string pickup, string drop, string distanceKm,string date, string time, int? categoryId)
        //{
        //    double.TryParse(distanceKm, out double distKm);

        //    using var conn = _db.CreateConnection();
        //    var p = new DynamicParameters();
        //    p.Add("CarTypeId", categoryId);
        //    var cars = (await conn.QueryAsync<CarMaster>(
        //        "dbo.usp_CarMaster_GetByType", p, commandType: CommandType.StoredProcedure)).ToList();

        //    foreach (var car in cars)
        //    {
        //        car.EstimatedCost = car.BaseFare
        //            + (car.PricePerKm * (decimal)distKm)
        //            + car.DriverAllowance;
        //    }

        //    var pickupCoords = await GeocodeAsync(pickup);
        //    var dropCoords = await GeocodeAsync(drop);

        //    var vm = new SelectRideViewModel
        //    {
        //        Pickup = pickup,
        //        Drop = drop,
        //        DistanceKm = distKm,
        //        RideDate = date,
        //        RideTime = time,
        //        CategoryId = categoryId,
        //        Cars = cars,
        //        PickupLat = pickupCoords.lat,
        //        PickupLng = pickupCoords.lng,
        //        DropLat = dropCoords.lat,
        //        DropLng = dropCoords.lng
        //    };

        //    return View(vm);
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetDistance(string origin, string destination)
        //{
        //    if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
        //        return BadRequest("Origin and destination are required.");

        //    var apiKey = _config["GoogleDistanceApiKey"];

        //    var url = $"https://routes.googleapis.com/directions/v2:computeRoutes" +
        //              $"?origins={Uri.EscapeDataString(origin)}" +
        //              $"&destinations={Uri.EscapeDataString(destination)}" +
        //              $"&units=metric" +
        //              $"&key={apiKey}";

        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.GetStringAsync(url);

        //    using var doc = JsonDocument.Parse(response);
        //    var root = doc.RootElement;

        //    if (root.GetProperty("status").GetString() != "OK")
        //        return BadRequest("Unable to calculate distance.");

        //    var element = root.GetProperty("rows")[0]
        //                      .GetProperty("elements")[0];

        //    if (element.GetProperty("status").GetString() != "OK")
        //        return BadRequest("No route found between these locations.");

        //    var distanceMeters = element.GetProperty("distance")
        //                                .GetProperty("value")
        //                                .GetDouble();

        //    var distanceText = element.GetProperty("distance")
        //                              .GetProperty("text")
        //                              .GetString();

        //    var durationText = element.GetProperty("duration")
        //                              .GetProperty("text")
        //                              .GetString();

        //    return Ok(new
        //    {
        //        distanceKm = Math.Round(distanceMeters / 1000.0, 2),
        //        distanceText,
        //        durationText
        //    });
        //}

        //public IActionResult Dateview()
        //{

        //    return View();
        //}


        #endregion old code




        public async Task<IActionResult> Travel_summary_ticket(string pickup, string drop, double distanceKm, string date, string time, int carId, string carName, decimal cost)
        {
            var car = await _carService.GetCarByIdAsync(carId);

            CustomerVM customer = HttpContext.Session.GetObject<CustomerVM>("customer");
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

      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


