using FodanArtistry.Application.DTOs.ArtworkModel;
using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Application.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly IAccountService _accountService;

        public FavouriteService(
            IFavoriteRepository favoriteRepository,
            IArtworkRepository artworkRepository,
            IAccountService accountService)
        {
            _favoriteRepository = favoriteRepository;
            _artworkRepository = artworkRepository;
            _accountService = accountService;
        }

        // ========== ADD TO FAVORITES ==========
        public async Task<bool> AddToFavoritesAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if user exists
                var user = await _accountService.GetUserProfileAsync(userId, cancellationToken);
                if (user == null)
                    throw new InvalidOperationException("User not found");

                // Check if artwork exists
                var artwork = await _artworkRepository.GetByIdAsync(artworkId, cancellationToken);
                if (artwork == null)
                    throw new InvalidOperationException("Artwork not found");

                // Check if already favorited
                var isFavorited = await _favoriteRepository.IsFavoritedAsync(userId, artworkId, cancellationToken);
                if (isFavorited)
                    return true; // Already favorited, return true

                // Create new favorite
                var favorite = new Favourite
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ArtworkId = artworkId,
                    AddedDate = DateTime.UtcNow
                };

                await _favoriteRepository.AddAsync(favorite, cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ========== REMOVE FROM FAVORITES ==========
        public async Task<bool> RemoveFromFavoritesAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if favorited
                var isFavorited = await _favoriteRepository.IsFavoritedAsync(userId, artworkId, cancellationToken);
                if (!isFavorited)
                    return true; // Already not favorited, return true

                // Delete the favorite
                var result = await _favoriteRepository.DeleteUserFavoriteAsync(userId, artworkId, cancellationToken);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ========== GET USER FAVORITES ==========
        public async Task<IEnumerable<ArtworkDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var favoriteArtworks = await _favoriteRepository.GetUserFavoriteArtworksAsync(userId, cancellationToken);
                
                return favoriteArtworks.Select(a => new ArtworkDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                    IsAvailable = a.IsAvailable,
                    CreatedAt = a.CreatedAt,
                    ArtistId = a.ArtistId,
                    ArtistName = a.Artist != null 
                        ? $"{a.Artist.FirstName} {a.Artist.LastName}" 
                        : "Unknown Artist",
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category?.Name ?? "Uncategorized",
                    FavoriteCount = a.FavoritedBy?.Count ?? 0
                }).ToList();
            }
            catch (Exception)
            {
                return new List<ArtworkDto>();
            }
        }

        // ========== CHECK IF ARTWORK IS FAVORITED ==========
        public async Task<bool> IsFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _favoriteRepository.IsFavoritedAsync(userId, artworkId, cancellationToken);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ========== GET FAVORITE COUNT FOR ARTWORK ==========
        public async Task<int> GetFavoriteCountAsync(Guid artworkId, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _favoriteRepository.GetFavoriteCountAsync(artworkId, cancellationToken);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // ========== ADDITIONAL HELPER METHODS ==========

        public async Task<int> GetUserFavoriteCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var favorites = await _favoriteRepository.GetByUserAsync(userId, cancellationToken);
                return favorites.Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<ArtworkDto>> GetMostFavoritedArtworksAsync(string count, CancellationToken cancellationToken = default)
        {
            try
            {
                var artworks = await _favoriteRepository.GetUserFavoriteArtworksAsync(count, cancellationToken);
                
                return artworks.Select(a => new ArtworkDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                    IsAvailable = a.IsAvailable,
                    CreatedAt = a.CreatedAt,
                    ArtistId = a.ArtistId,
                    ArtistName = a.Artist != null 
                        ? $"{a.Artist.FirstName} {a.Artist.LastName}" 
                        : "Unknown Artist",
                    CategoryId = a.CategoryId,
                    CategoryName = a.Category?.Name ?? "Uncategorized",
                    FavoriteCount = a.FavoritedBy?.Count ?? 0
                }).ToList();
            }
            catch (Exception)
            {
                return new List<ArtworkDto>();
            }
        }

        public async Task<Dictionary<Guid, int>> GetMultipleFavoriteCountsAsync(List<Guid> artworkIds, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<Guid, int>();
            
            foreach (var artworkId in artworkIds)
            {
                var count = await GetFavoriteCountAsync(artworkId, cancellationToken);
                result[artworkId] = count;
            }
            
            return result;
        }

        public async Task<List<Guid>> GetUserFavoriteIdsAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var favorites = await _favoriteRepository.GetByUserAsync(userId, cancellationToken);
                return favorites.Select(f => f.ArtworkId).ToList();
            }
            catch (Exception)
            {
                return new List<Guid>();
            }
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            var isFavorited = await IsFavoriteAsync(userId, artworkId, cancellationToken);
            
            if (isFavorited)
                return await RemoveFromFavoritesAsync(userId, artworkId, cancellationToken);
            else
                return await AddToFavoritesAsync(userId, artworkId, cancellationToken);
        }

        public async Task<int> ClearAllUserFavoritesAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var favorites = await _favoriteRepository.GetByUserAsync(userId, cancellationToken);
                var count = favorites.Count();
                
                foreach (var favorite in favorites)
                {
                    await _favoriteRepository.DeleteAsync(favorite.Id, cancellationToken);
                }
                
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        Task IFavouriteService.ToggleFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken)
        {
            return ToggleFavoriteAsync(userId, artworkId, cancellationToken);
        }
    }
}