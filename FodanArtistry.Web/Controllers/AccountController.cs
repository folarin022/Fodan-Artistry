using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Application.DTOs.AccountModel;

namespace FodanArtistry.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _accountService.RegisterAsync(dto, cancellationToken);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors ?? new[] { result.Message })
                ModelState.AddModelError("", error);

            return View(dto);
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
            return RedirectToAction("Index", "Home");
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