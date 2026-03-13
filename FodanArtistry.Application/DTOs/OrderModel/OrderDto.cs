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
    }
}
