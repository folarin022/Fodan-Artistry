using System;
using System.Collections.Generic;

namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? DateJoined { get; set; }
        public List<string> Roles { get; set; } = new();
        public int ArtworkCount { get; set; }
        public int OrderCount { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePicture { get; set; }
    }
}