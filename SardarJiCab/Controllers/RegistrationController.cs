using Microsoft.AspNetCore.Mvc;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Helper;
using SardarJi_Cab_Booking.Models;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SardarJi_Cab_Booking.Controllers
{
    public class RegistrationController : Controller
    {


        private readonly IRegistrationService _registrationService;
        private readonly IConfiguration _config;
        private readonly ICustomerService _customerService;

        public RegistrationController(IRegistrationService registrationService, IConfiguration config, ICustomerService customerService)
        {

            _registrationService = registrationService;
            _config = config;
            _customerService = customerService;
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
                    IsSuccess = true;
                    long ClientId = (Convert.ToInt64(_config["ClientId"]));

                   
                    await SendRegEmailAsync(model.ClientId, result);
                    //SignUpMail mail = new SignUpMail();
                    //mail.SignUp_DesignMailer(model);
                }
                else
                {
                    IsSuccess = false;
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


        private async Task SendRegEmailAsync(long clientId, CustomerVM customer)
        {
            QuotationEmailSettings quotationemailsettings = await _customerService.GetQuotationDetails(clientId);

            string emailBody = $@"
<html>
<body style='margin:0; padding:0; background-color:#f4f6f8; font-family: Arial, Helvetica, sans-serif;'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f6f8; padding:40px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='480' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 16px rgba(0,0,0,0.08);'>

                    <!-- Header with gradient -->
                    <tr>
                        <td style='background:linear-gradient(135deg, #2E86C1 0%, #1B4F72 100%); padding:36px 32px; text-align:center;'>
                            <div style='font-size:36px; margin-bottom:8px;'>🎉</div>
                            <h1 style='margin:0; color:#ffffff; font-size:22px; font-weight:700;'>
                                Welcome to {quotationemailsettings.DisplayName}!
                            </h1>
                            <p style='margin:6px 0 0 0; color:#d6eaf8; font-size:14px;'>
                                We're excited to have you on board
                            </p>
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style='padding:36px 32px;'>
                            <p style='margin:0 0 8px 0; font-size:16px; color:#222222;'>
                                Hi {customer.Name}, 👋
                            </p>

                            <p style='margin:0 0 16px 0; font-size:15px; color:#555555; line-height:1.6;'>
                                Thanks for signing up! Your account has been successfully created and
                                you're all set to get started.
                            </p>

                            <p style='margin:0; font-size:15px; color:#555555; line-height:1.6;'>
                                If you have any questions along the way, our team is always happy to help.
                            </p>

                            <p style='margin:24px 0 0 0; font-size:13px; color:#888888; line-height:1.6;'>
                                🔒 If you didn't create this account, you can safely ignore this email.
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color:#f9f9f9; padding:22px 32px; text-align:center; border-top:1px solid #eeeeee;'>
                            <p style='margin:0; font-size:12px; color:#999999; line-height:1.6;'>
                                We're glad you're here.<br/>
                                <strong style='color:#555555;'>{quotationemailsettings.DisplayName}</strong>
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
                Subject = $"Welcome to {quotationemailsettings.DisplayName}!",
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

    }
}
