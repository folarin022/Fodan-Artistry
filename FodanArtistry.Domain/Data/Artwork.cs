namespace FodanArtistry.Domain.Data
{
    public class Artwork
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public string ArtistId { get; set; } = string.Empty;
        public ApplicationUser? Artist { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Favourite>? FavoritedBy { get; set; }
    }
}
