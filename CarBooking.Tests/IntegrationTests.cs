using CarBooking.Api;
using Microsoft.EntityFrameworkCore;

namespace CarBooking.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task AddBookingAndGetBookings_ReturnsListWithBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new BookingDbContext(options);
            var bookingRepo = new BookingRepo(context);
            var bookingService = new BookingService(bookingRepo, 100, 5);

            var booking = new Booking
            {
                BookingNumber = 1,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                PickupDate = DateTime.Now.AddDays(1),
                PickupKm = 100
            };

            await bookingService.RegisterPickup(booking);
            var result = await bookingService.GetBookings(10, 1);

            Assert.NotNull(result);
            Assert.Contains(booking, result);
            Assert.Equal(1, result.First().BookingNumber);
            Assert.Equal(100, result.First().PickupKm);
        }

        [Fact]
        public async Task RegisterPickup_AddsBooking_WhenBookingIsValid()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "CarBookingTestDb")
                .Options;

            using (var context = new BookingDbContext(options))
            {
                var bookingRepo = new BookingRepo(context);
                var bookingService = new BookingService(bookingRepo, 100, 5);

                var booking = new Booking
                {
                    PickupDate = DateTime.Now.AddDays(1),
                    PickupKm = 100,
                    RegistrationNumber = "ABC123",
                    CustomerSsn = "197804060098"
                };

                await bookingService.RegisterPickup(booking);

                var addedBooking = await bookingRepo.GetBookingByIdAsync(booking.BookingNumber);
                Assert.NotNull(addedBooking);
                Assert.Equal(booking.RegistrationNumber, addedBooking.RegistrationNumber);
                Assert.Equal(booking.PickupKm, addedBooking.PickupKm);
            }
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_WhenValidBooking()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseInMemoryDatabase(databaseName: "CarBookingTestDb")
                .Options;

            using (var context = new BookingDbContext(options))
            {
                var bookingRepo = new BookingRepo(context);
                var bookingService = new BookingService(bookingRepo, 100, 5);

                var booking = new Booking
                {
                    PickupDate = DateTime.Now.AddDays(-2),
                    ReturnDate = null,
                    PickupKm = 100,
                    ReturnKm = 0,
                    RegistrationNumber = "ABC123",
                    CustomerSsn = "197804060098"
                };

                await bookingService.RegisterPickup(booking);

                var cost = await bookingService.RegisterReturn(booking.BookingNumber, DateTime.Now, 200);

                Assert.Equal(200, cost);

                var updatedBooking = await bookingRepo.GetBookingByIdAsync(booking.BookingNumber);
                Assert.NotNull(updatedBooking);
                Assert.Equal(200, updatedBooking.ReturnKm);
                Assert.NotNull(updatedBooking.ReturnDate);
            }
        }

    }
}
