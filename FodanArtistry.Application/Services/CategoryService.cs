using FodanArtistry.Application.DTOs.CategoryModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArtworkRepository _artworkRepository;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IArtworkRepository artworkRepository)
        {
            _categoryRepository = categoryRepository;
            _artworkRepository = artworkRepository;
        }


        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(cancellationToken);

                return categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description ?? string.Empty,
                    ArtworkCount = c.Artworks?.Count ?? 0
                }).OrderBy(c => c.Name);
            }
            catch (Exception)
            {
                return new List<CategoryDto>();
            }
        }


        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                ArtworkCount = category.Artworks?.Count ?? 0
            };
        }


        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
        {


            var existing = await _categoryRepository.GetByNameAsync(dto.Name, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException($"Category '{dto.Name}' already exists");

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim()
            };

            var created = await _categoryRepository.AddAsync(category, cancellationToken);

            return new CategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description ?? string.Empty,
                ArtworkCount = 0
            };
        }


        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {dto.Id} not found");


            if (category.Name != dto.Name.Trim())
            {
                var existing = await _categoryRepository.GetByNameAsync(dto.Name, cancellationToken);
                if (existing != null)
                    throw new InvalidOperationException($"Category '{dto.Name}' already exists");
            }

            category.Name = dto.Name.Trim();
            category.Description = dto.Description?.Trim();

            await _categoryRepository.UpdateAsync(category, cancellationToken);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description ?? string.Empty,
                ArtworkCount = category.Artworks?.Count ?? 0
            };
        }


        public async Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
                return false;


            var artworkCount = await _categoryRepository.GetArtworkCountAsync(id, cancellationToken);
            if (artworkCount > 0)
                throw new InvalidOperationException($"Cannot delete category '{category.Name}' because it has {artworkCount} artwork(s). Please reassign or delete the artworks first.");

            await _categoryRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<CategoryWithCountDto>> GetCategoriesWithArtworkCountsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync(cancellationToken);
                var result = new List<CategoryWithCountDto>();

                foreach (var category in categories)
                {
                    var artworkCount = category.Artworks?.Count ?? 0;

                    var availableCount = category.Artworks?
                        .Count(a => a.IsAvailable) ?? 0;

                    var coverImage = category.Artworks?
                        .FirstOrDefault()?.ImageUrl;

                    var latestDate = category.Artworks?
                        .OrderByDescending(a => a.CreatedAt)
                        .FirstOrDefault()?.CreatedAt;

                    result.Add(new CategoryWithCountDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description ?? string.Empty,
                        ArtworkCount = artworkCount,
                        AvailableArtworkCount = availableCount,
                        CoverImageUrl = coverImage,
                        LatestArtworkDate = latestDate
                    });
                }

                return result.OrderByDescending(c => c.ArtworkCount);
            }
            catch (Exception)
            {
                return new List<CategoryWithCountDto>();
            }
        }


        public async Task<IEnumerable<CategoryListDto>> GetCategoryListAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);

            return categories.Select(c => new CategoryListDto
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c => c.Name);
        }
        public async Task<Dictionary<string, int>> GetCategoryStatsAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);

            return categories.ToDictionary(
                c => c.Name,
                c => c.Artworks?.Count ?? 0
            );
        }
    }
}