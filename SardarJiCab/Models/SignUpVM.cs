using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SardarJi_Cab_Booking.Models
{
      public class SignUpVM
      {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public long ClientId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid mobile number.")]
        public string MobileNo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class CustomerVM
    {
        
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public decimal CardAmount { get; set; }
        public decimal WalletAmount { get; set; }
        public string IsdCodes { get; set; }
        public string SelectedCurrency { get; set; }
        public string PaymentDetails { get; set; }
        public TravelSummaryViewModel CabsDetails { get; set; }
        public string ConversionRate { get; set; }
        public string temperatured { get; set; }
        public string Title { get; set; }
        public string VisaDetails { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string InsAuthRes { get; set; }
        public string TravelInsuranceDetails { get; set; }
        public string TransferDetails { get; set; }
        public string TransAuthRes { get; set; }
        public string HotelBookingValues { get; set; }
        public string ActivityPreview { get; set; }
        public string FlightBookingDetalsSeries { get; set; }
        public string FlightBookingDetalsSelf { get; set; }
        public string PackageDetailsforBooking { get; set; }
        public string BookingPackageDetails { get; set; }
        public string PackageSideDetails { get; set; }
        public string amount { get; set; }
        public string GiftCardDetails { get; set; }
        public bool IsCard { get; set; }
        public string HotelBookingDetals { get; set; }
        public bool IsWallet { get; set; }
        public string HotelUpdatedPrice { get; set; }
        public bool IsAll { get; set; }
        public string FlightBookingDetals { get; set; }
        public string TBOTokenId { get; set; }
        public string BookingQueueId { get; set; }
        public string FirstName { get; set; }
        public bool IsExists { get; set; }
        public string LogoPath { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public long ClientId { get; set; }
        public string Mobile { get; set; }
        public long CustomerType { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
        public string Dob { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public long Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public bool IsAlreadyNotExists { get; set; }
        public int walletLogin { get; set; }
        //Changes due to ticket Format
        public string CompanyName { get; set; }
        public string GSTNo { get; set; }
    }

    public class ProfileUpdateRequest
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public IFormFile ProfileImageFile { get; set; }
    }
    public class ProfileUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BookingPlaceholder
    {
        public int BookingId { get; set; }
        public string BookingNo { get; set; } = "";
        public string PickupAddress { get; set; } = "";
        public string DropAddress { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string VehicleNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string? CarImage { get; set; }
        public double DropLat { get; set; }
        public double DropLng { get; set; }
    }
    public class TrackRideViewModel
    {
        public int BookingId { get; set; }
        public string BookingNo { get; set; } = "";
        public string PickupAddress { get; set; } = "";
        public string DropAddress { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string VehicleNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string? CarImage { get; set; }
        public double? PickupLatitude { get; set; }
        public double? PickupLongitude { get; set; }
        public double? DropLatitude { get; set; }   // NEW
        public double? DropLongitude { get; set; }  // NEW

        public string RidePhase { get; set; } = "toPickup";
        public double DropLat { get; set; }
        public double DropLng { get; set; }

        public double? Latitude { get; set; }  
        public double? Longitude { get; set; }

        public string GoogleMapsApiKey { get; set; } = "";
    }

    //public class LiveLocation
    //{
    //    public int BookingId { get; set; }
    //    public int DriverId { get; set; }
    //    public string DriverName { get; set; }
    //    public decimal Latitude { get; set; }
    //    public decimal Longitude { get; set; }
    //}
    public class ApplyCouponRequestDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public decimal FareAmount { get; set; }

        public string? RideType { get; set; }
    }

    public class ApplyCouponResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalFareAmount { get; set; }
        public string? CouponCode { get; set; }
    }
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Code { get; set; } = string.Empty;   

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

      
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaxDiscountAmount { get; set; }

      
        [Column(TypeName = "decimal(10,2)")]
        public decimal? MinFareAmount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

      
        public int? TotalUsageLimit { get; set; }
        public int CurrentUsageCount { get; set; } = 0;

       
        public int? UsageLimitPerUser { get; set; } = 1;

      
        [MaxLength(200)]
        public string? ApplicableRideTypes { get; set; }

        public bool IsFirstRideOnly { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum DiscountType
    {
        Percentage = 1,
        FlatAmount = 2
    }
    public class CouponUsage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    }
    public class CouponUpsertDto
    {
        [Required, MaxLength(30)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinFareAmount { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        public int? TotalUsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; } = 1;
        public string? ApplicableRideTypes { get; set; }
        public bool IsFirstRideOnly { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class CouponListItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinFareAmount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? TotalUsageLimit { get; set; }
        public int CurrentUsageCount { get; set; }
        public bool IsActive { get; set; }

        // Ready-to-show text for a <select> option, e.g. "EVFIRST50 — 50% off · min fare ₹100"
        public string DisplayLabel
        {
            get
            {
                var discountText = DiscountType == DiscountType.Percentage
                    ? $"{DiscountValue}% off"
                    : $"₹{DiscountValue} off";

                var parts = new System.Collections.Generic.List<string> { $"{Code} — {discountText}" };
                if (MinFareAmount.HasValue) parts.Add($"min fare ₹{MinFareAmount.Value:0}");
                if (!string.IsNullOrWhiteSpace(Description)) parts.Add(Description!);

                return string.Join(" · ", parts);
            }
        }
    }

    public class AddPageVM
    {
        public long Id { get; set; }
        public string? Header { get; set; }
        public string? SeoUrl { get; set; }
        public string? MetaKeyword { get; set; }
        public string? Overview { get; set; }
        public string? ImagePath { get; set; }
        public string? Title { get; set; }
        public long ClientId { get; set; }

        public List<AddPage_Banner> Banner { get; set; } = new();
        public List<AddPage_Content> Content { get; set; } = new();
        public List<AddPageVM> AddpageContent { get; set; } = new();
    }

    public class AddPage_Banner
    {
        public string? AltTag { get; set; }
        public string? ImageTitle { get; set; }
        public string? ImagePath { get; set; }
    }

    public class AddPage_Content
    {
        public string? ImageName { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageTitle { get; set; }
        public string? BlogContent { get; set; }
    }
    public class AddPageCategory
    {
        public string? AddPage { get; set; }
    }
}
