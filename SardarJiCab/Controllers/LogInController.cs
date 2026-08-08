using CabBookingMVC.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class LogInController : Controller
    {

        private readonly ILogInService _loginService;
        private readonly IConfiguration _config;

        public LogInController(ILogInService loginService, IConfiguration config)
        {

            _loginService = loginService;
            _config = config;
        }




        public IActionResult Index()
        {

            return View();
        }


        //[HttpPost]
        //public async Task<JsonResult> CustomerLogin(string UserName, string Password ,string Role)
        //{
        //    try
        //    {

        //        if (Role == "Driver")
        //        {
        //            return RedirectToAction("Index", "Home", new { area = "Driver" });
        //        }

        //        if (Role == "Admin")
        //        {
        //            return RedirectToAction("Index", "Home", new { area = "Admin" });
        //        }



        //        bool IsSuccess = false;
        //        HttpContext.Session.Remove("customer");

        //        var customer = new CustomerVM
        //        {
        //            UserName = UserName,
        //            Password = Password,
        //            ClientId = Convert.ToInt64(_config["ClientId"])
        //        };

        //        CustomerVM cus = await _loginService.customerLogin(customer);

        //        if (cus == null)
        //        {
        //            return Json(new
        //            {
        //                IsSuccess = false,
        //                Message = "Invalid username or password"
        //            });
        //        }
        //        if (cus.Id >0)
        //        {
        //            IsSuccess = true;
        //        }
        //        if (cus.Id > 0)
        //        {
        //            HttpContext.Session.SetObject("customer", cus);
        //        }

        //        return Json(new
        //        {
        //            IsSuccess = IsSuccess,
        //            Message = IsSuccess ? "Login successful." : "Login failed.",
        //            Data = cus
        //        });
        //    }
        //    catch (Exception ex)
        //    {


        //        return Json(new
        //        {
        //            IsSuccess = false,
        //            Message = "An unexpected error occurred while processing your request.",
        //            Error = ex.Message 
        //        });
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> CustomerLogin(string UserName, string Password)
        {
            try
            {
                HttpContext.Session.Remove("customer");

                var customer = new CustomerVM
                {
                    UserName = UserName,
                    Password = Password,
                    ClientId = Convert.ToInt64(_config["ClientId"])
                };

                CustomerVM cus = await _loginService.customerLogin(customer);

                if (cus == null || cus.Id <= 0)
                {
                    return Json(new
                    {
                        IsSuccess = false,
                        Message = "Invalid username or password."
                    });
                }

                HttpContext.Session.SetObject("customer", cus);

                string redirectUrl = "";

              
                redirectUrl = "/Home/Index";
                return Json(new
                {
                    IsSuccess = true,
                    Message = "Login Successful",
                    RedirectUrl = redirectUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }


        public IActionResult CustomerLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "LogIn");
        }

    }
    }
