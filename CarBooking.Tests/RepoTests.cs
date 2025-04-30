using CarBooking.Api;
using Microsoft.EntityFrameworkCore;

namespace CarBooking.Tests
{
    public class RepoTests
    {
        [Fact]
        public async Task GetBookingByIdAsync_ReturnsBooking_WhenBookingExists()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BookingDbContext(options);
            var repo = new BookingRepo(context);
            var booking = new Booking { BookingNumber = 1, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };
            await repo.AddBookingAsync(booking);

            var result = await repo.GetBookingByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result?.BookingNumber);
        }
        [Fact]
        public async Task GetBookingByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BookingDbContext(options);
            var repo = new BookingRepo(context);

            var result = await repo.GetBookingByIdAsync(999);

            Assert.Null(result);
        }
        [Fact]
        public async Task AddBookingAsync_AddsBooking_WhenBookingIsValid()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BookingDbContext(options);
            var repo = new BookingRepo(context);
            var booking = new Booking { BookingNumber = 1, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };

            await repo.AddBookingAsync(booking);

            var result = await repo.GetBookingByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("ABC123", result?.RegistrationNumber);
        }
        [Fact]
        public async Task UpdateBookingAsync_UpdatesBooking_WhenBookingExists()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BookingDbContext(options);
            var repo = new BookingRepo(context);
            var booking = new Booking { BookingNumber = 1, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };
            await repo.AddBookingAsync(booking);
            booking.RegistrationNumber = "XYZ789";

            await repo.UpdateBookingAsync(booking);

            var result = await repo.GetBookingByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal("XYZ789", result?.RegistrationNumber);
        }
        [Fact]
        public async Task DeleteBookingAsync_DeletesBooking_WhenBookingExists()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new BookingDbContext(options);
            var repo = new BookingRepo(context);
            var booking = new Booking { BookingNumber = 1, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };
            await repo.AddBookingAsync(booking);

            await repo.DeleteBookingAsync(1);

            var result = await repo.GetBookingByIdAsync(1);
            Assert.Null(result);
        }

    }
}
