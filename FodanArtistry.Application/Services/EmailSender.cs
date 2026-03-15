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
                var smtpClient = new SmtpClient(_configuration["EmailSettings:Host"])
                {
                    Port = int.Parse(_configuration["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    ),
                    EnableSsl = true,  // ← MUST be true for Gmail
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false  
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

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw; // Re-throw to see the error
            }
        }
    }
}