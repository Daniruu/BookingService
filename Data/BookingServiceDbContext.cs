using Microsoft.EntityFrameworkCore;
using BookingService.Models;

namespace BookingService.Data
{
    public class BookingServiceDbContext : DbContext
    {
        public BookingServiceDbContext(DbContextOptions<BookingServiceDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }
        public DbSet<BusinessImage> BusinessImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Business
            modelBuilder.Entity<Business>()
                .HasMany(b => b.Employees)
                .WithOne(e => e.Business)
                .HasForeignKey(e => e.BusinessId);

            modelBuilder.Entity<Business>()
                .HasMany(b => b.Services)
                .WithOne(s => s.Business)
                .HasForeignKey(s => s.BusinessId);

            modelBuilder.Entity<Business>()
                .OwnsOne(b => b.Address);

            modelBuilder.Entity<Business>()
                .HasMany(b => b.Images)
                .WithOne(i => i.Business)
                .HasForeignKey(i => i.BusinessId);

            // Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            // WorkingHours
            modelBuilder.Entity<WorkingHours>()
                .HasOne(wh => wh.Business)
                .WithMany(b => b.WorkingHours)
                .HasForeignKey(wh => wh.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkingHours>()
                .HasOne(wh => wh.Employee)
                .WithMany(e => e.WorkingHours)
                .HasForeignKey(wh => wh.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // CreatedAt
            modelBuilder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
