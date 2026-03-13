using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;
using FodanArtistry.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FodanArtistryDbContext _context;

        public CategoryRepository(FodanArtistryDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return category;
        }

        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await GetByIdAsync(id, cancellationToken);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithArtworksAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.Artworks)
                .Where(c => c.Artworks != null && c.Artworks.Any())
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetArtworkCountAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Artworks
                .CountAsync(a => a.CategoryId == categoryId, cancellationToken);
        }


        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}