using System;

namespace FodanArtistry.Application.DTOs.CategoryModel
{
    public class CategoryWithCountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ArtworkCount { get; set; }
        public int AvailableArtworkCount { get; set; }  
        public string? CoverImageUrl { get; set; }    
        public DateTime? LatestArtworkDate { get; set; }
    }
}