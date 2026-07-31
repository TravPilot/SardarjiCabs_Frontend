using SardarJiCab.Model.SardarJiEV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.BL.Interface
{
    public interface IDriverLoginBL
    {
        Task<DriverLoginResult> LoginWithPasswordAsync(string mobile, string password, string ipAddress);
        Task<OtpRequestResult> RequestOtpAsync(string mobile);
        Task<DriverLoginResult> VerifyOtpAsync(string mobile, string otp, string ipAddress);
    }
}
