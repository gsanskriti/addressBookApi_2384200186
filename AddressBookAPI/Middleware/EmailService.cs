using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AddressBookAPI.Middleware
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.To.Add(toEmail);
                    mail.From = new MailAddress(_configuration["EmailSettings:FromEmail"]);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"])))
                    {
                        smtp.Credentials = new NetworkCredential(
                            _configuration["EmailSettings:FromEmail"],
                            _configuration["EmailSettings:SmtpPass"]
                        );
                        smtp.EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSSL"]);
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error: {ex.Message}");
            }
        }
    }
}
