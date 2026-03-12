//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace FodanArtistry.Application.Interfaces
//{
//    public interface ICategoryService
//    {
//        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
//        Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
//        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
//        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default);
//        Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
//        Task<IEnumerable<CategoryWithCountDto>> GetCategoriesWithArtworkCountsAsync(CancellationToken cancellationToken = default);
//    }
//}
