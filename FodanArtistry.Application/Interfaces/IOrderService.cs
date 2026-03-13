using FodanArtistry.Application.DTOs.OrderModel;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string? userId = null, CancellationToken cancellationToken = default);
        Task<OrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
        Task<OrderDto> UpdateOrderStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default);
        Task<bool> CancelOrderAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<RecentOrderDto>> GetRecentOrdersAsync(int count = 10, CancellationToken cancellationToken = default);
    }
}
