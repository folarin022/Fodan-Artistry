using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetOrderCountAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetOrderWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalSalesAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
