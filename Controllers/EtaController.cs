using Microsoft.AspNetCore.Mvc;

namespace SardarJi_Cab_Booking.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class EtaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public EtaController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetEta(
            [FromQuery] double originLat,
            [FromQuery] double originLng,
            [FromQuery] double destLat,
            [FromQuery] double destLng)
        {
            var apiKey = _config["GoogleDistanceApiKey"];
            var client = _httpClientFactory.CreateClient();

            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                      $"?origins={originLat},{originLng}" +
                      $"&destinations={destLat},{destLng}" +
                      $"&units=metric" +
                      $"&key={apiKey}";

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode,
                        new
                        {
                            error = "Google Distance Matrix API returned an error."
                        });
                }

                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message
                });
            }
        }
    }

}
