using System.ComponentModel.DataAnnotations;

namespace SardarJi_Cab_Booking.Models
{
    public class TravelSummaryViewModel
    {
        public string RideCode { get; set; }
        public long? BookingId { get; set; }          
        public string BookingNo { get; set; }
        public long ClientId { get; set; }
        public long UserId { get; set; }
        public string BookingOtp { get; set; }
        public string Pickup { get; set; }
        public string Drop { get; set; }
        public double DistanceKm { get; set; }
        public string RideDate { get; set; }  
        public string RideTime { get; set; }  
        public int CarId { get; set; }
        public string CarName { get; set; }
        public decimal Cost { get; set; }
        public string UserName { get; set; }
        public string InvoicePdfUrl { get; set; }
        public string CarImage { get; set; }
        public string CarModelName { get; set; }
        public string FuelType { get; set; }
        public string Color { get; set; }
        public decimal NetTotalAmount { get; set; }
        public string? RegistrationNo { get; set; }
        public string PassengerName { get; set; }
        public string PassengerContact { get; set; }
        public decimal StateCharges { get; set; }

        public decimal BasePrice => Cost;
        public decimal Discount { get; set; }
        public decimal TollCharges { get; set; }
        public decimal Total => BasePrice - Discount + TollCharges;

        public string PaymentMethod { get; set; } = "Pay Now";
        public string BarcodeCaption { get; set; }


        public decimal walletamount { get; set; }
        public bool IsCard { get; set; }
        public bool IsWallet { get; set; }
        public decimal RazorpayAmount { get; set; }

        public List<BookingListItem> Bookings { get; set; } = new();
        public BookingListItem TodayRide { get; set; }
        public BookingListItem PendingRide { get; set; }

        public int TotalTrips => Bookings.Count;
        public decimal TotalSpent => Bookings.Sum(b => b.NetPayable);
        public decimal TotalDistanceKm => Bookings.Sum(b => b.TotalDistanceKm ?? 0);
        public DateTime? LastTripDate => Bookings.OrderByDescending(b => b.JourneyDate)
                                                  .Select(b => (DateTime?)b.JourneyDate)
                                                  .FirstOrDefault();
        public int CompletedCount => Bookings.Count(b => b.Status == "Completed");
        public int CancelledCount => Bookings.Count(b => b.Status == "Cancelled");



    }


    public class BookingListItem
    {
        public string DriverName { get; set; }
        public string OTP { get; set; }
        public int DriverId { get; set; }
        public long NewBookingId { get; set; }
        public long? BookingId { get; set; }
        public string BookingNo { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public string CabRoute { get; set; }
        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
        public DateTime JourneyDate { get; set; }
        public decimal? TotalDistanceKm { get; set; }
        public string PassengerName { get; set; }
        public string ContactNumber { get; set; }
        public int? CarId { get; set; }
        public string VehicleName { get; set; }
        public string VehicleModelYear { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleFuelType { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalFare { get; set; }
        public decimal NetPayable { get; set; }
        public string BarcodeInfo { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public string VehicleNumber { get; set; }
        public string CarImage { get; set; }
        public DateTime JourneyTime { get; set; }

        public string StatusBadgeClass => Status switch
        {
            "Confirmed" => "badge-confirmed",
            "Completed" => "badge-completed",
            "Cancelled" => "badge-cancelled",
            "Ongoing" => "badge-ongoing",
            _ => "badge-secondary"
        };
    }

    public class BookingDetails
    {
        public long? BookingId { get; set; }
        public string RideCode { get; set; }       
        public int ClientId { get; set; }
        public int UserId { get; set; }

        public string Pickup { get; set; }
        public string Drop { get; set; }
        public DateTime RideDate { get; set; }
        public decimal? DistanceKm { get; set; }

        public string PassengerName { get; set; }
        public string PassengerContact { get; set; }

        public int? CarId { get; set; }
        public string CarName { get; set; }
        public string CarModelName { get; set; }
        public string Color { get; set; }
        public string FuelType { get; set; }

        public string PaymentMethod { get; set; }
        public decimal Cost { get; set; }
        public string BarcodeCaption { get; set; }
    }

    public class BookingFilter
    {
        public int? ClientId { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; } = "All";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchText { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }


    public enum TicketCategory
    {
        [Display(Name = "Billing")]
        Billing,
        [Display(Name = "Technical Issue")]
        Technical,
        [Display(Name = "Account")]
        Account,
        [Display(Name = "General Question")]
        General,
        [Display(Name = "Other")]
        Other
    }

    public enum TicketPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }

    public class SupportTicketViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone (optional)")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public TicketCategory Category { get; set; }

        [Display(Name = "Priority")]
        public TicketPriority Priority { get; set; } = TicketPriority.Normal;

        [Required(ErrorMessage = "Please enter a subject.")]
        [StringLength(150, ErrorMessage = "Subject must be under 150 characters.")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please describe your issue.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Please provide at least 20 characters describing the issue.")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Order / Reference Number (optional)")]
        public string? ReferenceNumber { get; set; }

        [Display(Name = "Attachment (optional)")]
        public IFormFile? Attachment { get; set; }


        public string AttachmentPath { get; set; } = string.Empty;

        public string AttachmentFileName { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public long customerId { get; set; }
    }
}
