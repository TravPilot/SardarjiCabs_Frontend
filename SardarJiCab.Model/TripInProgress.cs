using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class TripInProgressViewModel
    {
        public long BookingId { get; set; }
        public string BookingNo { get; set; }
        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        public string PassengerName { get; set; }
        public string ContactNumber { get; set; }
        public string GoogleMapsApiKey { get; set; }
        public decimal NetPayable { get; set; }

        public string PassengerInitials =>
            string.IsNullOrWhiteSpace(PassengerName)
                ? "P"
                : string.Concat(PassengerName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2).Select(w => char.ToUpper(w[0])));
    }
}
