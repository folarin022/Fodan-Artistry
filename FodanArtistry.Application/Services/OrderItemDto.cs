namespace FodanArtistry.Application.Services
{
    internal class OrderItemDto
    {
        public Guid ArtworkId { get; set; }
        public string ArtworkTitle { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}