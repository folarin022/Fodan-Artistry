namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class SettingsViewModel
    {
        public UserProfileDto Profile { get; set; } = new();
        public ChangePasswordDto PasswordChange { get; set; } = new();
        public UpdateProfileDto ProfileUpdate { get; set; } = new();
    }
}