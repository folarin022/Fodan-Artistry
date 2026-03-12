using FodanArtistry.Domain.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FodanArtistry.Infrastructure.Context
{
    public class FodanArtistryDbContext : IdentityDbContext<ApplicationUser>
    {
        public FodanArtistryDbContext(DbContextOptions<FodanArtistryDbContext> options)
            : base(options)
        {
        }

        // DbSets for all your entities
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Favourite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Artwork>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Artworks)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Artwork>()
                .HasOne(a => a.Artist)
                .WithMany(u => u.Artworks)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); 


            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Artwork)
                .WithMany() 
                .HasForeignKey(oi => oi.ArtworkId)
                .OnDelete(DeleteBehavior.Restrict); 


            builder.Entity<Favourite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<Favourite>()
                .HasOne(f => f.Artwork)
                .WithMany(a => a.FavoritedBy)
                .HasForeignKey(f => f.ArtworkId)
                .OnDelete(DeleteBehavior.Cascade); 


            builder.Entity<Favourite>()
                .HasIndex(f => new { f.UserId, f.ArtworkId })
                .IsUnique();

            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<Artwork>()
                .Property(a => a.Price)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<Artwork>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Order>()
                .Property(o => o.OrderDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Favourite>()
                .Property(f => f.AddedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue(OrderStatus.Pending);
        }
    }
}