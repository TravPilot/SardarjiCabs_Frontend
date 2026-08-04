using System.Text.Json;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class GeocodingService : IGeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public GeocodingService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        public async Task<(double Lat, double Lng)?> GeocodeAsync(string address)
        {
            
            var apiKey = _config["GoogleMapsApiKey"];
            var client = _httpClientFactory.CreateClient();

           
            var url = "https://maps.googleapis.com/maps/api/geocode/json" +
          $"?address={Uri.EscapeDataString(address)}" +
          $"&key={apiKey}";
        
            var json = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            if (root.GetProperty("status").GetString() != "OK")
                return null;

            var location = root.GetProperty("results")[0]
                                .GetProperty("geometry")
                                .GetProperty("location");

            return (location.GetProperty("lat").GetDouble(),
                    location.GetProperty("lng").GetDouble());
        }
    }
}
