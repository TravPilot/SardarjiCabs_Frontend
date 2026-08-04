using Newtonsoft.Json;

namespace SardarJi_Cab_Booking.Models
{
    public class CustomerProfile
    {
        public List<NameValueList> Countries { get; set; }
        public string Body { get; set; }
        public List<NameValueList> State { get; set; }
        public List<NameValueList> Cities { get; set; }
        public string IsdCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public long Id { get; set; }
        public string IsdCodes { get; set; }
        public string Email { get; set; }
        public string OldDetails { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public string PinCode { get; set; }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public long ClientId { get; set; }

        public string OldPassword { get; set; }
        public string WebsiteUrl { get; set; }

        public string CompanyName { get; set; }
        public string GstNo { get; set; }
        public string PanNo { get; set; }
        public string LogoPath { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyMobile { get; set; }
        //public string Title { get; set; }
    }
    public class NameValueList
    {
        public long Id { get; set; }
        public long SelectRoomId { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long DestinationTypeId { get; set; }
        public string DestinationType { get; set; }
        public string Destination { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string ImagePath { get; set; }

        public string CountryCode
        {
            get; set;
        }

        public long RoomTypId { get; set; }
        public long NewRoomTypId { get; set; }

        public decimal Price { get; set; }
        public int Dicount { get; set; }
        public string SeoUrl { get; set; }
        public string City { get; set; }

        public List<NameValueList> FeaturedHotels { get; set; }
        public List<NameValueList> RoomPrice { get; set; }
        public Int64 RoomId { get; set; }
        public string Description { get; set; }
        public string RoomType { get; set; }
        public Int64 RoomTypeId { get; set; }
        public string RoomImage { get; set; }
        public double BasePrice { get; set; }
        public string Decription { get; set; }
        public decimal HotelPrice { get; set; }
        public Int32 MaxAdultsAllowed { get; set; }
        public Int32 MaxChildsAllowed { get; set; }
        public Int32 BaseAdultsAllowed { get; set; }
        public Int32 BaseChildsAllowed { get; set; }
        public Int64 MaxAdult { get; set; }
        public Int64 MaxChild { get; set; }
        public Int64 BaseAdult { get; set; }
        public Int64 BaseChild { get; set; }
        public double PerAdultExtraPrice { get; set; }
        public double PerChildExtraPrice { get; set; }
        public string CurrencyIcon { get; set; }
        public Nullable<DateTime> AvailableFromDate { get; set; }
        public Nullable<DateTime> AvailableToDate { get; set; }
        public string HotelName { get; set; }
        public string RoomBannerImage { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }

        public ulong Pincode { get; set; }
        public string MealPlan { get; set; }
        public string FromDate { get; set; }
        public decimal SingleOccupancy { get; set; }
        public decimal DoubleOccupancy { get; set; }
        public string ToDate { get; set; }

        public long CityId { get; set; }

    }
    public class StatesVM
    {
        public long Id { get; set; }
        public string SName { get; set; }
    }

    public class CitiesVM
    {
        public long Id { get; set; }
        public string CTName { get; set; }

        public long CityId { get; set; }
        public string CityName { get; set; }

        public string DestinationId { get; set; }
        public string StateProvince { get; set; }

    }

    public class QueryVM
    {
        public string Purpose { get; set; }
        public int Stay { get; set; }
        public string MidName { get; set; }
        public string IssueCountry { get; set; }
        public DateTime Timedetail { get; set; }
        public string Nights { get; set; }
        public string HotelType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string Mobile { get; set; }
        public string Text { get; set; }
        public string SelectedValue { get; set; }
        public string ActivityName { get; set; }
        public string Image { get; set; }

        public string options { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public string DepartureDate { get; set; }
        public string Adult { get; set; }
        public string Child { get; set; }
        public string Infant { get; set; }
        public string AirlineCode { get; set; }
        public string ReturnDate { get; set; }
        public string PreferredTime { get; set; }
        public string ExpectedFare { get; set; }
        public string Comments { get; set; }
        public string MobileNo { get; set; }
        public String Captcha { get; set; }


        public long QueryType { get; set; }
        public long CreatedBy { get; set; }

        public string TimeLineDescription { get; set; }
        public Int64 Id { get; set; }
        public Int64 VendorServiceId { get; set; }
        public Int64 ClientId { get; set; }
        public String FirstName { get; set; }
        public String FullName { get; set; }
        public String LastName { get; set; }

        public String Phone { get; set; }
        public String Nationality { get; set; }
        public String VisaType { get; set; }
        public String Startlocation { get; set; }
        public String Endlocation { get; set; }
        public String CorporateType { get; set; }
        public String Railclass { get; set; }
        public Int32 Adults { get; set; }
        public string NoOfTTravellers { get; set; }
        public string NoOfAdults { get; set; }
        public string NoOfChilds { get; set; }
        public Int32 TypeOfService { get; set; }
        public string HotelName { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public Int32 NumberOfChildrens { get; set; }
        public Int32 AgeOfChildren { get; set; }
        public Int32 NoOfChildrenTwelveAbove { get; set; }

        public Int32 Duration { get; set; }
        public String Destination { get; set; }
        public String Origin { get; set; }
        public String DepartureAirport { get; set; }
        public String Url { get; set; }
        public string EmailSignature { get; set; }
        public bool IsSuccess { get; set; }
        private string _depatureDate { get; set; }
        public string ServiceName { get; set; }
        public string Card { get; set; }
        public string Cvv { get; set; }
        public string CExDate { get; set; }
        public string Price { get; set; }
        [JsonProperty("success")]
        public bool Success
        {
            get;
            set;
        }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage
        {
            get;
            set;
        }


        public String DepatureDate
        {
            get { return _depatureDate; }
            set
            {
                if (value != null && value.Trim() != string.Empty)
                {
                    try
                    {
                        _depatureDate = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        _depatureDate = value;
                    }
                }
            }
        }
        //   public String DepatureDate { get; set; }
        private string _arrivalDate { get; set; }
        public String ArrivalDate
        {
            get { return _arrivalDate; }
            set
            {
                if (value != null && value.Trim() != string.Empty)
                {
                    try
                    {
                        _arrivalDate = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        _arrivalDate = value;
                    }
                }
            }
        }
        //public String ArrivalDate { get; set; }
        public String Message { get; set; }
        public String Commodity { get; set; }
        public String Weight { get; set; }
        public String PackageId { get; set; }
        public String PackageName { get; set; }
        public double ExpectedBudget { get; set; }
        public string CcTo { get; set; }
        public string Sendto { get; set; }
        public string From { get; set; }
        public string CabinClass { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string DisplayName { get; set; }
        public long RecordStatusId { get; set; }
        public string PdfPath { get; set; }
        public string FilePath { get; set; }
        public int BranchId { get; set; }
        public string Comment { get; set; }
        public string HotelCategory { get; set; }
        public string MealPlan { get; set; }

        public string IFCCode { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
        public string CityId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public int MDLType { get; set; }

        public string Host { get; set; }
        public string Port { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }

    }


    public class QuotationEmailSettings
    {

        public String DisplayName { get; set; }
        public String FromEmail { get; set; }
        public String CCEmail { get; set; }
        public String EmailSubject { get; set; }
        public String EmailBody { get; set; }
        public String EmailSignature { get; set; }
        public String Notes { get; set; }
        public Boolean IsAutoEmail { get; set; }

        public Int32 RecordStatusId { get; set; }
        public long CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public long UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string Logo { get; set; }

        public string Host { get; set; }
        public string Port { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class LiveLocation
    {

        public int Latitude { get; set; }
        public int longitute { get; set; }
        public int bookingid { get; set; }
        public decimal driverId { get; set; }
        public string DriverName { get; set; }
        
    }
}

