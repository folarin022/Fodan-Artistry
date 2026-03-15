using Microsoft.AspNetCore.Identity;

namespace FodanArtistry.Domain.Data
{
    public class ApplicationUser :IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;


        public ICollection<Artwork>? Artworks { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Favourite>? Favorites { get; set; }
    }
}
