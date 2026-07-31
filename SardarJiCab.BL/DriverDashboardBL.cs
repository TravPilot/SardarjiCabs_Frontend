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
    public class DriverDashboardBL : IDriverDashboardBL
    {
        private readonly IDriverDashboardDB _driverDashboardDB;
        private const int RecentTripsCount = 5;

        public DriverDashboardBL(IDriverDashboardDB driverDashboardDB)
        {
            _driverDashboardDB = driverDashboardDB;
        }

        public async Task<DriverDashboard> GetDashboardAsync(int driverId)
        {
            var summary = await _driverDashboardDB.GetDriverSummaryAsync(driverId);
            if (summary == null)
                return null;

            var todayEarnings = await _driverDashboardDB.GetTodayEarningsAsync(driverId);
            var recentTrips = await _driverDashboardDB.GetRecentTripsAsync(driverId, RecentTripsCount);
            var unreadCount = await _driverDashboardDB.GetUnreadNotificationCountAsync(driverId);

            return new DriverDashboard
            {
                FullName = summary.FullName,
                ProfilePhotoUrl = summary.ProfilePhotoUrl,
                IsOnline = summary.IsOnline,
                ApprovalStatus = summary.ApprovalStatus,
                RejectionReason = summary.RejectionReason,
                TotalTrips = summary.TotalTrips,
                AverageRating = summary.AverageRating,
                TotalCo2SavedKg = summary.TotalCo2SavedKg,
                TodayEarnings = todayEarnings,
                HasUnreadNotifications = unreadCount > 0,
                RecentTrips = recentTrips.Select(t => new RecentTrip
                {
                    PickupArea = t.PickupArea,
                    DropArea = t.DropArea,
                    Fare = t.Fare,
                    Status = t.Status,
                    CompletedAt = t.CompletedAt
                }).ToList()
            };
        }

        public async Task<StatusUpdateResult> SetOnlineStatusAsync(int driverId, bool isOnline)
        {
            var summary = await _driverDashboardDB.GetDriverSummaryAsync(driverId);
            if (summary == null)
                return new StatusUpdateResult { Success = false, Message = "Driver account not found." };

            if (isOnline && summary.ApprovalStatus != "Approved")
                return new StatusUpdateResult { Success = false, Message = "Your account isn't approved yet." };

            var updated = await _driverDashboardDB.UpdateOnlineStatusAsync(driverId, isOnline);
            if (!updated)
                return new StatusUpdateResult { Success = false, Message = "Could not update your status. Please try again." };

            return new StatusUpdateResult
            {
                Success = true,
                Message = isOnline ? "You're now online." : "You're now offline."
            };
        }

    }
}
