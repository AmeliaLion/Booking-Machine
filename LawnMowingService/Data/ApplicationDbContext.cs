using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LawnMowingService.Models; // Ensure this is included

namespace LawnMowingService.Data
{
    public class ApplicationDbContext : IdentityDbContext<Customer>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Define DbSets for your entities
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Operator> Operators { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure this line is present

            // Configure Booking relationships
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust delete behavior as needed

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Operator)
                .WithMany(o => o.Bookings)
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Machine)
                .WithMany(m => m.Bookings)
                .HasForeignKey(b => b.MachineId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust delete behavior as needed

            // Configure composite key for IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }); // Define the composite key
            });

            // Configure AspNetRoles to use MySQL-compatible types
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("varchar(450)");
                entity.Property(e => e.Name).HasColumnType("varchar(256)");
                entity.Property(e => e.NormalizedName).HasColumnType("varchar(256)");
                entity.Property(e => e.ConcurrencyStamp).HasColumnType("varchar(255)"); // Change from nvarchar(max) to varchar(255)
            });
        }
    }
}
