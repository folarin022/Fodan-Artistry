using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs.ArtworkDto
{
    public class CreateArtworkDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string ArtistId { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}
