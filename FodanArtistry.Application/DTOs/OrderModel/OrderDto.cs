using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs.OrderModel
{
    public class OrderDto
    {
        public Guid ArtworkId { get; set; }
        public string ArtworkTitle { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public Guid Id { get; internal set; }
        public string OrderNumber { get; internal set; }
        public string CustomerName { get; internal set; }
        public string CustomerEmail { get; internal set; }
        public string CustomerPhone { get; internal set; }
        public string StatusColor { get; internal set; }
        public string ShippingAddress { get; internal set; }
        public DateTime OrderDate { get; internal set; }
        public decimal TotalAmount { get; internal set; }
        public string Status { get; internal set; }
        public List<OrderItemDto> Items { get; internal set; }
        public int ItemCount { get; internal set; }
    }
}
