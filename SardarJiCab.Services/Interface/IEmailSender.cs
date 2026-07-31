using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Services.Interface
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody, string textBody = null);
    }
}
