using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
        Task<Order?> GetOrderWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalSalesAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<int> GetOrderCountAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken);
    }
}
