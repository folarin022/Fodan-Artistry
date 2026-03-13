using System;

namespace FodanArtistry.Application.DTOs.CategoryModel
{
    public class CategoryListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}