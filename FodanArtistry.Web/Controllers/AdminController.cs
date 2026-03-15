using AspNetCoreGeneratedDocument;
using FodanArtistry.Application.DTOs.ArtworkDto;
using FodanArtistry.Application.DTOs.CategoryModel;
using FodanArtistry.Application.DTOs.OrderModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FodanArtistry.Web.Controllers
{
    [Authorize(Roles = "Admin,Artist")]
    public class AdminController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IArtworkService _artworkService;
        private readonly IAccountService _accountService;

        public AdminController(
            IOrderService orderService,
            ICategoryService categoryService,
            IArtworkService artworkService,
            IAccountService accountService)
        {
            _orderService = orderService;
            _categoryService = categoryService;
            _artworkService = artworkService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var recentOrders = await _orderService.GetRecentOrdersAsync(10, cancellationToken);
            var orderSummary = await _orderService.GetOrderSummaryAsync(cancellationToken);
            var categories = await _categoryService.GetCategoriesWithArtworkCountsAsync(cancellationToken);
            var users = await _accountService.GetAllUsersAsync(cancellationToken);

            ViewBag.RecentOrders = recentOrders;
            ViewBag.OrderSummary = orderSummary;
            ViewBag.Categories = categories;
            ViewBag.TotalUsers = users.Count();
            ViewBag.TotalArtworks = categories.Sum(c => c.ArtworkCount);

            return View();
        }

        public async Task<IActionResult> Orders(CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return View(orders ?? new List<OrderDto>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string status, CancellationToken cancellationToken)
        {
            if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                await _orderService.UpdateOrderStatusAsync(orderId, orderStatus, cancellationToken);
                TempData["SuccessMessage"] = "Order status updated";
            }
            return RedirectToAction(nameof(Orders));
        }

        public async Task<IActionResult> Categories(CancellationToken cancellationToken)
        {
            return View(await _categoryService.GetCategoriesWithArtworkCountsAsync(cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.CreateCategoryAsync(dto, cancellationToken);
                TempData["SuccessMessage"] = "Category created";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(dto, cancellationToken);
                TempData["SuccessMessage"] = "Category updated";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result ? "Category deleted" : "Category not found";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> Users(CancellationToken cancellationToken)
        {
            return View(await _accountService.GetAllUsersAsync(cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string role, CancellationToken cancellationToken)
        {
            try
            {
                await _accountService.AssignRoleAsync(userId, role, cancellationToken);
                TempData["SuccessMessage"] = $"Role '{role}' assigned";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> AddArtWork(CreateArtworkDto Dto,string artistId,CancellationToken cancellationToken)
        {
            try
            {
                await _artworkService.CreateArtworkAsync(Dto, artistId, cancellationToken);
                TempData["SuccessMessage"] = $"Artwork '{Dto}' created";
            }
            catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
            return RedirectToAction(nameof(Views_Admin_Index));
        }
    }
}