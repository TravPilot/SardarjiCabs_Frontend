namespace SardarJi_Cab_Booking.Models
{
    public class CabCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }

        public CabCategory() { IsActive = true; }
    }

    public class LocationMaster
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsAirport { get; set; }
    }
    public class HomeIndexViewModel
    {
        public List<CabCategory> Categories { get; set; } = new();
        //public List<LocationMaster> Locations { get; set; } = new();
        public string GoogleMapsApiKey { get; set; }
    }
    public class SelectRideViewModel
    {
        public string Pickup { get; set; }
        public string Drop { get; set; }
        public double DistanceKm { get; set; }
        public string RideDate { get; set; }
        public string RideTime { get; set; }
        public int? CategoryId { get; set; }
       public string GoogleMapsApiKey { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public double DropLat { get; set; }
        public double DropLng { get; set; }
        public decimal? statetax { get; set; }
        public List<AvailableCarWithFare> Cars { get; set; } = new List<AvailableCarWithFare>();

    }
    public class CarMaster
    {
        public int CarId { get; set; }
        public int CarTypeId { get; set; }
        public string CarName { get; set; }
        public string Model { get; set; }
        public string RegistrationNo { get; set; }
        public string Color { get; set; }
        public string FuelType { get; set; }
        public decimal BaseFare { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal DriverAllowance { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }

        
        public decimal EstimatedCost { get; set; }
    }

    public class GoogleGeoResponse
    {
        public List<Result> results { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}
