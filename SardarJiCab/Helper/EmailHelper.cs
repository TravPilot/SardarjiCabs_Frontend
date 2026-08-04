using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text.RegularExpressions;

namespace SardarJi_Cab_Booking.Helper
{
    public class EmailHelper
    {
        public string ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Bcc { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> attachmentFullPath { get; set; }
        public List<string> CcMailIdList { get; set; }
        public List<string> BccMailIdList { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Attachment { get; set; }
        public string FilePath { get; set; }
        public string DisplayName { get; set; }

        public async Task SendEmail()
        {
            var message = new MailMessage();

            using (var smtpClient = new SmtpClient())
            {
                await smtpClient.SendMailAsync(message);
            }
        }

        #region Static Members

    

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<bool> SendMailMessage()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            bool IsMailSent = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(this.UserId, this.Password);
            //create the MailMessage object
            MailMessage mMailMessage = new MailMessage();

            //set the sender address of the mail message
            if (!string.IsNullOrEmpty(this.FromEmail))
            {
                mMailMessage.From = new MailAddress(this.FromEmail);
            }

            //set the recipient address of the mail message
            mMailMessage.To.Add(new MailAddress(this.ToEmail));

            //set the blind carbon copy address
            if (!string.IsNullOrEmpty(this.Bcc))
            {
                mMailMessage.Bcc.Add(new MailAddress(this.Bcc));
            }

            //set the carbon copy address
            if (!string.IsNullOrEmpty(this.CC))
            {
                string[] CCEmail = this.CC.Split(',');
                foreach (string ccmail in CCEmail)
                {
                    mMailMessage.CC.Add(new MailAddress(ccmail)); //Adding Multiple CC email Id
                }
                //  mMailMessage.CC.Add(new MailAddress(this.CC));
            }

            //set the subject of the mail message
            if (!string.IsNullOrEmpty(this.Subject))
            {
                mMailMessage.Subject = Subject;
            }
            else
            {
                mMailMessage.Subject = "TraviYo Mail Notifications";

            }


            //set the body of the mail message
            mMailMessage.Body = Body;

            //set the format of the mail message body
            mMailMessage.IsBodyHtml = true;

            //set the priority
            mMailMessage.Priority = MailPriority.Normal;
            if (attachmentFullPath != null)
            {
                //add any attachments from the filesystem
                foreach (var attachmentPath in attachmentFullPath)
                {
                    Attachment mailAttachment = new Attachment(attachmentPath);
                    mMailMessage.Attachments.Add(mailAttachment);
                }
            }
            //create the SmtpClient instance
            using (SmtpClient SmtpClient = new SmtpClient())
            {

                SmtpClient.Host = this.Host;
                SmtpClient.Port = Convert.ToInt16(this.Port);

                //SmtpClient.Port = 80;
                SmtpClient.UseDefaultCredentials = false;
                SmtpClient.Credentials = credentials;
                SmtpClient.EnableSsl = false;

                //send the mail message
                try
                {
                    SmtpClient.Send(mMailMessage);
                    //                    await SmtpClient.SendMailAsync(mMailMessage);
                    IsMailSent = true;
                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                {
                    IsMailSent = false;
                }
            }
            return IsMailSent;
        }

        public bool SendMailMsg(out string Remarks)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (IsValidEmailAddress(this.ToEmail) && IsValidEmailAddress(this.FromEmail))
            {


                bool IsMailSent = false;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(this.UserId, this.Password);
                //create the MailMessage object
                using (MailMessage mMailMessage = new MailMessage())
                {
                    //set the sender address of the mail message
                    if (!string.IsNullOrEmpty(this.FromEmail))
                    {
                        mMailMessage.From = new MailAddress(this.FromEmail, DisplayName);
                    }

                    //set the recipient address of the mail message
                    mMailMessage.To.Add(new MailAddress(this.ToEmail));

                    //set the blind carbon copy address
                    if (!string.IsNullOrEmpty(this.Bcc))
                    {
                        mMailMessage.Bcc.Add(new MailAddress(this.Bcc));
                    }

                    //set the carbon copy address
                    if (!string.IsNullOrEmpty(this.CC))
                    {
                        string[] CCEmail = null;
                        if (CC.Contains(","))
                        {
                            CCEmail = this.CC.Split(',');
                        }
                        else if (CC.Contains(";"))
                        {
                            CCEmail = this.CC.Split(',');
                        }

                        if (CCEmail != null && CCEmail.Count() > 0)
                        {
                            foreach (string ccmail in CCEmail)
                            {
                                if (IsValidEmailAddress(ccmail))
                                {
                                    mMailMessage.CC.Add(new MailAddress(ccmail)); //Adding Multiple CC email Id
                                }

                            }
                        }
                        if (IsValidEmailAddress(this.CC))
                        {
                            mMailMessage.CC.Add(new MailAddress(this.CC)); //Adding Multiple CC email Id
                        }

                        //  mMailMessage.CC.Add(new MailAddress(this.CC));
                    }

                    //set the subject of the mail message
                    if (!string.IsNullOrEmpty(this.Subject))
                    {
                        string subb = Regex.Replace(Subject, @"\r\n?|\n|\t", String.Empty);
                        mMailMessage.Subject = subb;
                    }
                    else
                    {
                        mMailMessage.Subject = "TravelLED Mail Notifications";

                    }

                    //set the body of the mail message
                    mMailMessage.Body = Body;

                    //set the format of the mail message body
                    mMailMessage.IsBodyHtml = true;

                    //set the priority
                    mMailMessage.Priority = MailPriority.Normal;
                    if (attachmentFullPath != null)
                    {
                        //add any attachments from the filesystem
                        foreach (var attachmentPath in attachmentFullPath)
                        {
                            Attachment mailAttachment = new Attachment(attachmentPath);
                            mMailMessage.Attachments.Add(mailAttachment);
                        }
                    }


                    if (this.FilePath != null && this.FilePath != "")
                    {
                        Attachment attachment;
                        attachment = new Attachment(this.FilePath);
                        mMailMessage.Attachments.Add(attachment);

                    }

                    //create the SmtpClient instance
                    using (SmtpClient SmtpClient = new SmtpClient())
                    {

                        //SmtpClient.Host = "smtpout.secureserver.net";
                        //SmtpClient.Port = 80;
                        SmtpClient.Host = this.Host;
                        SmtpClient.Port = Convert.ToInt16(this.Port);
                        //SmtpClient.Port = 80;
                        SmtpClient.UseDefaultCredentials = false;
                        SmtpClient.Credentials = credentials;
                        SmtpClient.EnableSsl = false;

                        //send the mail message
                        try
                        {
                            SmtpClient.SendMailAsync(mMailMessage);
                            //                    await SmtpClient.SendMailAsync(mMailMessage);
                            IsMailSent = true;
                            Remarks = "Email Send Successfully";
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            Remarks = ex.Message.ToString();
                            IsMailSent = false;
                        }
                    }

                    return IsMailSent;
                }
            }
            else
            { Remarks = "Invalid Email Id"; return false; }
        }

        public bool Send(out string Remarks)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (IsValidEmailAddress(this.ToEmail) && IsValidEmailAddress(this.FromEmail))
            {


                bool IsMailSent = false;
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(this.UserId, this.Password);
                //create the MailMessage object
                using (MailMessage mMailMessage = new MailMessage())
                {
                    //set the sender address of the mail message
                    if (!string.IsNullOrEmpty(this.FromEmail))
                    {
                        mMailMessage.From = new MailAddress(this.FromEmail, DisplayName);
                    }

                    //set the recipient address of the mail message
                    mMailMessage.To.Add(new MailAddress(this.ToEmail));

                    //set the blind carbon copy address
                    if (!string.IsNullOrEmpty(this.Bcc))
                    {
                        mMailMessage.Bcc.Add(new MailAddress(this.Bcc));
                    }

                    //set the carbon copy address
                    if (!string.IsNullOrEmpty(this.CC))
                    {
                        string[] CCEmail = null;
                        if (CC.Contains(","))
                        {
                            CCEmail = this.CC.Split(',');
                        }
                        else if (CC.Contains(";"))
                        {
                            CCEmail = this.CC.Split(',');
                        }

                        if (CCEmail != null && CCEmail.Count() > 0)
                        {
                            foreach (string ccmail in CCEmail)
                            {
                                if (IsValidEmailAddress(ccmail))
                                {
                                    mMailMessage.CC.Add(new MailAddress(ccmail)); //Adding Multiple CC email Id
                                }

                            }
                        }
                        if (IsValidEmailAddress(this.CC))
                        {
                            mMailMessage.CC.Add(new MailAddress(this.CC)); //Adding Multiple CC email Id
                        }

                        //  mMailMessage.CC.Add(new MailAddress(this.CC));
                    }

                    //set the subject of the mail message
                    if (!string.IsNullOrEmpty(this.Subject))
                    {
                        string subb = Regex.Replace(Subject, @"\r\n?|\n|\t", String.Empty);
                        mMailMessage.Subject = subb;
                    }
                    else
                    {
                        mMailMessage.Subject = "TravelLED Mail Notifications";

                    }

                    //set the body of the mail message
                    mMailMessage.Body = Body;

                    //set the format of the mail message body
                    mMailMessage.IsBodyHtml = true;

                    //set the priority
                    mMailMessage.Priority = MailPriority.Normal;
                    if (attachmentFullPath != null)
                    {
                        //add any attachments from the filesystem
                        foreach (var attachmentPath in attachmentFullPath)
                        {
                            Attachment mailAttachment = new Attachment(attachmentPath);
                            mMailMessage.Attachments.Add(mailAttachment);
                        }
                    }


                    if (this.FilePath != null && this.FilePath != "")
                    {
                        Attachment attachment;
                        attachment = new Attachment(this.FilePath);
                        mMailMessage.Attachments.Add(attachment);

                    }

                    //create the SmtpClient instance
                    using (SmtpClient SmtpClient = new SmtpClient())
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // .NET 4.5
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                        //SmtpClient.Host = "smtpout.secureserver.net";
                        //SmtpClient.Port = 80;
                        SmtpClient.Host = this.Host;
                        SmtpClient.Port = Convert.ToInt16(this.Port);
                        //SmtpClient.Port = 80;
                        SmtpClient.UseDefaultCredentials = false;
                        SmtpClient.Credentials = credentials;
                        SmtpClient.EnableSsl = false;

                        //send the mail message
                        try
                        {
                            SmtpClient.Send(mMailMessage);
                            //                    await SmtpClient.SendMailAsync(mMailMessage);
                            IsMailSent = true;
                            Remarks = "Email Send Successfully";
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            Remarks = ex.Message.ToString();
                            IsMailSent = false;
                        }
                    }

                    return IsMailSent;
                }
            }
            else
            { Remarks = "Invalid Email Id"; return false; }
        }





        /// <summary>
        /// Determines whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the email address is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (String.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }

        #endregion

        /// <summary>
        /// to convert static images to embedded images
        /// for the purpose of auto download images
        /// 
        /// </summary>
        /// <param name="File"></param>
        /// <param name="email"></param>
        /// <returns></returns>

        public List<AlternateView> getEmbeddeImage(IEnumerable<string> File, string email)
        {
            List<AlternateView> listAlternateView = new List<AlternateView>();
            foreach (var item in File)
            {
                LinkedResource linkedResource = new LinkedResource(item);
                linkedResource.ContentId = Guid.NewGuid().ToString();
                string htmlBody = string.Format(email, linkedResource.ContentId);

                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                alternateView.LinkedResources.Add(linkedResource);
                listAlternateView.Add(alternateView);
            }
            return listAlternateView;
        }

    }
}
