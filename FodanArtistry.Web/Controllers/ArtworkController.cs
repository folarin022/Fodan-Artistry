using FodanArtistry.Application.DTOs.ArtworkDto;
using FodanArtistry.Application.DTOs.ArtworkModel;
using FodanArtistry.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FodanArtistry.Web.Controllers
{
    public class ArtworkController : Controller
    {
        private readonly IArtworkService _artworkService;
        private readonly ICategoryService _categoryService;
        private readonly IFavouriteService _favouriteService;

        public ArtworkController(
            IArtworkService artworkService,
            ICategoryService categoryService,
            IFavouriteService favouriteService)
        {
            _artworkService = artworkService;
            _categoryService = categoryService;
            _favouriteService = favouriteService;
        }

        public async Task<IActionResult> Gallery(int page = 1, string? category = null, string? search = null, CancellationToken cancellationToken = default)
        {
            var artworks = await _artworkService.GetGalleryAsync(page, 12, category, search, cancellationToken);
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(artworks);
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var artwork = await _artworkService.GetArtworkByIdAsync(id, cancellationToken);
            if (artwork == null) return NotFound();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsFavorited = await _favouriteService.IsFavoriteAsync(userId!, id, cancellationToken);
            }

            var related = await _artworkService.GetGalleryAsync(1, 4, artwork.CategoryName, cancellationToken: cancellationToken);
            ViewBag.RelatedArtworks = related.Items.Where(a => a.Id != id);

            return View(artwork);
        }

        public async Task<IActionResult> Artist(string artistId, CancellationToken cancellationToken)
        {
            var artworks = await _artworkService.GetArtistPortfolioAsync(artistId, cancellationToken);
            return View(artworks);
        }

        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(new CreateArtworkDto());
        }

        [HttpPost]
        [Authorize(Roles = "Artist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateArtworkDto dto, IFormFile? imageFile, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
                return View(dto);
            }

            try
            {
                var artistId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var artwork = await _artworkService.CreateArtworkAsync(dto, artistId, cancellationToken);

                TempData["SuccessMessage"] = "Artwork created successfully!";
                return RedirectToAction(nameof(Details), new { id = artwork.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
                return View(dto);
            }
        }

        [Authorize(Roles = "Artist,Admin")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var artwork = await _artworkService.GetArtworkByIdAsync(id, cancellationToken);
            if (artwork == null) return NotFound();

            if (!User.IsInRole("Admin") && artwork.ArtistId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            return View(new UpdateArtworkDto
            {
                Id = artwork.Id,
                Title = artwork.Title,
                Description = artwork.Description,
                Price = artwork.Price,
                CategoryId = artwork.CategoryId,
                ImageUrl = artwork.ImageUrl,
                IsAvailable = artwork.IsAvailable
            });
        }

        [HttpPost]
        [Authorize(Roles = "Artist,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateArtworkDto dto, IFormFile? imageFile, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
                return View(dto);
            }

            try
            {
                var artwork = await _artworkService.UpdateArtworkAsync(dto, cancellationToken);
                TempData["SuccessMessage"] = "Artwork updated successfully!";
                return RedirectToAction(nameof(Details), new { id = artwork.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
                return View(dto);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Artist,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var success = await _artworkService.DeleteArtworkAsync(id, cancellationToken);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Artwork deleted" : "Artwork not found";
            return RedirectToAction(nameof(Gallery));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(Guid artworkId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _favouriteService.ToggleFavoriteAsync(userId, artworkId, cancellationToken);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var isFavorited = await _favouriteService.IsFavoriteAsync(userId, artworkId, cancellationToken);
                var favoriteCount = await _favouriteService.GetFavoriteCountAsync(artworkId, cancellationToken);

                return Json(new
                {
                    success = true,
                    isFavorited,
                    favoriteCount
                });
            }

            return RedirectToAction(nameof(Details), new { id = artworkId });
        }
    }
}