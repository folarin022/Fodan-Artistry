using System.ComponentModel.DataAnnotations;

namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        public string? ProfilePicture { get; set; }
    }
}