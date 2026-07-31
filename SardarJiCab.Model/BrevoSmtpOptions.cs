using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SardarJiCab.Model
{
    public class BrevoSmtpOptions
    {
        public string Host { get; set; } = "smtp-relay.brevo.com";
        public int Port { get; set; } = 587;               // 587 = STARTTLS, 465 = SSL
        public bool UseSsl { get; set; } = false;           // set true only if Port = 465
        public string SmtpLogin { get; set; }               // from Brevo → SMTP & API page
        public string SmtpKey { get; set; }                 // the SMTP "master password" / key, NOT the API key
        public string SenderEmail { get; set; }             // must be a verified sender in Brevo
        public string SenderName { get; set; } = "Sardar Ji EV";
    }
}
