using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Interfaces
{
    public interface IArtworkRepository
    {
        Task<Artwork?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Artwork>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Artwork> AddAsync(Artwork artwork, CancellationToken cancellationToken = default);
        Task UpdateAsync(Artwork artwork, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Artwork>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Artwork>> GetByArtistAsync(string artistId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Artwork>> GetAvailableArtworksAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Artwork>> SearchArtworksAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<(IEnumerable<Artwork> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? category = null,
            string? search = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> IsArtworkAvailableAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

