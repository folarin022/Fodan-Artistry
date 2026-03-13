using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.DTOs.OrderModel
{
    public class CreateOrderDto
    {
        public string? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();

        
    }
    
}