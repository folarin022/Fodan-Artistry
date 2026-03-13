using FodanArtistry.Application.DTOs.OrderModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IAccountService _accountService;

        public OrderService(
            IOrderRepository orderRepository,
            IArtworkRepository artworkRepository,
            IAccountService accountService)
        {
            _orderRepository = orderRepository;
            _artworkRepository = artworkRepository;
            _accountService = accountService;
        }

        // ========== CREATE ORDER ==========
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string? userId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate order items
                if (dto.OrderItems == null || !dto.OrderItems.Any())
                    throw new InvalidOperationException("Order must contain at least one item");

                decimal totalAmount = 0;
                var orderItems = new List<OrderItem>();

                // Process each order item
                foreach (var itemDto in dto.OrderItems)
                {
                    // Check if artwork exists and is available
                    var artwork = await _artworkRepository.GetByIdAsync(itemDto.ArtworkId, cancellationToken);
                    if (artwork == null)
                        throw new InvalidOperationException($"Artwork with ID {itemDto.ArtworkId} not found");

                    if (!artwork.IsAvailable)
                        throw new InvalidOperationException($"Artwork '{artwork.Title}' is not available for purchase");

                    // Create order item
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ArtworkId = artwork.Id,
                        ArtworkTitle = artwork.Title,
                        UnitPrice = artwork.Price,
                        Quantity = itemDto.Quantity
                    };

                    orderItems.Add(orderItem);
                    totalAmount += orderItem.Subtotal;

                }

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CustomerName = dto.CustomerName,
                    CustomerEmail = dto.CustomerEmail,
                    PhoneNumber = dto.PhoneNumber,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    OrderItems = orderItems
                };

                var created = await _orderRepository.AddAsync(order, cancellationToken);
                return MapToOrderDto(created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id, cancellationToken);
            if (order == null)
                return null;

            return MapToOrderDto(order);
        }
        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetByUserAsync(userId, cancellationToken);
            return orders.Select(MapToOrderDto).OrderByDescending(o => o.OrderDate);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            order.Status = status;
            await _orderRepository.UpdateAsync(order, cancellationToken);

            return MapToOrderDto(order);
        }

        public async Task<bool> CancelOrderAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
            if (order == null)
                return false;

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled");

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<RecentOrderDto>> GetRecentOrdersAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetRecentOrdersAsync(count, cancellationToken);

            return orders.Select(o => new RecentOrderDto
            {
                OrderId = o.Id,
                OrderNumber = GenerateOrderNumber(o),
                CustomerName = o.User?.FirstName != null
                    ? $"{o.User.FirstName} {o.User.LastName}"
                    : o.CustomerName,
                CustomerEmail = o.User?.Email ?? o.CustomerEmail,
                ItemCount = o.OrderItems?.Count ?? 0,
                TotalAmount = o.TotalAmount,
                OrderDate = o.OrderDate,
                TimeAgo = GetTimeAgo(o.OrderDate),
                OrderStatus = o.Status.ToString(),
                StatusColor = GetStatusColor(o.Status),
                ArtworkTitles = o.OrderItems?.Select(i => i.ArtworkTitle).ToList() ?? new()
            }).ToList();
        }


        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return orders.Select(MapToOrderDto).OrderByDescending(o => o.OrderDate);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _orderRepository.GetTotalSalesAsync(startDate, endDate, cancellationToken);
        }

        public async Task<int> GetOrderCountAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _orderRepository.GetOrderCountAsync(startDate, endDate, cancellationToken);
        }

        

        private async Task<int> GetOrderCountByStatusAsync(OrderStatus status, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetByStatusAsync(status, cancellationToken);
            return orders.Count();
        }

        // ========== PRIVATE HELPER METHODS ==========

        private OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = GenerateOrderNumber(order),
                CustomerName = order.User?.FirstName != null
                    ? $"{order.User.FirstName} {order.User.LastName}"
                    : order.CustomerName,
                CustomerEmail = order.User?.Email ?? order.CustomerEmail,
                CustomerPhone = order.PhoneNumber,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Items = order.OrderItems?.Select(i => new FodanArtistry.Application.DTOs.OrderModel.OrderItemDto
                {
                    ArtworkId = i.ArtworkId,
                    ArtworkTitle = i.ArtworkTitle,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList() ?? new(),
                ItemCount = order.OrderItems?.Count ?? 0
            };
        }

        private string GenerateOrderNumber(Order order)
        {
            return $"ORD-{order.OrderDate:yyyyMMdd}-{order.Id.ToString().Substring(0, 4).ToUpper()}";
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";

            return dateTime.ToString("MMM dd, yyyy");
        }

        private string GetStatusColor(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "warning",
                OrderStatus.Processing => "info",
                OrderStatus.Shipped => "primary",
                OrderStatus.Delivered => "success",
                OrderStatus.Completed => "success",
                OrderStatus.Cancelled => "danger",
                _ => "secondary"
            };
        }

        public async Task<DashboardOrderSummaryDto> GetOrderSummaryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var startOfYear = new DateTime(today.Year, 1, 1);

                // Get orders for different periods
                var todayOrders = await _orderRepository.GetByDateRangeAsync(today, today.AddDays(1), cancellationToken);
                var weekOrders = await _orderRepository.GetByDateRangeAsync(startOfWeek, today.AddDays(1), cancellationToken);
                var monthOrders = await _orderRepository.GetByDateRangeAsync(startOfMonth, today.AddDays(1), cancellationToken);
                var yearOrders = await _orderRepository.GetByDateRangeAsync(startOfYear, today.AddDays(1), cancellationToken);

                // Get orders by status
                var pendingOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Pending, cancellationToken);
                var processingOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Processing, cancellationToken);
                var completedOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Completed, cancellationToken);
                var cancelledOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Cancelled, cancellationToken);

                return new DashboardOrderSummaryDto
                {
                    TodayCount = todayOrders.Count(),
                    TodayRevenue = todayOrders.Sum(o => o.TotalAmount),

                    WeekCount = weekOrders.Count(),
                    WeekRevenue = weekOrders.Sum(o => o.TotalAmount),

                    MonthCount = monthOrders.Count(),
                    MonthRevenue = monthOrders.Sum(o => o.TotalAmount),

                    YearCount = yearOrders.Count(),
                    YearRevenue = yearOrders.Sum(o => o.TotalAmount),

                    PendingCount = pendingOrders.Count(),
                    PendingRevenue = pendingOrders.Sum(o => o.TotalAmount),

                    ProcessingCount = processingOrders.Count(),
                    ProcessingRevenue = processingOrders.Sum(o => o.TotalAmount),

                    CompletedCount = completedOrders.Count(),
                    CompletedRevenue = completedOrders.Sum(o => o.TotalAmount),

                    CancelledCount = cancelledOrders.Count(),
                    CancelledRevenue = cancelledOrders.Sum(o => o.TotalAmount),

                    // Average order value
                    AverageOrderValue = yearOrders.Any()
                        ? Math.Round(yearOrders.Average(o => o.TotalAmount), 2)
                        : 0
                };
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error getting order summary: {ex.Message}");

                // Return empty summary on error
                return new DashboardOrderSummaryDto();
            }
        }
    }
}