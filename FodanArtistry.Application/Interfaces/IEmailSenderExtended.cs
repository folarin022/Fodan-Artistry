using Microsoft.AspNetCore.Identity.UI.Services;

namespace FodanArtistry.Application.Interfaces
{
    public interface IEmailSenderExtended : IEmailSender
    {
        Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink);
        Task SendWelcomeEmailAsync(string email, string firstName);
    }
}