using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SardarJi_Cab_Booking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IBookingService _booking;
        private readonly IWebHostEnvironment _env;

        private readonly PaymentGatwaySettings _paymentGatewaySettings;


        public BookingController(ICustomerService customerService, IConfiguration config, PaymentGatwaySettings paymentGatewaySettings, IBookingService booking, IWebHostEnvironment env)
        {
            _customerService = customerService;
            _config = config;
            _paymentGatewaySettings = paymentGatewaySettings;
            _booking = booking;
            _env = env;

        }


        public async Task<IActionResult> Index()
        {
           long clientid= Convert.ToInt64(_config["ClientId"]);
            string otp = GenerateOtp(6);
            RazorPayVM razorPayVM = await _paymentGatewaySettings.CheckCapturePaymentGateway();
            TravelSummaryViewModel details = HttpContext.Session.GetObject<TravelSummaryViewModel>("Cardetails");
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            details.ClientId = clientid;
            details.UserId = customer.Id;
            details.BookingOtp = otp;
            var booking =await _booking.SaveBookingDetails(details);
            if (details == null)
            {
                return RedirectToAction("Index", "Home");
            }
            #region Booking OTP

           await SendOtpEmailAsync(clientid, customer, otp);
            await SendBookingConfirmationToCustomerAsync(clientid, customer, details);
            await SendBookingConfirmationToAdminAsync(clientid, customer, details);


            #endregion  Booking OTP
            details.InvoicePdfUrl = await GenerateInvoicePdfUrl(details);



            return View(details);
        }

<<<<<<< HEAD
        private async Task<string> GenerateInvoicePdfUrl(TravelSummaryViewModel model)
        {
            var pdfResult = new Rotativa.AspNetCore.ViewAsPdf("InvoicePdf", model)
            {
                FileName = $"Invoice_{model.RideCode}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };

            byte[] pdfBytes = await pdfResult.BuildFile(ControllerContext);

            string folderPath = Path.Combine(_env.WebRootPath, "invoices");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

           
            string fileName = $"Invoice_{model.RideCode}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string filePath = Path.Combine(folderPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            
            string invoiceUrl = $"{Request.Scheme}://{Request.Host}/invoices/{fileName}";
            return invoiceUrl;
        }

        public async Task<IActionResult> CurrentRide()
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");

            if (customer == null)
            {
                return RedirectToAction("Index", "LogIn");
            }
            TravelSummaryViewModel bookinglist = await _booking.GetBookingList(customer.Id);
            bookinglist.TodayRide = bookinglist.Bookings
    .FirstOrDefault(x =>
        x.JourneyDate.Date == DateTime.Today &&
        x.DriverId > 0 && x.Status != "Completed");
            bookinglist.PendingRide = bookinglist.Bookings
    .FirstOrDefault(x =>
        x.JourneyDate.Date == DateTime.Today &&
        x.DriverId <= 0 && x.Status != "Completed");
            bookinglist.UserName = customer.FirstName + " " + customer.LastName;
            return View(bookinglist);
        }

