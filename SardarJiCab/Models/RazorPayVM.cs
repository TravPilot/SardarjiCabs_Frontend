namespace SardarJi_Cab_Booking.Models
{
    public class BaseURL
    {
        public string SuccessURl { get; set; }
        public string FailURl { get; set; }
        public string RedirectURL { get; set; }

    }
    public class SeriesAPISettings
    {
        public string Environment { get; set; }
        public string SeriesAPI { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string APIKey { get; set; }
        public long SeriesAPIId { get; set; }


    }
    public class RazorPayVM
    {
        public bool IsSuccess { get; set; }
        public bool IsCard { get; set; }
        public bool IsWallet { get; set; }
        public string OrderId { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public decimal Amount { get; set; }
        public string CustomerSession { get; set; }
        public bool PaymentStatus { get; set; }
        public string Amt { get; set; }
    }

    public class formtorequest
    {
        public string appId { get; set; }
        public string orderId { get; set; }
        public string orderAmount { get; set; }
        public string orderCurrency { get; set; }
        public string orderNote { get; set; }
        public string customerName { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
    }
    public class PaymentGatewaySettings
    {
        public string Environment { get; set; }
        public string Paymentgateway { get; set; }
        public string Merchantkey { get; set; }
        public string MerchantId { get; set; }
        public string MerchantCode { get; set; }
        public long PGMasterId { get; set; }


    }
    public class ProfileSettings
    {
        public string GSTNo { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }

        public long GstAmount { get; set; }
        public bool AutoRefund { get; set; }
        public bool IsGstChecked { get; set; }
        public string LeadPrefix { get; set; }


    }
    public class OrderModel
    {
        public string strformCashfree { get; set; }
        public string strformPayU { get; set; }
        public OrderModel orderModel { get; set; }
        public string companyname { get; set; }
        public string logo { get; set; }
        public string orderId { get; set; }
        public string razorpayKey { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactNumber { get; set; }
        public string address { get; set; }
        public string description { get; set; }
        public string BookingId { get; set; }
        public string URI { get; set; }
    }
    public class WalletVM
    {
        public string DepositType { get; set; }
        public decimal Amount { get; set; }
        public List<Unpaid> unpaids { get; set; }
        public string BankName { get; set; }
        public string UTRNo { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        //public HttpPostedFileBase ImageUpload { get; set; }
        public string ImageFilePath { get; set; }

        public decimal AvailableCredit { get; set; }
        public decimal AvailBalance { get; set; }
        public decimal ApprovalBalance { get; set; }
    }
    public class Unpaid
    {

        public string InvoicePdf { get; set; }
        public string CreatedOn { get; set; }
        public string TicketNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string SectorTo { get; set; }
        public double BookingAmount { get; set; }
    }
}
