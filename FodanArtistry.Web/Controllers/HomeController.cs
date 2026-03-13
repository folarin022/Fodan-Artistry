using Microsoft.AspNetCore.Mvc;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Application.DTOs.ArtworkModel;

namespace FodanArtistry.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArtworkService _artworkService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            IArtworkService artworkService,
            ICategoryService categoryService)
        {
            _artworkService = artworkService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Get featured artworks (latest 6)
            var featured = await _artworkService.GetGalleryAsync(
                pageNumber: 1,
                pageSize: 6,
                cancellationToken: cancellationToken);

            // Get categories for navigation
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            ViewBag.Categories = categories;
            return View(featured.Items);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(string name, string email, string message)
        {
            // Handle contact form submission
            TempData["SuccessMessage"] = "Thank you for contacting us! We'll get back to you soon.";
            return RedirectToAction("Contact");
        }
    }
}