using FodanArtistry.Application.DTOs.AccountModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                    response.Errors = new[] { "Email already registered" };
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
                    EmailConfirmed = true 
                };

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
                response.Message = "Registration successful!";
                response.UserId = user.Id;
                response.Email = user.Email;
                response.Roles = new List<string> { "Customer" };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred during registration";
                response.Errors = new[] { ex.Message };
            }

            return response;
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
                ProfilePicture = user.ProfilePicture,
                DateJoined = user.CreatedAt ?? DateTime.UtcNow,
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
            user.ProfilePicture = dto.ProfilePicture ?? user.ProfilePicture;

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
                    DateJoined = user.CreatedAt ?? DateTime.UtcNow,
                    Roles = roles.ToList(),
                    ArtworkCount = user.Artworks?.Count ?? 0,
                    OrderCount = user.Orders?.Count ?? 0,
                    IsActive = user.LockoutEnabled && user.LockoutEnd == null,
                    ProfilePicture = user.ProfilePicture
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
    }
}