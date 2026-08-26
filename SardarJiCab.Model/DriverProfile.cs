using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class DriverProfile:BaseEntity
    {
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }

        public string LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string LicensePhotoUrl { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedOn { get; set; }

        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }
        public bool IsElectricVehicle { get; set; }
        public string RcNumber { get; set; }
        public string RcPhotoUrl { get; set; }

        public string ApprovalStatus { get; set; }
        public string RejectionReason { get; set; }

        public decimal AverageRating { get; set; }
        public int TotalTrips { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalCo2SavedKg { get; set; }

        public string Initials =>
            string.IsNullOrWhiteSpace(FullName) ? "D"
            : string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpper(w[0])));

        public string ApprovalStatusCss => (ApprovalStatus ?? "").Trim().ToLowerInvariant();
    }
}