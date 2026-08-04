namespace SardarJi_Cab_Booking.Business_Layer
{
    public interface IGeocodingService
    {
        Task<(double Lat, double Lng)?> GeocodeAsync(string address);
    }
}
