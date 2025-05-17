using Microsoft.EntityFrameworkCore;
using TourismWebsite.Models;

namespace TourismWebsite.Data
{
    public class TourismContext : DbContext
    {
        public TourismContext(DbContextOptions<TourismContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Destination> Destinations { get; set; } = null!;
        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Decimal hassasiyeti ayarları
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Tour>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);
        }
    }
} 