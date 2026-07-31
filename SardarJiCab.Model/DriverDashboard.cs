using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverDashboard : Driver
    {
        public string FirstName => (FullName ?? "Driver").Split(' ')[0];
        public string ProfilePhotoUrl { get; set; }

        public bool IsOnline { get; set; }
        public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected, Suspended
        public string RejectionReason { get; set; }

        public decimal TodayEarnings { get; set; }
        public int TotalTrips { get; set; }
        public decimal AverageRating { get; set; }
        public decimal TotalCo2SavedKg { get; set; }

        public bool HasUnreadNotifications { get; set; }

        public List<RecentTrip> RecentTrips { get; set; } = new();
    }

    public class RecentTrip
    {
        public string PickupArea { get; set; }
        public string DropArea { get; set; }
        public decimal Fare { get; set; }
        public string Status { get; set; } // Completed, Cancelled
        public DateTime CompletedAt { get; set; }
    }
    public class StatusUpdateResult : BaseEntity
    {
    }
}
