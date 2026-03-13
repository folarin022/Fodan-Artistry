using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Application.DTOs.OrderModel;

namespace FodanArtistry.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IArtworkService _artworkService;

        public OrderController(IOrderService orderService, IArtworkService artworkService)
        {
            _orderService = orderService;
            _artworkService = artworkService;
        }

        [Authorize]
        public async Task<IActionResult> Checkout(Guid artworkId, CancellationToken cancellationToken)
        {
            var artwork = await _artworkService.GetArtworkByIdAsync(artworkId, cancellationToken);
            if (artwork == null) return NotFound();

            var orderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ArtworkId = artworkId,
                    ArtworkTitle = artwork.Title,
                    UnitPrice = artwork.Price,
                    Quantity = 1
                }
            };

            var dto = new CreateOrderDto
            {
                OrderItems = orderItems
            };

            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = await _orderService.CreateOrderAsync(dto, userId, cancellationToken);
                TempData["SuccessMessage"] = $"Order #{order.OrderNumber} created!";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderAsync(id, cancellationToken);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.CustomerEmail != User.Identity?.Name)
                return Forbid();

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
            return View(orders);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id, cancellationToken);
                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result ? "Order cancelled" : "Order not found";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(MyOrders));
        }
    }
}