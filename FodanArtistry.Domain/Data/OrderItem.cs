using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Domain.Data
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid ArtworkId { get; set; }
        public Artwork? Artwork { get; set; }

        public string ArtworkTitle { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}
