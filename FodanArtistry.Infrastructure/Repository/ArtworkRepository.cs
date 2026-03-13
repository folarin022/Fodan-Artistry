using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using FodanArtistry.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Infrastructure.Repository
{
    public class ArtworkRepository : IArtworkRepository
    {
        private readonly FodanArtistryDbContext _context;

        public ArtworkRepository(FodanArtistryDbContext context)
        {
            _context = context;
        }
        public async Task<Artwork> AddAsync(Artwork artwork, CancellationToken cancellationToken = default)
        {
            await _context.Artworks.AddAsync(artwork, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return artwork;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var artwork = await GetByIdAsync(id, cancellationToken);
            if (artwork != null)
            {
                _context.Artworks.Remove(artwork);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks.AnyAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Artwork>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Artwork>> GetAvailableArtworksAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .Where(a => a.IsAvailable)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Artwork>> GetByArtistAsync(string artistId, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Category)
                .Where(a => a.ArtistId == artistId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Artwork>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Artist)
                .Where(a => a.CategoryId == categoryId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Artwork?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<(IEnumerable<Artwork> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? category = null,
            string? search = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(a => a.Category.Name == category);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a => a.Title.Contains(search) ||
                                         a.Description.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<bool> IsArtworkAvailableAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var artwork = await _context.Artworks
                .Where(a => a.Id == id)
                .Select(a => a.IsAvailable)
                .FirstOrDefaultAsync(cancellationToken);

            return artwork;
        }

        public async Task<IEnumerable<Artwork>> SearchArtworksAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .Include(a => a.Artist)
                .Include(a => a.Category)
                .Where(a => a.Title.Contains(searchTerm) ||
                            a.Description.Contains(searchTerm) ||
                            a.Artist.FirstName.Contains(searchTerm) ||
                            a.Artist.LastName.Contains(searchTerm))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Artwork artwork, CancellationToken cancellationToken = default)
        {
            _context.Artworks.Update(artwork);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}