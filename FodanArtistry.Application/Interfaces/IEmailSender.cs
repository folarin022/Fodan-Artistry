namespace FodanArtistry.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendConfirmationEmailAsync(string email, string firstName, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink);
        Task SendWelcomeEmailAsync(string email, string firstName);
    }
}