using SardarJiCab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarjiCab.DB.Interface
{
    public interface IDriverDashboardDB
    {
        Task<DriverDashboard> GetDriverSummaryAsync(int driverId);
        Task<decimal> GetTodayEarningsAsync(int driverId);
        Task<List<RecentTrip>> GetRecentTripsAsync(int driverId, int top);
        Task<bool> UpdateOnlineStatusAsync(int driverId, bool isOnline);
        Task<int> GetUnreadNotificationCountAsync(int driverId);
    }
}
