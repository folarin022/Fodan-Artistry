using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FodanArtistry.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PaymentController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            if (!user.IsArtistRequested)
                return RedirectToAction("Login", "Account");

            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(string userId, string paymentMethod)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            bool paymentSuccess = true; 

            if (paymentSuccess)
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
                await _userManager.AddToRoleAsync(user, "Artist");
                user.IsArtistRequested = false;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Payment successful! You are now an artist. Please log in.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["ErrorMessage"] = "Payment failed. You remain a customer. Please try again later.";
                return RedirectToAction("Subscribe", new { userId });
            }
        }
    }
}