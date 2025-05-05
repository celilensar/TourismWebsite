using Microsoft.EntityFrameworkCore;
using TourismWebsite.Models;

namespace TourismWebsite.Data
{
    public class TourismContext : DbContext
    {
        public TourismContext(DbContextOptions<TourismContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Tour> Tours { get; set; }

        // İleride modeller arasında daha detaylı yapılandırma (fluent API) 
        // yapmak istersek OnModelCreating metodunu override edebiliriz.
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //     // Örneğin: modelBuilder.Entity<Tour>().Property(t => t.Price).HasColumnType("decimal(18,2)");
        // }
    }
} 