=======
>>>>>>> parent of b370c3a (commit chage)
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

        private static string GenerateOtp(int length)
        {
            var random = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            random.GetBytes(bytes);
            int value = Math.Abs(BitConverter.ToInt32(bytes, 0));
            string otp = (value % (int)Math.Pow(10, length)).ToString().PadLeft(length, '0');
            return otp;
        }

        #region Email Notification Template 

        private async Task SendOtpEmailAsync(long clientId, CustomerVM customer, string otp)
        {
            QuotationEmailSettings quotationemailsettings = await _customerService.GetQuotationDetails(clientId);

            string emailBody = $@"
<html>
<body style='margin:0; padding:0; background-color:#f4f6f8; font-family: Arial, Helvetica, sans-serif;'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f8; padding:40px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='480' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.08);'>
                    
                    <tr>
                        <td style='background-color:#2E86C1; padding:24px 32px;'>
                            <h1 style='margin:0; color:#ffffff; font-size:20px; font-weight:600;'>
                                {quotationemailsettings.DisplayName}
                            </h1>
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:32px;'>
                            <p style='margin:0 0 8px 0; font-size:15px; color:#333333;'>
                                Dear {customer.Name},
                            </p>

                            <p style='margin:0 0 24px 0; font-size:15px; color:#555555; line-height:1.5;'>
                                Please use the One-Time Password (OTP) below to confirm your ride booking.
                                This code is valid for a limited time.
                            </p>

                            <table role='presentation' width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td align='center' style='background-color:#F4F8FB; border:1px dashed #2E86C1; border-radius:6px; padding:20px;'>
                                        <span style='font-size:32px; font-weight:bold; letter-spacing:8px; color:#2E86C1;'>
                                            {otp}
                                        </span>
                                    </td>
                                </tr>
                            </table>

                            <p style='margin:24px 0 0 0; font-size:13px; color:#888888; line-height:1.5;'>
                                For your security, please do not share this OTP with anyone, including our staff.
                                If you did not request this, you can safely ignore this email.
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='background-color:#f9f9f9; padding:20px 32px; text-align:center; border-top:1px solid #eeeeee;'>
                            <p style='margin:0; font-size:12px; color:#999999;'>
                                Thanks,<br/>
                                <strong style='color:#555555;'>
                                    {quotationemailsettings.DisplayName}
                                </strong>
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

            QueryVM query = new QueryVM
            {
                Sendto = customer.Email,
                From = quotationemailsettings.FromEmail,
                Subject = "Your OTP for Ride Confirmation",
                Body = emailBody,
                DisplayName = quotationemailsettings.DisplayName,
                ClientId = clientId,
                RecordStatusId = 1,
                Host = quotationemailsettings.Host,
                Port = quotationemailsettings.Port,
                UserId = quotationemailsettings.UserId,
                Password = quotationemailsettings.Password,
                FilePath = null
            };

            Sendmailstoall sendmailstoall = new Sendmailstoall();
            sendmailstoall.SendMail(query);
        }

        private async Task SendBookingConfirmationToCustomerAsync(long clientId, CustomerVM customer, TravelSummaryViewModel booking)
        {
            QuotationEmailSettings settings = await _customerService.GetQuotationDetails(clientId);

            string emailBody = $@"
<html>
<body style='margin:0;padding:0;background:#eef1f5;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 20px;'>
<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,0.06);'>

<!-- Header -->
<tr>
<td style='background:linear-gradient(135deg,#1fae5c,#0f8f47);padding:28px 32px;'>
<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td style='color:#ffffff;font-size:20px;font-weight:600;'>{settings.DisplayName}</td>
<td align='right'>
<span style='background:rgba(255,255,255,0.2);color:#ffffff;font-size:12px;font-weight:600;padding:6px 14px;border-radius:20px;letter-spacing:0.5px;'>CONFIRMED</span>
</td>
</tr>
</table>
</td>
</tr>

<!-- Body -->
<tr>
<td style='padding:32px;'>

<h2 style='margin:0 0 8px 0;color:#1a1a1a;font-size:22px;'>Your ride is booked!</h2>
<p style='margin:0 0 24px 0;color:#5f6b7a;font-size:14px;line-height:1.6;'>
Dear {customer.FirstName +" "+customer.LastName}, thank you for booking with us. Here are your trip details:
</p>

<table width='100%' cellpadding='0' cellspacing='0'
style='border:1px solid #e8ebee;border-radius:8px;overflow:hidden;'>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;width:40%;'>Booking ID</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;font-weight:600;'>#{booking.BookingId}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Pickup</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.Pickup}</td>
</tr>
<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Drop</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.Drop}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Date &amp; Time</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.RideDate} &nbsp;•&nbsp; {booking.RideTime}</td>
</tr>
<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Vehicle</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.CarName}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Total Fare</td>
<td style='padding:12px 16px;color:#1fae5c;font-size:16px;font-weight:700;'>{booking.Cost}</td>
</tr>

</table>

<p style='margin:28px 0 0 0;color:#5f6b7a;font-size:14px;line-height:1.6;'>
Thank you for choosing <b style='color:#1a1a1a;'>{settings.DisplayName}</b>. We look forward to serving you.
</p>

