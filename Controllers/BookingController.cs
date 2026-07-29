using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IBookingService _booking;
       

        private readonly PaymentGatwaySettings _paymentGatewaySettings;


        public BookingController(ICustomerService customerService, IConfiguration config, PaymentGatwaySettings paymentGatewaySettings, IBookingService booking)
        {
            _customerService = customerService;
            _config = config;
            _paymentGatewaySettings = paymentGatewaySettings;
            _booking = booking;
            
        }


        public async Task<IActionResult> Index()
        {
           
            RazorPayVM razorPayVM = await _paymentGatewaySettings.CheckCapturePaymentGateway();
            TravelSummaryViewModel details = HttpContext.Session.GetObject<TravelSummaryViewModel>("Cardetails");
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            details.ClientId = Convert.ToInt64(_config["ClientId"]);
            details.UserId = customer.Id;
            var booking =await _booking.SaveBookingDetails(details);
            if (details == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(details);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentsDetails(string selectPayment, string currency, string temperatured, string conversionRate)
        {
            bool isCard = false;
            bool isWallet = false;

            TravelSummaryViewModel details = HttpContext.Session.GetObject<TravelSummaryViewModel>("Cardetails");
            var customerdetails = HttpContext.Session.GetObject<CustomerVM>("customer");
            CustomerVM customer = new CustomerVM();
            customer.CabsDetails = details;
            var customerJson = HttpContext.Session.GetString("customer");
            if (!string.IsNullOrEmpty(customerJson))
            {
                customer = JsonConvert.DeserializeObject<CustomerVM>(customerJson);
                customer.SelectedCurrency = currency;
                customer.temperatured = temperatured;
                customer.ConversionRate = conversionRate;

                HttpContext.Session.SetString("customer",
                    JsonConvert.SerializeObject(customer));
            }

            decimal totalAmount;
            decimal razorpayAmount;

            if (customer != null && customer.CustomerType == 2)
            {
                totalAmount = Math.Round(details.Cost);
                razorpayAmount = Math.Round(details.Cost);
            }
            else
            {
                totalAmount = Math.Round(details.Cost);
                razorpayAmount = Math.Round(details.Cost);
            }

            decimal walletAmount = 0;

            Random random = new Random();
            string transactionId = random.Next(10000000, 100000000).ToString();

            long clientId = Convert.ToInt64(_config["ClientId"]);

            if (selectPayment == "Card" || string.IsNullOrEmpty(selectPayment))
            {
                isCard = true;
            }
            else
            {
                if (customer != null)
                {
                    WalletVM walletDetails = await _customerService.GetWalletDetails(customer.Id);

                    if (walletDetails != null)
                    {
                        if (totalAmount > walletDetails.AvailBalance)
                        {
                            isCard = true;
                            isWallet = true;

                            totalAmount -= walletDetails.AvailBalance;
                            walletAmount = walletDetails.AvailBalance;
                            razorpayAmount = totalAmount;
                        }
                        else
                        {
                            isWallet = true;
                            walletAmount = totalAmount;
                            razorpayAmount = 0;
                        }
                    }
                }
            }

            details.RazorpayAmount = Math.Abs(razorpayAmount);
            details.walletamount = Math.Abs(walletAmount);
            details.IsCard = isCard;
            details.IsWallet = isWallet;

            string orderId = $"{DateTime.Now:yyyyMMddHHmmssfff}{clientId}";

            HttpContext.Session.SetString("IsCard", isCard.ToString());
            HttpContext.Session.SetString("IsWallet", isWallet.ToString());
            HttpContext.Session.SetString("BookingflightDetails",
                JsonConvert.SerializeObject(details));

            HttpContext.Session.SetString("LocalOrderId", orderId);
            HttpContext.Session.SetString("Localamount", totalAmount.ToString());

            OrderModel order = new OrderModel();

            if (isCard)
            {
                string imagePath = _config["ImagePath"];


                ProfileSettings profileSettings = await _customerService.GetProfileSettings(clientId);
                customer.FlightBookingDetals = JsonConvert.SerializeObject(details);
                customer.InsAuthRes = JsonConvert.SerializeObject(customerdetails);



                customer.BookingQueueId =
                    TempData["BookingQueueId"]?.ToString();
                customer.amount = totalAmount.ToString();

                order = await _paymentGatewaySettings.CheckPaymentGateway(Convert.ToDouble(totalAmount), orderId, isCard, isWallet, JsonConvert.SerializeObject(customer), profileSettings, "Cabbooking");


            }

            return Json(new
            {
                order,
                IsSuccess = true,
                IsWallet = isWallet,
                IsCard = isCard
            });
        }
    }
}
