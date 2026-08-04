using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class CabBooking
    {
        public long BookingId { get; set; }
        public string BookingNo { get; set; }
        public DateTime JourneyDate { get; set; }
        public string JourneyTime { get; set; }
        public string PassengerName { get; set; }
        public string ContactNumber { get; set; }
        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        public decimal TotalDistanceKm { get; set; }
        public string VehicleName { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleNumber { get; set; }
        public string CarImage { get; set; }
        public string PaymentMethod { get; set; }
        public decimal NetPayable { get; set; }
        public string Status { get; set; }
        public string Otp { get; set; }
        public DateTime? CompletedOn { get; set; }

        public string PassengerInitials =>
            string.IsNullOrWhiteSpace(PassengerName)
                ? "P"
                : string.Concat(PassengerName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2).Select(w => char.ToUpper(w[0])));

        public string StatusCssClass => (Status ?? "").Trim().ToLowerInvariant() switch
        {
            "confirmed" => "confirmed",
            "completed" => "completed",
            "cancelled" => "cancelled",
            "inprogress" => "inprogress",
            "in progress" => "inprogress",
            _ => "confirmed"
        };
    }
}
