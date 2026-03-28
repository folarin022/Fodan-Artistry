using FodanArtistry.Application.DTOs.AccountModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailsender;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailsender = emailSender;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            var response = new AuthResultDto();

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Email already registered";
                    return response;
                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PhoneNumber = registerDto.PhoneNumber,
                    Gender = registerDto.Gender,
                    EmailConfirmed = false,
                    IsArtistRequested = registerDto.IsArtistRequested,
                };
                if (!IsValidEmail(registerDto.Email))
                {
                    response.IsSuccess = false;
                    response.Message = "Please enter a valid email address";
                    return response;
                }

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = "Registration failed";
                    response.Errors = result.Errors.Select(e => e.Description);
                    return response;
                }

                await _userManager.AddToRoleAsync(user, "Customer");
                
                response.IsSuccess = true;
                response.Message = "Registration successful! Please check your email to confirm your account.";
                response.UserId = user.Id;
                response.RequiresEmailConfirmation = true;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred during registration";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            var response = new AuthResultDto();

            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid email or password";
                    return response;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    loginDto.Password,
                    loginDto.RememberMe,
                    lockoutOnFailure: false);

                if (!result.Succeeded)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid email or password";
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);

                response.IsSuccess = true;
                response.Message = "Login successful!";
                response.UserId = user.Id;
                response.Email = user.Email;
                response.Roles = roles.ToList();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred during login";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                FavoriteCount = user.Favorites?.Count ?? 0,
                OrderCount = user.Orders?.Count ?? 0,
                Roles = roles.ToList()
            };
        }

        public async Task<UserProfileDto?> UpdateUserProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return null;

            return await GetUserProfileAsync(userId, cancellationToken);
        }
        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword);

            return result.Succeeded;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userManager.Users
                .Include(u => u.Artworks)
                .Include(u => u.Orders)
                .Include(u => u.Favorites)
                .ToListAsync(cancellationToken);

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    Roles = roles.ToList(),
                    ArtworkCount = user.Artworks?.Count ?? 0,
                    OrderCount = user.Orders?.Count ?? 0,
                    IsActive = user.LockoutEnabled && user.LockoutEnd == null,
                });
            }

            return userDtos;
        }

        public async Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }


        public async Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<bool> ForgotPasswordAsync(string email, string resetLink, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return false; // Don't reveal that the user doesn't exist

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{resetLink}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailsender.SendEmailAsync(
                user.Email,
                "Reset your password - Fodan Artistry",
                $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
            <h2>Hello {user.FirstName},</h2>
            <p>We received a request to reset your password. Click the button below:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{callbackUrl}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 12px 30px; text-decoration: none; border-radius: 25px; display: inline-block;'>Reset Password</a>
            </div>
            <p>If you didn't request this, please ignore this email.</p>
            <p>This link expires in 24 hours.</p>
        </div>"
            );

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }
}