</td>
</tr>

<!-- Footer -->
<tr>
<td align='center' style='padding:20px;background:#f8fafb;border-top:1px solid #e8ebee;color:#9aa4b1;font-size:12px;'>
&copy; {DateTime.Now.Year} {settings.DisplayName}. All rights reserved.
</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";

            QueryVM query = new QueryVM
            {
                Sendto = customer.Email,
                From = settings.FromEmail,
                Subject = "Booking Confirmation",
                Body = emailBody,
                DisplayName = settings.DisplayName,
                ClientId = clientId,
                RecordStatusId = 1,
                Host = settings.Host,
                Port = settings.Port,
                UserId = settings.UserId,
                Password = settings.Password
            };

            new Sendmailstoall().SendMail(query);
        }

        private async Task SendBookingConfirmationToAdminAsync(long clientId, CustomerVM customer, TravelSummaryViewModel booking)
        {
            QuotationEmailSettings settings = await _customerService.GetQuotationDetails(clientId);

            string emailBody = $@"
<html>
<body style='margin:0;padding:0;background:#eef1f5;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 20px;'>
<tr>
<td align='center'>

<table width='650' cellpadding='0' cellspacing='0'
style='background:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,0.06);'>

<!-- Header -->
<tr>
<td style='background:linear-gradient(135deg,#2563eb,#1d4ed8);padding:28px 32px;'>
<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td style='color:#ffffff;font-size:20px;font-weight:600;'>{settings.DisplayName}</td>
<td align='right'>
<span style='background:rgba(255,255,255,0.2);color:#ffffff;font-size:12px;font-weight:600;padding:6px 14px;border-radius:20px;letter-spacing:0.5px;'>NEW BOOKING</span>
</td>
</tr>
</table>
</td>
</tr>

<!-- Body -->
<tr>
<td style='padding:32px;'>

<h2 style='margin:0 0 8px 0;color:#1a1a1a;font-size:20px;'>A new booking has been received</h2>
<p style='margin:0 0 24px 0;color:#5f6b7a;font-size:14px;line-height:1.6;'>
Booking ID <b>#{booking.BookingId}</b> was just created. Details below.
</p>

<table width='100%' cellpadding='0' cellspacing='0'
style='border:1px solid #e8ebee;border-radius:8px;overflow:hidden;'>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;width:40%;'>Customer Name</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;font-weight:600;'>{customer.FirstName + " " + customer.LastName}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Email</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{customer.Email}</td>
</tr>
<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Mobile</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{customer.Mobile}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Pickup</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.Pickup}</td>
</tr>
<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Drop</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.Drop}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Journey Date &amp; Time</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.RideDate} &nbsp;•&nbsp; {booking.RideTime}</td>
</tr>
<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Vehicle</td>
<td style='padding:12px 16px;color:#1a1a1a;font-size:14px;'>{booking.CarName}</td>
</tr>
<tr>
<td style='padding:12px 16px;color:#8792a2;font-size:12px;font-weight:600;text-transform:uppercase;letter-spacing:0.4px;'>Total Fare</td>
<td style='padding:12px 16px;color:#2563eb;font-size:16px;font-weight:700;'>{booking.Cost}</td>
</tr>

</table>

</td>
</tr>

<!-- Footer -->
<tr>
<td align='center' style='padding:20px;background:#f8fafb;border-top:1px solid #e8ebee;color:#9aa4b1;font-size:12px;'>
This is an automated notification from {settings.DisplayName}.
</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";

            QueryVM query = new QueryVM
            {
                Sendto = "sonu.singh.traviyo@gmail.com" /*settings.FromEmail*/,
                From = settings.FromEmail,
                Subject = "New Booking Received",
                Body = emailBody,
                DisplayName = settings.DisplayName,
                ClientId = clientId,
                RecordStatusId = 1,
                Host = settings.Host,
                Port = settings.Port,
                UserId = settings.UserId,
                Password = settings.Password
            };

            new Sendmailstoall().SendMail(query);
        }

        #endregion Email Notification Template 
    }

}
