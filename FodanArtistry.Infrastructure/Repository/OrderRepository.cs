using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using FodanArtistry.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FodanArtistryDbContext _context;

        public OrderRepository(FodanArtistryDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<Order?> GetOrderWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await GetByIdAsync(id, cancellationToken);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


        public async Task<IEnumerable<Order>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Artwork)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }


        public async Task<int> GetOrderCountAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .CountAsync(o => o.OrderDate >= start && o.OrderDate <= end, cancellationToken);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .SumAsync(o => o.TotalAmount, cancellationToken);
        }


        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders.CountAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount, cancellationToken);
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrderStatusBreakdownAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Pending, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetProcessingOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Processing, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetCompletedOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Completed, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetCancelledOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Cancelled, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetShippedOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Shipped, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetDeliveredOrdersAsync(CancellationToken cancellationToken = default)
        {
            return await GetByStatusAsync(OrderStatus.Delivered, cancellationToken);
        }

        public async Task<int> GetUserOrderCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .CountAsync(o => o.UserId == userId, cancellationToken);
        }

        public async Task<decimal> GetUserTotalSpentAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .SumAsync(o => o.TotalAmount, cancellationToken);
        }
    }
}