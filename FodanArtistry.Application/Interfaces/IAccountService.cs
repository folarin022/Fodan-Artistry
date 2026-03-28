using FodanArtistry.Application.DTOs.AccountModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Interfaces
{
    public interface IAccountService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task LogoutAsync();

        Task<UserProfileDto> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);

        Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
        Task<bool> ForgotPasswordAsync(string email, string resetLink, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default);
    }
}
        