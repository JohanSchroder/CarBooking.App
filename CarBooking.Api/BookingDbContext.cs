using Microsoft.EntityFrameworkCore;

namespace CarBooking.Api
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options)
        {
        }
        public DbSet<Booking> Bookings { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingNumber);
                entity.Property(e => e.BookingNumber)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RegistrationNumber)
                    .IsRequired();
                entity.Property(e => e.CustomerSsn)
                    .IsRequired();
                entity.Property(e => e.CarCategory)
                    .IsRequired();
                entity.Property(e => e.PickupDate)
                    .IsRequired();
                entity.Property(e => e.PickupKm)
                    .IsRequired();
                entity.Property(e => e.ReturnDate);
                entity.Property(e => e.ReturnKm);
            });
        }
    }
}
