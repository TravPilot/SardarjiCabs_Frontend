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


        public ReportsController(ICustomerService customerService, IConfiguration config, IBookingService booking, IInvoiceService invoiceService)
        {

            _customerService = customerService;
            _config = config;
            _booking = booking;
            _invoiceService = invoiceService;
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



    }
}
