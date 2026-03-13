using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs.OrderModel
{
    public class RecentOrderDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public List<string> ArtworkTitles { get; set; } = new();
        public string CustomerEmail { get; internal set; }
    }
}
