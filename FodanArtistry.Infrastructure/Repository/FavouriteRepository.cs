using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using FodanArtistry.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Infrastructure.Repositories
{
    public class FavouriteRepository : IFavoriteRepository
    {
        private readonly FodanArtistryDbContext _context;

        public FavouriteRepository(FodanArtistryDbContext context)
        {
            _context = context;
        }

        // ========== BASIC CRUD ==========

        public async Task<Favourite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .Include(f => f.Artwork)
                .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<Favourite> AddAsync(Favourite favourite, CancellationToken cancellationToken = default)
        {
            await _context.Favorites.AddAsync(favourite, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return favourite;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var favourite = await GetByIdAsync(id, cancellationToken);
            if (favourite != null)
            {
                _context.Favorites.Remove(favourite);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // ========== USER FAVOURITE METHODS ==========

        public async Task<IEnumerable<Favourite>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .Include(f => f.Artwork)
                .ThenInclude(a => a.Artist)
                .Include(f => f.Artwork.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Artwork>> GetUserFavoriteArtworksAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .Include(f => f.Artwork)
                .ThenInclude(a => a.Artist)
                .Include(f => f.Artwork.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedDate)
                .Select(f => f.Artwork)
                .ToListAsync(cancellationToken);
        }

        // ========== FAVOURITE CHECK METHODS ==========

        public async Task<bool> IsFavoritedAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.ArtworkId == artworkId, cancellationToken);
        }

        public async Task<int> GetFavoriteCountAsync(Guid artworkId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .CountAsync(f => f.ArtworkId == artworkId, cancellationToken);
        }


        public async Task<Favourite?> GetUserFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtworkId == artworkId, cancellationToken);
        }

        public async Task<int> GetUserFavoriteCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .CountAsync(f => f.UserId == userId, cancellationToken);
        }

        public async Task<bool> DeleteUserFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default)
        {
            var favourite = await GetUserFavoriteAsync(userId, artworkId, cancellationToken);
            if (favourite != null)
            {
                _context.Favorites.Remove(favourite);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Artwork>> GetMostFavoritedArtworksAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Favorites
                .Include(f => f.Artwork)
                .ThenInclude(a => a.Artist)
                .GroupBy(f => f.ArtworkId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.First().Artwork)
                .ToListAsync(cancellationToken);
        }
    }
}