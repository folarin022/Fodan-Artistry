using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.DTOs.FavouriteModel
{
    public class FavouriteDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public Guid ArtworkId { get; set; }
        public string ArtworkTitle { get; set; } = string.Empty;
        public string ArtworkImage { get; set; } = string.Empty;
        public decimal ArtworkPrice { get; set; }

        public DateTime AddedDate { get; set; }
        public string? Collection { get; set; }
    }
}
