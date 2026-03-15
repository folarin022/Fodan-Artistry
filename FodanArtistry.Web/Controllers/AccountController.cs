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
        private readonly Microsoft.AspNetCore.Identity.UI.Services. IEmailSender _emailSender;

        public AccountController(
            IAccountService accountService,
            UserManager<ApplicationUser> userManager,
            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender) 
        {
            _accountService = accountService;
            _userManager = userManager;
            _emailSender = emailSender;
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
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token = token },
                        protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(
                        user.Email,
                        user.FirstName,
                        confirmationLink
                    );
                }

                TempData["SuccessMessage"] = "Registration successful! Please check your email to confirm your account.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors ?? new[] { result.Message })
            {
                ModelState.AddModelError("", error);
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to find user");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
                return View("EmailConfirmed");
            }
            else
            {
                TempData["ErrorMessage"] = "Error confirming your email. The link may have expired.";
                return View("Error");
            }
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