using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.DTOs.CategoryModel
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ArtworkCount { get; set; }
    }
}
