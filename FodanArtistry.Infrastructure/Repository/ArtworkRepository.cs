using FodanArtistry.Application.Interfaces;
using FodanArtistry.Domain.Data;

namespace FodanArtistry.Infrastructure.Repository
{
    public class ArtworkRepository : IArtworkRepository
    {
        public Task<Artwork> AddAsync(Artwork artwork, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> GetAvailableArtworksAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> GetByArtistAsync(string artistId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Artwork?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<Artwork> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? category = null, string? search = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsArtworkAvailableAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Artwork>> SearchArtworksAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Artwork artwork, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}