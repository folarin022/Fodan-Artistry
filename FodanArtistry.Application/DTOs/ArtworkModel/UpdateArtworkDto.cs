using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.DTOs.ArtworkModel
{
    public class UpdateArtworkDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Guid CategoryId { get; set; }
    }
}