//using FodanArtistry.Application.DTOs.ArtworkDto;
//using FodanArtistry.Application.DTOs.ArtworkModel;

//namespace FodanArtistry.Application.Interfaces
//{
//    public interface IArtworkService
//    {
//        Task<ArtworkDto?> GetArtworkByIdAsync(Guid id, CancellationToken cancellationToken = default);
//        Task<PagedResult<ArtworkDto>> GetGalleryAsync(
//            int pageNumber = 1,
//            int pageSize = 12,
//            string? category = null,
//            string? search = null,
//            CancellationToken cancellationToken = default);

//        Task<IEnumerable<ArtworkDto>> GetArtistPortfolioAsync(string artistId, CancellationToken cancellationToken = default);

//        Task<ArtworkDto> CreateArtworkAsync(CreateArtworkDto dto, string artistId, CancellationToken cancellationToken = default);
//        Task<ArtworkDto> UpdateArtworkAsync(UpdateArtworkDto dto, CancellationToken cancellationToken = default);
//        Task<bool> DeleteArtworkAsync(Guid id, CancellationToken cancellationToken = default);
//        Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable, CancellationToken cancellationToken = default);
//    }
//}
