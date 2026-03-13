using System;

namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;  
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? DateJoined { get; set; }
        public int FavoriteCount { get; set; }
        public int OrderCount { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}