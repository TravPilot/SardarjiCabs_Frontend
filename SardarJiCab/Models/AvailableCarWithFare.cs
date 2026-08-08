namespace SardarJi_Cab_Booking.Models
{
  
    public class AvailableCarWithFare
    {
        public int CarId { get; set; }
        public int CarTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string CarType { get; set; } = string.Empty;
        public int? SeatingCapacity { get; set; }
        public int? LuggageCapacity { get; set; }
        public string? CarTypeImage { get; set; }
        public string Image { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string? Model { get; set; }
        public string? RegistrationNo { get; set; }
        public string? Color { get; set; }
        public string? FuelType { get; set; }
        public string? CarImage { get; set; }
        public bool IsAvailable { get; set; }
        public TimeSpan? PeakHourFromTime { get; set; }
        public TimeSpan? PeakHourToTime { get; set; }

        public int? FareId { get; set; }
        public decimal? BaseFare { get; set; }
        public decimal? PricePerKm { get; set; }
        public decimal? DriverAllowance { get; set; }
        public decimal? WaitingCharge { get; set; }
        public decimal? NightCharge { get; set; }
        public decimal? AirportCharge { get; set; }

        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string? DriverMobileNo { get; set; }
        public decimal? DriverRating { get; set; }
        public string? DriverPhoto { get; set; }

       
        public decimal EstimatedCost { get; set; }
        public decimal? StateTax { get; set; }

        public decimal? PeakHourCharge { get; set; }



        public bool IsPriced => BaseFare.HasValue || PricePerKm.HasValue;
    }

    public class CarModel
    {
        public int CarId { get; set; }
        public string CarName { get; set; }
        public string Model { get; set; }
        public string FuelType { get; set; }
        public string Color { get; set; }
        public string? RegistrationNo { get; set; }
        public string Image { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}
