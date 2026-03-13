using FodanArtistry.Application.DTOs;
using FodanArtistry.Application.DTOs.ArtworkDto;
using FodanArtistry.Application.DTOs.ArtworkModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.Services
{
    public class ArtworkService : IArtworkService
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountService _accountService;

        public ArtworkService(
            IArtworkRepository artworkRepository,
            ICategoryRepository categoryRepository,
            IAccountService accountService)
        {
            _artworkRepository = artworkRepository;
            _categoryRepository = categoryRepository;
            _accountService = accountService;
        }

        public async Task<ArtworkDto?> GetArtworkByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var artwork = await _artworkRepository.GetByIdAsync(id, cancellationToken);
            if (artwork == null)
                return null;

            return MapToArtworkDto(artwork);
        }
        public async Task<PagedResult<ArtworkDto>> GetGalleryAsync(
            int pageNumber = 1,
            int pageSize = 12,
            string? category = null,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var (items, totalCount) = await _artworkRepository.GetPagedAsync(
                    pageNumber, pageSize, category, search, cancellationToken);

                var artworkDtos = items.Select(MapToArtworkDto).ToList();

                return new PagedResult<ArtworkDto>
                {
                    Items = artworkDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Category = category,
                    SearchTerm = search
                };
            }
            catch (Exception)
            {
                return new PagedResult<ArtworkDto>
                {
                    Items = new List<ArtworkDto>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = 0,
                    TotalPages = 0,
                    Category = category,
                    SearchTerm = search
                };
            }
        }

        public async Task<IEnumerable<ArtworkDto>> GetArtistPortfolioAsync(string artistId, CancellationToken cancellationToken = default)
        {
            var artworks = await _artworkRepository.GetByArtistAsync(artistId, cancellationToken);
            return artworks.Select(MapToArtworkDto);
        }

        public async Task<ArtworkDto> CreateArtworkAsync(CreateArtworkDto dto, string artistId, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
                throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist");

            var artwork = new Artwork
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Price = dto.Price,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                ArtistId = artistId,
                CategoryId = dto.CategoryId
            };

            var created = await _artworkRepository.AddAsync(artwork, cancellationToken);
            return MapToArtworkDto(created);
        }

        public async Task<ArtworkDto> UpdateArtworkAsync(UpdateArtworkDto dto, CancellationToken cancellationToken = default)
        {
            var existing = await _artworkRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Artwork with ID {dto.Id} not found");

            if (dto.CategoryId != existing.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, cancellationToken);
                if (category == null)
                    throw new InvalidOperationException($"Category with ID {dto.CategoryId} does not exist");
            }

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.ImageUrl = dto.ImageUrl;
            existing.Price = dto.Price;
            existing.CategoryId = dto.CategoryId;

            await _artworkRepository.UpdateAsync(existing, cancellationToken);
            return MapToArtworkDto(existing);
        }

        public async Task<bool> DeleteArtworkAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await _artworkRepository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return false;

            await _artworkRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable, CancellationToken cancellationToken = default)
        {
            var artwork = await _artworkRepository.GetByIdAsync(id, cancellationToken);
            if (artwork == null)
                return false;

            artwork.IsAvailable = isAvailable;
            await _artworkRepository.UpdateAsync(artwork, cancellationToken);
            return true;
        }


        public async Task<IEnumerable<ArtworkDto>> GetAvailableArtworksAsync(CancellationToken cancellationToken = default)
        {
            var artworks = await _artworkRepository.GetAvailableArtworksAsync(cancellationToken);
            return artworks.Select(MapToArtworkDto);
        }

        public async Task<IEnumerable<ArtworkDto>> SearchArtworksAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ArtworkDto>();

            var artworks = await _artworkRepository.SearchArtworksAsync(searchTerm, cancellationToken);
            return artworks.Select(MapToArtworkDto);
        }

        public async Task<bool> ArtworkExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _artworkRepository.ExistsAsync(id, cancellationToken);
        }


        private ArtworkDto MapToArtworkDto(Artwork artwork)
        {
            return new ArtworkDto
            {
                Id = artwork.Id,
                Title = artwork.Title,
                Description = artwork.Description,
                ImageUrl = artwork.ImageUrl,
                Price = artwork.Price,
                IsAvailable = artwork.IsAvailable,
                CreatedAt = artwork.CreatedAt,
                ArtistId = artwork.ArtistId,
                ArtistName = artwork.Artist != null
                    ? $"{artwork.Artist.FirstName} {artwork.Artist.LastName}"
                    : "Unknown Artist",
                CategoryId = artwork.CategoryId,
                CategoryName = artwork.Category?.Name ?? "Uncategorized",
                FavoriteCount = artwork.FavoritedBy?.Count ?? 0
            };
        }
    }
}