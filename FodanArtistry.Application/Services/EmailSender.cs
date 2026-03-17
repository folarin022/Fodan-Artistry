using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FodanArtistry.Application.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["EmailSettings:Host"])
                {
                    Port = int.Parse(_configuration["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    ),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        _configuration["EmailSettings:FromEmail"],
                        _configuration["EmailSettings:FromName"] ?? "Fodan Artistry"
                    ),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email error: {ex.Message}");
                throw;
            }
        }
    }
}