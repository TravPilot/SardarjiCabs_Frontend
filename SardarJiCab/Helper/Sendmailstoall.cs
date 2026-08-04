using SardarJi_Cab_Booking.Models;
using System.Net;

namespace SardarJi_Cab_Booking.Helper
{
    public class Sendmailstoall
    {

        public async void SendMail(QueryVM EmailSms)
        {


            {
                EmailHelper emailHelper = new EmailHelper();
               
                emailHelper.FromEmail = EmailSms.From;
                emailHelper.ToEmail = EmailSms.Sendto;
                emailHelper.Subject = EmailSms.Subject;
                emailHelper.Body = EmailSms.Body;

                //emailHelper.Host = "smtp-relay.sendinblue.com";
                //emailHelper.Port = "587";
                //emailHelper.UserId = "divyansh1.traviyo@gmail.com";
                //emailHelper.Password = "pQHLbn9PTyICWMdO";

                emailHelper.Host = EmailSms.Host;
                emailHelper.Port = EmailSms.Port;
                emailHelper.UserId = EmailSms.UserId;
                emailHelper.Password = EmailSms.Password;


                emailHelper.Attachment = EmailSms.FilePath == null ? "" : EmailSms.FilePath;
                emailHelper.FilePath = EmailSms.FilePath == null ? "" : EmailSms.FilePath;
                emailHelper.DisplayName = EmailSms.DisplayName;

                bool IsMailSent = true;
                if (emailHelper.Attachment != null && emailHelper.Attachment != "")
                {
                    try
                    {

                        using (WebClient client = new WebClient())
                        {
                            var url = new Uri(emailHelper.Attachment);
                            string path = url.ToString();

                            var tempFile = Path.GetTempPath() + Path.GetFileName(path);

                            var bytes = client.DownloadData(url);
                            using (FileStream fs = new FileStream(tempFile, FileMode.OpenOrCreate))
                            {
                                fs.Write(bytes, 0, bytes.Length);
                            }

                            emailHelper.FilePath = tempFile;
                        }

                    }
                    catch (Exception ex)
                    {
                        emailHelper.FilePath = "";
                    }
                }

                string Remarks;

                emailHelper.Send(out Remarks);


                if (IsMailSent)
                {
                    //_emailSmsData.UpdateStatusAfterEmail(Remarks, EmailSms);

                    //if (EmailSms.ReminderId != 0)
                    //{
                    //    Reminder reminder = new Reminder();
                    //    reminder.Id = EmailSms.ReminderId;
                    //    reminder.ReminderType = EmailSms.ReminderType;
                    //    _reminderData.UpdateStatusAfterRemind(reminder);
                    //}
                }
                else
                {
                    //UpdateMailStatus(Remarks, EmailSms);
                }

            }
        }
    }
}
