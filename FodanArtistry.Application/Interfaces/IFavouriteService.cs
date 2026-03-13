using FodanArtistry.Application.DTOs.ArtworkModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FodanArtistry.Application.Interfaces
{
    public interface IFavouriteService
    {
        Task<bool> AddToFavoritesAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default);
        Task<bool> RemoveFromFavoritesAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ArtworkDto>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> IsFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken = default);
        Task<int> GetFavoriteCountAsync(Guid artworkId, CancellationToken cancellationToken = default);
        Task ToggleFavoriteAsync(string userId, Guid artworkId, CancellationToken cancellationToken);
    }
}