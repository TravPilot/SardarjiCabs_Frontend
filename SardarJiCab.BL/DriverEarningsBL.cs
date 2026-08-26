using SardarjiCab.DB.Interface;
using SardarJiCab.BL.Interface;
using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL
{
    public class DriverEarningsBL : IDriverEarningsBL
    {
        private readonly IDriverEarningsDB _driverEarningsDB;

        public DriverEarningsBL(IDriverEarningsDB driverEarningsDB)
        {
            _driverEarningsDB = driverEarningsDB;
        }

        public async Task<DriverEarnings> GetEarningsAsync(int driverId, string period)
        {
            period = (period ?? "all").ToLowerInvariant();
            var (fromDate, label) = ResolvePeriod(period);

            var summary = await _driverEarningsDB.GetEarningsSummaryAsync(driverId, fromDate, null);
            var trips = await _driverEarningsDB.GetEarningsTripsAsync(driverId, fromDate, null);

            return new DriverEarnings
            {
                SelectedPeriod = period,
                PeriodLabel = label,
                TotalEarnings = summary.TotalEarnings,
                TotalFare = summary.TotalFare,
                TotalTrips = summary.TotalTrips,
                CashEarnings = summary.CashEarnings,
                OnlineEarnings = summary.OnlineEarnings,
                Trips = trips.Select(t => new EarningsTrip
                {
                    BookingId = t.BookingId,
                    BookingNo = t.BookingNo,
                    PickupArea = t.PickupArea,
                    DropArea = t.DropArea,
                    TotalFare = t.TotalFare,
                    NetPayable = t.NetPayable,
                    PaymentMethod = t.PaymentMethod,
                    CompletedOn = t.CompletedOn
                }).ToList()
            };
        }

        private static (DateTime? fromDate, string label) ResolvePeriod(string period)
        {
            var today = DateTime.UtcNow.Date; // adjust to IST if your server isn't already IST

            return period switch
            {
                "today" => (today, "Today's"),
                "week" => (today.AddDays(-(int)today.DayOfWeek), "This week's"),
                "month" => (new DateTime(today.Year, today.Month, 1), "This month's"),
                _ => ((DateTime?)null, "All-time")
            };
        }
    }
}
