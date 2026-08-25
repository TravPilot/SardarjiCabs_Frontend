using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverEarnings
    {
        public string SelectedPeriod { get; set; } = "all";
        public string PeriodLabel { get; set; } = "All-time";

        public decimal TotalEarnings { get; set; }
        public decimal TotalFare { get; set; }
        public int TotalTrips { get; set; }
        public decimal CashEarnings { get; set; }
        public decimal OnlineEarnings { get; set; }

        public decimal AverageEarningPerTrip =>
            TotalTrips > 0 ? Math.Round(TotalEarnings / TotalTrips, 0) : 0;

        public List<EarningsTrip> Trips { get; set; } = new();
    }
    public class EarningsTrip
    {
        public long BookingId { get; set; }
        public string BookingNo { get; set; }
        public string PickupArea { get; set; }
        public string DropArea { get; set; }
        public decimal TotalFare { get; set; }
        public decimal NetPayable { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? CompletedOn { get; set; }
    }
}
