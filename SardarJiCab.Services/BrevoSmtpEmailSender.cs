using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SardarJiCab.Model;
using SardarJiCab.Services.Interface;

namespace SardarJiCab.Services
{
    public class BrevoSmtpEmailSender : IEmailSender
    {
        private readonly BrevoSmtpOptions _options;
        private readonly ILogger<BrevoSmtpEmailSender> _logger;

        public BrevoSmtpEmailSender(IOptions<BrevoSmtpOptions> options, ILogger<BrevoSmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, string textBody = null)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody ?? StripHtml(htmlBody)
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            try
            {
                var socketOptions = _options.UseSsl
                    ? SecureSocketOptions.SslOnConnect   // port 465
                    : SecureSocketOptions.StartTls;      // port 587

                await client.ConnectAsync(_options.Host, _options.Port, socketOptions);
                await client.AuthenticateAsync(_options.SmtpLogin, _options.SmtpKey);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Brevo SMTP send failed for {Recipient}", toEmail);
                throw new EmailSendException("Could not send the email. Please try again.", ex);
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true);
            }

            _logger.LogInformation("Email sent via Brevo SMTP to {Recipient}", toEmail);
        }

        private static string StripHtml(string html) =>
            System.Text.RegularExpressions.Regex.Replace(html ?? "", "<.*?>", string.Empty);
    }

    public class EmailSendException : Exception
    {
        public EmailSendException(string message) : base(message) { }
        public EmailSendException(string message, Exception inner) : base(message, inner) { }
    }
}
