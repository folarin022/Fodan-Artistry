using FodanArtistry.Application;
using FodanArtistry.Application.DTOs;
using FodanArtistry.Application.DTOs.AccountModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FodanArtistry.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IAccountService accountService,
            UserManager<ApplicationUser> userManager) 
        {
            _accountService = accountService;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Register() => View(new RegisterDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _accountService.RegisterAsync(dto, cancellationToken);

            if (result.IsSuccess)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user != null)
                {
                    // Generate 6-digit code
                    var code = new Random().Next(100000, 999999).ToString();
                    user.EmailVerificationCode = code;
                    user.CodeExpiryTime = DateTime.UtcNow.AddMinutes(10);
                    await _userManager.UpdateAsync(user);

                    // ✅ NO EMAIL SENDING! Just store code in TempData
                    TempData["VerificationCode"] = code;
                    TempData["VerificationEmail"] = user.Email;
                }

                TempData["SuccessMessage"] = "Registration successful! Use the code below to verify your email.";
                return RedirectToAction("VerifyEmail", new { email = dto.Email });
            }

            foreach (var error in result.Errors ?? new[] { result.Message })
            {
                ModelState.AddModelError("", error);
            }

            return View(dto);
        }



        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            return View(new VerifyEmailDto { Email = email });
        }


        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return NotFound();

            if (user.CodeExpiryTime < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "Code expired");
                return View(dto);
            }

            if (user.EmailVerificationCode != dto.Code)
            {
                ModelState.AddModelError("", "Invalid code");
                return View(dto);
            }

            user.EmailConfirmed = true;
            user.EmailVerificationCode = null;

            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Email verified!";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _accountService.LoginAsync(dto, cancellationToken);
            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Message);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _accountService.GetUserProfileAsync(userId, cancellationToken);
            return profile == null ? NotFound() : View(profile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                return View("Profile", await _accountService.GetUserProfileAsync(userId, cancellationToken));
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var updated = await _accountService.UpdateUserProfileAsync(currentUserId, dto, cancellationToken);

            TempData[updated != null ? "SuccessMessage" : "ErrorMessage"] =
                updated != null ? "Profile updated" : "Failed to update profile";

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the errors";
                return RedirectToAction(nameof(Profile));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _accountService.ChangePasswordAsync(userId, dto, cancellationToken);

            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Password changed" : "Failed to change password";

            return RedirectToAction(nameof(Profile));
        }

        public IActionResult AccessDenied() => View();
    }
}