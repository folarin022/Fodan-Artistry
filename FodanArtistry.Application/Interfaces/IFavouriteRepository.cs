using FodanArtistry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Favourite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Favourite>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<Favourite> AddAsync(Favourite favorite, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> IsFavoritedAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default);
        Task<int> GetFavoriteCountAsync(Guid artworkId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Artwork>> GetUserFavoriteArtworksAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default);
    }
}
