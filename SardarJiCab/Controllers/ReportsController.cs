using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Controllers
{
    public class ReportsController : Controller
    {

        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly IBookingService _booking;
        private readonly IInvoiceService _invoiceService;
        private readonly IWebHostEnvironment _env;


        public ReportsController(ICustomerService customerService, IConfiguration config, IBookingService booking, IInvoiceService invoiceService, IWebHostEnvironment env)
        {

            _customerService = customerService;
            _config = config;
            _booking = booking;
            _invoiceService = invoiceService;
            _env = env;
        }



        //public IActionResult Index()
        //{
        //    return View();
        //}


        #region Get Booking List

        [HttpGet]
        public async Task<ActionResult> CabBookingList()
        {
            var customer = HttpContext.Session.GetObject<CustomerVM>("customer");
            TravelSummaryViewModel bookinglist = await _booking.GetBookingList(customer.Id);
            bookinglist.TodayRide = bookinglist.Bookings
    .FirstOrDefault(x =>
        x.JourneyDate.Date == DateTime.Today &&
        x.DriverId > 0);
            bookinglist.UserName=customer.FirstName +" "+customer.LastName;
            return View(bookinglist);
        }
        



        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] BookingFilter filter)
        {
            filter ??= new BookingFilter();
            if (filter.PageSize <= 0) filter.PageSize = 10;
            if (filter.PageNumber <= 0) filter.PageNumber = 1;

            var result = await _booking.GetBookingsAsync(filter);

            ViewBag.Filter = filter;
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetBookingsPartial([FromQuery] BookingFilter filter)
        {
            filter ??= new BookingFilter();
            if (filter.PageSize <= 0) filter.PageSize = 10;
            if (filter.PageNumber <= 0) filter.PageNumber = 1;

            var result = await _booking.GetBookingsAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_BookingTablePartial", result);
        }


        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(long id)
        {
            var booking = await _booking.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound($"Booking #{id} was not found.");

            var pdfBytes = _invoiceService.GenerateInvoicePdf(booking);
            var fileName = $"Invoice_{booking.BookingNo}_{booking.BookingId}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateStatus(long id, string status)
        {
            await _booking.UpdateStatusAsync(id, status);
            return Ok(new { success = true });
        }



        #endregion End Get Booking List

       
        [HttpGet]
        public IActionResult SupportReview()
        {
            return View(new SupportTicketViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicketViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var customer = HttpContext.Session.GetObject<CustomerVM>("customer");

                if (customer == null)
                {
                    return RedirectToAction("Index", "LogIn");
                }

                model.customerId = customer.Id;

              
                if (model.Attachment != null)
                {
                    const long maxBytes = 5 * 1024 * 1024; // 5 MB

                    if (model.Attachment.Length > maxBytes)
                    {
                        ModelState.AddModelError(
                            nameof(model.Attachment),
                            "Attachment must be 5 MB or smaller."
                        );

                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(
                        _env.WebRootPath,
                        "uploads",
                        "support-tickets"
                    );

                    Directory.CreateDirectory(uploadsFolder);

                    var originalFileName = Path.GetFileName(model.Attachment.FileName);

                    var storedFileName = $"{Guid.NewGuid()}_{originalFileName}";

                    var fullPath = Path.Combine(
                        uploadsFolder,
                        storedFileName
                    );

                    await using (var stream = new FileStream(
                        fullPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }

                    model.AttachmentFileName = originalFileName;
                    model.AttachmentPath = $"/uploads/support-tickets/{storedFileName}";
                }

               
                model = await _booking.SaveTicketDetails(model);

                
                await SendSupportTicketConfirmationAsync(customer, model);

                await SendBookingConfirmationToAdminAsync(customer, model);

                TempData["SupportSuccessMessage"] =
                    $"Thanks! Your ticket (#{model.TicketId}) has been submitted. " +
                    "Our team will get back to you shortly.";

                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception ex)
            {
               

                ModelState.AddModelError(
                    string.Empty,
                    "Something went wrong while submitting your ticket. Please try again."
                );

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        private async Task SendSupportTicketConfirmationAsync(CustomerVM customer, SupportTicketViewModel ticket)
        {
            Int64 ClientId = Convert.ToInt64(_config["ClientId"]);
            QuotationEmailSettings settings =
                await _customerService.GetQuotationDetails(ClientId);

            string customerName = string.IsNullOrWhiteSpace(ticket.Name)
                ? "Customer"
                : ticket.Name;

            string phone = string.IsNullOrWhiteSpace(ticket.Phone)
                ? "Not provided"
                : ticket.Phone;

            string referenceNumber = string.IsNullOrWhiteSpace(ticket.ReferenceNumber)
                ? "Not provided"
                : ticket.ReferenceNumber;

            string attachment = string.IsNullOrWhiteSpace(ticket.AttachmentFileName)
                ? "No attachment"
                : ticket.AttachmentFileName;

            string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Support Ticket Confirmation</title>
</head>

<body style='margin:0;padding:0;background:#f4f6f8;
             font-family:Arial,Helvetica,sans-serif;color:#1f2937;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'
       style='background:#f4f6f8;padding:30px 15px;'>

<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0' border='0'
       style='width:100%;max-width:600px;background:#ffffff;
              border-radius:10px;overflow:hidden;'>

    <!-- HEADER -->
    <tr>
        <td style='background:#16a34a;padding:25px 30px;'>

            <table width='100%' cellpadding='0' cellspacing='0'>
                <tr>

                    <td style='color:#ffffff;
                               font-size:20px;
                               font-weight:bold;'>
                        {settings.DisplayName}
                    </td>

                    <td align='right'>
                        <span style='background:#ffffff;
                                     color:#16a34a;
                                     font-size:11px;
                                     font-weight:bold;
                                     padding:7px 12px;
                                     border-radius:20px;'>
                            RECEIVED
                        </span>
                    </td>

                </tr>
            </table>

        </td>
    </tr>

    <!-- BODY -->
    <tr>
        <td style='padding:30px;'>

            <h1 style='margin:0 0 10px 0;
                       font-size:24px;
                       color:#111827;'>
                Support Request Received
            </h1>

            <p style='margin:0 0 20px 0;
                      font-size:14px;
                      line-height:1.6;
                      color:#6b7280;'>
                Dear <strong style='color:#111827;'>{customerName}</strong>,
            </p>

            <p style='margin:0 0 25px 0;
                      font-size:14px;
                      line-height:1.6;
                      color:#6b7280;'>
                Thank you for contacting
                <strong>{settings.DisplayName}</strong>.
                We have received your support request and our team
                will review it shortly.
            </p>

            <!-- TICKET ID -->
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='background:#f0fdf4;
                          border:1px solid #bbf7d0;
                          border-radius:8px;
                          margin-bottom:20px;'>

                <tr>
                    <td style='padding:15px;'>

                        <div style='font-size:11px;
                                    color:#6b7280;
                                    text-transform:uppercase;
                                    font-weight:bold;'>
                            Ticket Number
                        </div>

                        <div style='font-size:20px;
                                    color:#15803d;
                                    font-weight:bold;
                                    margin-top:5px;'>
                            #{ticket.TicketId}
                        </div>

                    </td>
                </tr>

            </table>

            <!-- TICKET DETAILS -->
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='border:1px solid #e5e7eb;
                          border-radius:8px;
                          overflow:hidden;'>

                <tr>
                    <td colspan='2'
                        style='padding:14px 16px;
                               background:#f9fafb;
                               color:#111827;
                               font-size:15px;
                               font-weight:bold;
                               border-bottom:1px solid #e5e7eb;'>
                        Request Details
                    </td>
                </tr>

                <tr>
                    <td width='35%'
                        style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Name
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               font-weight:600;
                               border-bottom:1px solid #f1f1f1;'>
                        {customerName}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Email
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               border-bottom:1px solid #f1f1f1;'>
                        {ticket.Email}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Phone
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               border-bottom:1px solid #f1f1f1;'>
                        {phone}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Category
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               font-weight:600;
                               border-bottom:1px solid #f1f1f1;'>
                        {ticket.Category}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Priority
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               font-weight:600;
                               border-bottom:1px solid #f1f1f1;'>
                        {ticket.Priority}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Subject
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               font-weight:600;
                               border-bottom:1px solid #f1f1f1;'>
                        {ticket.Subject}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;
                               border-bottom:1px solid #f1f1f1;'>
                        Reference Number
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;
                               border-bottom:1px solid #f1f1f1;'>
                        {referenceNumber}
                    </td>
                </tr>

                <tr>
                    <td style='padding:12px 16px;
                               color:#6b7280;
                               font-size:13px;'>
                        Attachment
                    </td>

                    <td style='padding:12px 16px;
                               color:#111827;
                               font-size:14px;'>
                        {attachment}
                    </td>
                </tr>

            </table>

            <!-- MESSAGE -->
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='margin-top:20px;
                          border:1px solid #e5e7eb;
                          border-radius:8px;'>

                <tr>
                    <td style='padding:15px;
                               background:#f9fafb;
                               color:#111827;
                               font-size:14px;
                               font-weight:bold;'>
                        Your Message
                    </td>
                </tr>

                <tr>
                    <td style='padding:15px;
                               color:#4b5563;
                               font-size:14px;
                               line-height:1.7;'>
                        {ticket.Message}
                    </td>
                </tr>

            </table>

            <p style='margin:25px 0 0 0;
                      font-size:14px;
                      line-height:1.6;
                      color:#6b7280;'>

                Please keep your ticket number
                <strong style='color:#111827;'>
                    #{ticket.TicketId}
                </strong>
                for future reference.

            </p>

            <p style='margin:20px 0 0 0;
                      font-size:14px;
                      line-height:1.6;
                      color:#6b7280;'>

                Our support team will get back to you as soon as possible.

            </p>

            <p style='margin:20px 0 0 0;
                      font-size:14px;
                      color:#111827;'>
                Regards,<br>
                <strong>{settings.DisplayName}</strong>
            </p>

        </td>
    </tr>

    <!-- FOOTER -->
    <tr>
        <td align='center'
            style='padding:20px;
                   background:#f9fafb;
                   border-top:1px solid #e5e7eb;
                   color:#9ca3af;
                   font-size:11px;
                   line-height:1.6;'>

            This is an automated support confirmation email.
            <br>
            &copy; {DateTime.Now.Year} {settings.DisplayName}.
            All rights reserved.

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
                Sendto = ticket.Email,
                From = settings.FromEmail,
                Subject = $"Support Ticket Received - #{ticket.TicketId}",
                Body = emailBody,
                DisplayName = settings.DisplayName,
                ClientId = customer.ClientId,
                RecordStatusId = 1,
                Host = settings.Host,
                Port = settings.Port,
                UserId = settings.UserId,
                Password = settings.Password
            };

            new Sendmailstoall().SendMail(query);
        }
        private async Task SendBookingConfirmationToAdminAsync(
     CustomerVM customer,
     SupportTicketViewModel booking)
        {
            Int64 ClientId = Convert.ToInt64(_config["ClientId"]);
            QuotationEmailSettings settings =
                await _customerService.GetQuotationDetails(ClientId);

            string phone = string.IsNullOrWhiteSpace(booking.Phone)
                ? "Not Provided"
                : booking.Phone;

            string referenceNumber = string.IsNullOrWhiteSpace(booking.ReferenceNumber)
                ? "Not Provided"
                : booking.ReferenceNumber;

            string attachment = string.IsNullOrWhiteSpace(booking.AttachmentFileName)
                ? "No Attachment"
                : booking.AttachmentFileName;

            string emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>New Support Ticket</title>
</head>

<body style='margin:0;padding:0;background:#eef1f5;
font-family:Segoe UI,Helvetica,Arial,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0'
style='padding:40px 20px;background:#eef1f5;'>
<tr>
<td align='center'>

<table width='650' cellpadding='0' cellspacing='0'
style='width:100%;max-width:650px;background:#ffffff;
border-radius:10px;overflow:hidden;
box-shadow:0 2px 10px rgba(0,0,0,0.06);'>

<!-- HEADER -->
<tr>
<td style='background:#2563eb;padding:28px 32px;'>

<table width='100%' cellpadding='0' cellspacing='0'>
<tr>

<td style='color:#ffffff;font-size:20px;font-weight:600;'>
    {settings.DisplayName}
</td>

<td align='right'>
<span style='background:#ffffff;
color:#2563eb;
font-size:12px;
font-weight:600;
padding:6px 14px;
border-radius:20px;
letter-spacing:0.5px;'>
    NEW TICKET
</span>
</td>

</tr>
</table>

</td>
</tr>

<!-- BODY -->
<tr>
<td style='padding:32px;'>

<h2 style='margin:0 0 8px 0;
color:#1a1a1a;
font-size:22px;'>
    New Support Ticket Received
</h2>

<p style='margin:0 0 24px 0;
color:#5f6b7a;
font-size:14px;
line-height:1.6;'>

A new support ticket has been submitted.
Please review the details below.

</p>

<!-- TICKET ID -->
<table width='100%' cellpadding='0' cellspacing='0'
style='margin-bottom:20px;
background:#eff6ff;
border:1px solid #bfdbfe;
border-radius:8px;'>

<tr>
<td style='padding:15px 18px;'>

<div style='color:#6b7280;
font-size:11px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.5px;'>
    Ticket ID
</div>

<div style='margin-top:5px;
color:#1d4ed8;
font-size:20px;
font-weight:700;'>
    #{booking.TicketId}
</div>

</td>
</tr>

</table>

<!-- CUSTOMER DETAILS -->
<table width='100%' cellpadding='0' cellspacing='0'
style='border:1px solid #e8ebee;
border-radius:8px;
overflow:hidden;
margin-bottom:20px;'>

<tr style='background:#f8fafb;'>
<td colspan='2'
style='padding:14px 16px;
color:#1a1a1a;
font-size:15px;
font-weight:700;
border-bottom:1px solid #e8ebee;'>
    Customer Information
</td>
</tr>

<tr>
<td width='35%'
style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Name
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;
font-weight:600;
border-bottom:1px solid #f0f0f0;'>
    {booking.Name}
</td>
</tr>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Email
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;
border-bottom:1px solid #f0f0f0;'>
    {booking.Email}
</td>
</tr>

<tr>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Phone
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;
border-bottom:1px solid #f0f0f0;'>
    {phone}
</td>
</tr>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;'>
    Reference Number
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;'>
    {referenceNumber}
</td>
</tr>

</table>

<!-- TICKET DETAILS -->
<table width='100%' cellpadding='0' cellspacing='0'
style='border:1px solid #e8ebee;
border-radius:8px;
overflow:hidden;'>

<tr style='background:#f8fafb;'>
<td colspan='2'
style='padding:14px 16px;
color:#1a1a1a;
font-size:15px;
font-weight:700;
border-bottom:1px solid #e8ebee;'>
    Ticket Details
</td>
</tr>

<tr>
<td width='35%'
style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Category
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;
font-weight:600;
border-bottom:1px solid #f0f0f0;'>
    {booking.Category}
</td>
</tr>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Priority
</td>

<td style='padding:12px 16px;
color:#dc2626;
font-size:14px;
font-weight:700;
border-bottom:1px solid #f0f0f0;'>
    {booking.Priority}
</td>
</tr>

<tr>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;
border-bottom:1px solid #f0f0f0;'>
    Subject
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;
font-weight:600;
border-bottom:1px solid #f0f0f0;'>
    {booking.Subject}
</td>
</tr>

<tr style='background:#f8fafb;'>
<td style='padding:12px 16px;
color:#8792a2;
font-size:12px;
font-weight:600;
text-transform:uppercase;
letter-spacing:0.4px;'>
    Attachment
</td>

<td style='padding:12px 16px;
color:#1a1a1a;
font-size:14px;'>
    {attachment}
</td>
</tr>

</table>

<!-- MESSAGE -->
<table width='100%' cellpadding='0' cellspacing='0'
style='margin-top:20px;
border:1px solid #e8ebee;
border-radius:8px;
overflow:hidden;'>

<tr>
<td style='padding:14px 16px;
background:#f8fafb;
color:#1a1a1a;
font-size:15px;
font-weight:700;
border-bottom:1px solid #e8ebee;'>
    Customer Message
</td>
</tr>

<tr>
<td style='padding:16px;
color:#5f6b7a;
font-size:14px;
line-height:1.7;'>
    {booking.Message}
</td>
</tr>

</table>

</td>
</tr>

<!-- FOOTER -->
<tr>
<td align='center'
style='padding:20px;
background:#f8fafb;
border-top:1px solid #e8ebee;
color:#9aa4b1;
font-size:12px;
line-height:1.6;'>

This is an automated notification from
<strong>{settings.DisplayName}</strong>.

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
                Sendto = "sonu.singh.traviyo@gmail.com",
                From = settings.FromEmail,
                Subject = $"New Support Ticket - #{booking.TicketId}",
                Body = emailBody,
                DisplayName = settings.DisplayName,
                ClientId = customer.ClientId,
                RecordStatusId = 1,
                Host = settings.Host,
                Port = settings.Port,
                UserId = settings.UserId,
                Password = settings.Password
            };

            new Sendmailstoall().SendMail(query);
        }

    }
}
