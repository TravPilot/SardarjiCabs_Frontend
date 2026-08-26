namespace SardarJiCab.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace SardarJiEV.Models
    {
        public class Driver: BaseEntity
        {
            [Required]
            [StringLength(10, MinimumLength = 10)]
            public string MobileNumber { get; set; }

            public string PasswordHash { get; set; }

            [EmailAddress]
            [StringLength(150)]
            public string Email { get; set; }

            [Required]
            [StringLength(120)]
            public string FullName { get; set; }

            public string ProfilePhotoUrl { get; set; }

            [StringLength(20)]
            public string Gender { get; set; }

            public DateTime? DateOfBirth { get; set; }

            [StringLength(300)]
            public string Address { get; set; }

            [StringLength(100)]
            public string City { get; set; }

            [StringLength(10)]
            public string PinCode { get; set; }

            // ---------------- Licensing & Verification ----------------

            [StringLength(30)]
            public string LicenseNumber { get; set; }

            public DateTime? LicenseExpiryDate { get; set; }

            public string LicensePhotoUrl { get; set; }

            [StringLength(20)]
            public string AadhaarNumber { get; set; }
            public string AadhaarPhotoUrl { get; set; }

            public bool IsVerified { get; set; } = false;

            public DateTime? VerifiedOn { get; set; }

            // ---------------- Vehicle ----------------

            [StringLength(20)]
            public string VehicleNumber { get; set; }

            [StringLength(60)]
            public string VehicleModel { get; set; }

            [StringLength(30)]
            public string VehicleType { get; set; } // e.g. Sedan, SUV, Hatchback

            public bool IsElectricVehicle { get; set; } = true;

            [StringLength(30)]
            public string RcNumber { get; set; }

            public string RcPhotoUrl { get; set; }

            // ---------------- Account status ----------------

            public bool IsActive { get; set; } = true;

            public bool IsOnline { get; set; } = false;

            [StringLength(20)]
            public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected, Suspended

            public string RejectionReason { get; set; }

            // ---------------- Ratings & activity ----------------

            [Column(TypeName = "decimal(3,2)")]
            public decimal AverageRating { get; set; } = 0;

            public int TotalTrips { get; set; } = 0;

            [Column(TypeName = "decimal(10,2)")]
            public decimal TotalEarnings { get; set; } = 0;

            [Column(TypeName = "decimal(10,3)")]
            public decimal TotalCo2SavedKg { get; set; } = 0;

            // ---------------- Auth/session bookkeeping ----------------

            public DateTime? LastLoginAt { get; set; }

            [StringLength(45)]
            public string LastLoginIp { get; set; }

            public int FailedLoginAttempts { get; set; } = 0;

            public DateTime? LockedUntil { get; set; }

            // ---------------- Navigation (adjust to your actual related entities) ----------------

            // public ICollection<Booking> Bookings { get; set; }
            // public ICollection<DriverDocument> Documents { get; set; }
            // public ICollection<Payout> Payouts { get; set; }
        }

        public class DriverLoginResult : BaseEntity
        {
            public Driver Driver { get; set; }
        }

        public class OtpRequestResult : BaseEntity
        {
            public OtpRequestResult() { }
        }
    }
}
