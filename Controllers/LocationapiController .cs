//using System;
//using System.Data;
//using System.Linq;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Dapper;
//using Microsoft.AspNetCore.Connections;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using SardarJi_Cab_Booking.Models;

//namespace YourProject.Controllers // TODO: rename to match your actual namespace
//{
//    public class HomeController : Controller
//    {
//        private readonly IDbConnectionFactory _db;              // your existing Dapper connection factory
//        private readonly IConfiguration _config;
//        private readonly IHttpClientFactory _httpClientFactory; // ADDED

//        public HomeController(
//            IDbConnectionFactory db,
//            IConfiguration config,
//            IHttpClientFactory httpClientFactory)                // ADDED PARAMETER
//        {
//            _db = db;
//            _config = config;
//            _httpClientFactory = httpClientFactory;               // ADDED ASSIGNMENT
//        }

//        public async Task<IActionResult> Index()
//        {
//            using var conn = _db.CreateConnection();

//            var allCategories = await conn.QueryAsync<CabCategory>(
//                "dbo.usp_CabCategory_GetAll", commandType: CommandType.StoredProcedure);
//            var allLocations = await conn.QueryAsync<LocationMaster>(
//                "dbo.usp_LocationMaster_GetAll", commandType: CommandType.StoredProcedure);

//            var activeCategories = allCategories
//                .Where(c => c.IsActive)
//                .OrderBy(c => c.CategoryId)
//                .ToList();

//            var uploadsBaseUrl = _config["AdminUploadsBaseUrl"] ?? "https://localhost:7209";
//            foreach (var cat in activeCategories)
//            {
//                if (!string.IsNullOrEmpty(cat.Icon) && !string.IsNullOrEmpty(uploadsBaseUrl))
//                {
//                    cat.Icon = uploadsBaseUrl.TrimEnd('/') + cat.Icon;
//                }
//            }

//            var locations = allLocations
//                .OrderBy(l => l.LocationName)
//                .ToList();

//            var vm = new HomeIndexViewModel
//            {
//                Categories = activeCategories,
//                Locations = locations,
//                GoogleMapsApiKey = _config["GoogleMapsApiKey"]
//            };

//            return View(vm);
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetDistance(string origin, string destination)
//        {
//            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
//                return BadRequest("Origin and destination are required.");

//            var apiKey = _config["GoogleMapsApiKey"];
//            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
//                      $"?origins={Uri.EscapeDataString(origin)}" +
//                      $"&destinations={Uri.EscapeDataString(destination)}" +
//                      $"&units=metric&key={apiKey}";

//            var client = _httpClientFactory.CreateClient();
//            var response = await client.GetStringAsync(url);

//            using var doc = JsonDocument.Parse(response);
//            var root = doc.RootElement;

//            if (root.GetProperty("status").GetString() != "OK")
//                return BadRequest("Unable to calculate distance.");

//            var element = root.GetProperty("rows")[0].GetProperty("elements")[0];
//            if (element.GetProperty("status").GetString() != "OK")
//                return BadRequest("No route found between these locations.");

//            var distanceMeters = element.GetProperty("distance").GetProperty("value").GetDouble();
//            var distanceText = element.GetProperty("distance").GetProperty("text").GetString();
//            var durationText = element.GetProperty("duration").GetProperty("text").GetString();

//            return Ok(new
//            {
//                distanceKm = Math.Round(distanceMeters / 1000.0, 2),
//                distanceText,
//                durationText
//            });
//        }
//    }
//}

