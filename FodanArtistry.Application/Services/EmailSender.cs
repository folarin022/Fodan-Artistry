using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FodanArtistry.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateService _templateService;

        public EmailSender(
            IConfiguration configuration,
            IEmailTemplateService templateService)
        {
            _configuration = configuration;
            _templateService = templateService;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailInternalAsync(email, subject, htmlMessage);
        }

        public async Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink)
        {
            var htmlBody = _templateService.GetConfirmationEmail(firstName, confirmationLink);
            await SendEmailInternalAsync(email, "Confirm your email - Fodan Artistry", htmlBody);
        }

        private async Task SendEmailInternalAsync(string email, string subject, string htmlBody)
        {
            try
            {
                // Log what we're about to do
                Console.WriteLine($"=== ATTEMPTING TO SEND EMAIL ===");
                Console.WriteLine($"Host: {_configuration["EmailSettings:Host"]}");
                Console.WriteLine($"Port: {_configuration["EmailSettings:Port"]}");
                Console.WriteLine($"Username: {_configuration["EmailSettings:Username"]}");
                Console.WriteLine($"Password length: {_configuration["EmailSettings:Password"]?.Length ?? 0}");
                Console.WriteLine($"To: {email}");
                Console.WriteLine($"Subject: {subject}");

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
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        _configuration["EmailSettings:FromEmail"],
                        _configuration["EmailSettings:FromName"]
                    ),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                Console.WriteLine("Attempting to send...");
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP ERROR: {smtpEx.Message}");
                Console.WriteLine($"Status Code: {smtpEx.StatusCode}");
                if (smtpEx.InnerException != null)
                    Console.WriteLine($"Inner: {smtpEx.InnerException.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GENERAL ERROR: {ex.Message}");
                throw;
            }
        }
    }
}