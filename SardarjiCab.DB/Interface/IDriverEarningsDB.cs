using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverEarningsDB
    {
        Task<DriverEarnings> GetEarningsSummaryAsync(int driverId, DateTime? fromDate, DateTime? toDate);
        Task<List<EarningsTrip>> GetEarningsTripsAsync(int driverId, DateTime? fromDate, DateTime? toDate);
    }
}
