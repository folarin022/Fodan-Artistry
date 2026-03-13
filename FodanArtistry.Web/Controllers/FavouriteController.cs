using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FodanArtistry.Application.Interfaces;

namespace FodanArtistry.Web.Controllers
{
    [Authorize]
    public class FavouriteController : Controller
    {
        private readonly IFavouriteService _favouriteService;

        public FavouriteController(IFavouriteService favouriteService)
        {
            _favouriteService = favouriteService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var favorites = await _favouriteService.GetUserFavoritesAsync(userId, cancellationToken);
            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid artworkId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _favouriteService.RemoveFromFavoritesAsync(userId, artworkId, cancellationToken);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result ? "Removed from favorites" : "Failed to remove";
            return RedirectToAction(nameof(Index));
        }
    }
}