using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Domain.Data
{
    public class Favourite
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public Guid ArtworkId { get; set; }
        public Artwork? Artwork { get; set; }
        public DateTime AddedDate { get; set; }
        public string? Collection { get; set; }
    }
}
