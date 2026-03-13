using System;
using System.ComponentModel.DataAnnotations;

namespace FodanArtistry.Application.DTOs.CategoryModel
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Category ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
    }
}