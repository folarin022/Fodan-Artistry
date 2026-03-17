using System.Collections.Generic;

namespace FodanArtistry.Application.DTOs.AccountModel
{
    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public bool RequiresEmailConfirmation { get; internal set; }
        public string ConfirmationToken { get; internal set; }
    }
}