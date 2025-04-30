using CarBooking.Api;
using Moq;

namespace CarBooking.Tests
{
    public class ServiceTests
    {
        [Fact]
        public async Task RegisterPickup_DoesNotThrowException_WhenValidBooking()
        {
            var bookingService = new BookingService(new Mock<IBookingRepo>().Object, 100, 5);
            var validBooking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(1),
                PickupKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098"
            };
            var exception = await Record.ExceptionAsync(() => bookingService.RegisterPickup(validBooking));
            Assert.Null(exception);
        }

        [Fact]
        public async Task RegisterPickup_ThrowsException_WhenBookingIsNull()
        {
            var bookingService = new BookingService(new Mock<IBookingRepo>().Object, 100, 5);
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookingService.RegisterPickup(null));
        }
        [Fact]
        public async Task RegisterPickup_ThrowsException_WhenPickupDateIsDefault()
        {
            var bookingService = new BookingService(new Mock<IBookingRepo>().Object, 100, 5);
            var booking = new Booking { PickupDate = default, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterPickup(booking));
        }
        [Fact]
        public async Task RegisterPickup_ThrowsException_WhenPickupKmIsZero()
        {
            var bookingService = new BookingService(new Mock<IBookingRepo>().Object, 100, 5);
            var booking = new Booking { PickupKm = 0, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" };
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterPickup(booking));
        }
        [Fact]
        public async Task RegisterReturn_ThrowsException_WhenBookingNotFound()
        {
            var bookingService = new BookingService(new Mock<IBookingRepo>().Object, 100, 5);
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterReturn(999, DateTime.Now, 100));
        }
        [Fact]
        public async Task RegisterReturn_ThrowsException_WhenReturnDateIsNull()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Booking { ReturnDate = null, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" });
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterReturn(1, default, 100));
        }
        [Fact]
        public async Task RegisterReturn_ThrowsException_WhenReturnKmIsNull()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Booking { ReturnKm = null, RegistrationNumber = "ABC123", CustomerSsn = "197804060098" });
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterReturn(1, DateTime.Now, 0));
        }
        [Fact]
        public async Task RegisterReturn_ThrowsException_WhenReturnDateIsBeforePickupDate()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(1),
                ReturnDate = null,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098"
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            await Assert.ThrowsAsync<InvalidOperationException>(() => bookingService.RegisterReturn(1, DateTime.Now, 100));
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_CarCategory_Small()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(-2),
                ReturnDate = DateTime.Now,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                CarCategory = CarCategory.Small
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            var cost = await bookingService.RegisterReturn(1, DateTime.Now, 100);
            Assert.Equal(200.00m, cost);
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_CarCategory_Medium()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(-2),
                ReturnDate = DateTime.Now,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                CarCategory = CarCategory.Medium
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            var cost = await bookingService.RegisterReturn(1, DateTime.Now, 100);
            Assert.Equal(760.00m, cost);
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_CarCategory_Large()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(-2),
                ReturnDate = DateTime.Now,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                CarCategory = CarCategory.Large
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            var cost = await bookingService.RegisterReturn(1, DateTime.Now, 100);
            Assert.Equal(800, cost);
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_CarCategory_Van()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(-2),
                ReturnDate = DateTime.Now,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                CarCategory = CarCategory.SUV
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            var cost = await bookingService.RegisterReturn(1, DateTime.Now, 100);
            Assert.Equal(1050, cost);
        }
        [Fact]
        public async Task RegisterReturn_CalculatesCostCorrectly_CarCategory_Truck()
        {
            var bookingRepoMock = new Mock<IBookingRepo>();
            var booking = new Booking
            {
                PickupDate = DateTime.Now.AddDays(-2),
                ReturnDate = DateTime.Now,
                PickupKm = 0,
                ReturnKm = 100,
                RegistrationNumber = "ABC123",
                CustomerSsn = "197804060098",
                CarCategory = CarCategory.Minivan
            };
            bookingRepoMock.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(booking);
            var bookingService = new BookingService(bookingRepoMock.Object, 100, 5);
            var cost = await bookingService.RegisterReturn(1, DateTime.Now, 100);
            Assert.Equal(1190, cost);
        }
    }
}
