using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;

namespace SardarJi_Cab_Booking.Controllers
{
    public class RegistrationController : Controller
    {


        private readonly IRegistrationService _registrationService;
        private readonly IConfiguration _config;

        public RegistrationController(IRegistrationService registrationService, IConfiguration config)
        {

            _registrationService = registrationService;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UserSignup(SignUpVM model)
        {
            try
            {
                bool IsSuccess = false;
                model.ClientId = Convert.ToInt64(_config["ClientId"]);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();

                    return Json(new
                    {
                        IsSuccess = false,
                        Message = errors.Any() ? string.Join(" ", errors) : "Please fill all required fields correctly."
                    });

                   
                }

                CustomerVM result = await _registrationService.CustomerSignUp(model);

                if (result.Id > 0 && result.Message == "Customer details created successfully..")
                {
                    //SignUpMail mail = new SignUpMail();
                    //mail.SignUp_DesignMailer(model);
                }
                if (result.Id > 0)
                {
                    IsSuccess = true;
                }

                return Json(new
                {
                    IsSuccess = IsSuccess,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    IsSuccess = false,
                    Message = "Something went wrong while processing your registration. Please try again."
                });
            }
        }







    }
}
