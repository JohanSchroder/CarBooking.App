namespace CarBooking.Api
{
    public interface IBookingService
    {
        Task RegisterPickup(Booking booking);
        Task<decimal> RegisterReturn(int bookingNumber, DateTime returnDate, int returnOdometer);
        Task<List<Booking>> GetBookings(int pageSize, int pageNumber);
    }
    public class BookingService : IBookingService
    {
        private readonly IBookingRepo _bookingRepository;
        private readonly decimal _baseDayRental;
        private readonly decimal _baseKmPrice;
        public BookingService(IBookingRepo bookingRepository, decimal baseDayRental, decimal baseKmPrice)
        {
            _bookingRepository = bookingRepository;

            _baseDayRental = baseDayRental;
            _baseKmPrice = baseKmPrice;
        }
        public async Task<List<Booking>> GetBookings(int pageSize, int pageNumber)
        {
            return await _bookingRepository.GetAllBookingsAsync(pageSize, pageNumber);
        }

        public async Task RegisterPickup(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException(nameof(booking));
            }
            if (booking.PickupDate == default)
            {
                throw new InvalidOperationException("Pickup date must be specified.");
            }
            if (booking.PickupKm == 0)
            {
                throw new InvalidOperationException("Pickup km must be specified.");
            }
            await _bookingRepository.AddBookingAsync(booking);
        }

        public async Task<decimal> RegisterReturn(int bookingNumber, DateTime returnDate, int returnOdometer)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingNumber);
            if (booking == null)
            {
                throw new InvalidOperationException("Booking not found.");
            }
            if (returnDate == default)
            {
                throw new InvalidOperationException("Return date must be specified.");
            }
            if (returnOdometer == 0)
            {
                throw new InvalidOperationException("Return km must be specified.");
            }
            if (returnDate < booking.PickupDate)
            {
                throw new InvalidOperationException("Return date cannot be earlier than pickup date.");
            }
            booking.ReturnDate = returnDate;
            booking.ReturnKm = returnOdometer;
            var cost = CalculateCost(booking);
            await _bookingRepository.UpdateBookingAsync(booking);

            return cost;
        }
        private decimal CalculateCost(Booking booking)
        {
            if (booking.ReturnDate == null)
            {
                throw new InvalidOperationException("Return date must be specified to calculate cost.");
            }
            if (booking.ReturnKm == null)
            {
                throw new InvalidOperationException("Return km must be specified to calculate cost.");
            }

            var numberOfDays = (booking.ReturnDate.Value - booking.PickupDate).Days;
            var numberOfKm = booking.ReturnKm.HasValue ? booking.ReturnKm.Value - booking.PickupKm : 0;

            switch (booking.CarCategory)
            {
                case CarCategory.Small:
                    return _baseDayRental * numberOfDays;
                case CarCategory.Medium:
                    return _baseDayRental * numberOfDays * 1.3m + (_baseKmPrice * numberOfKm);
                case CarCategory.Large:
                    return _baseDayRental * numberOfDays * 1.5m + (_baseKmPrice * numberOfKm);
                case CarCategory.SUV:
                    return _baseDayRental * numberOfDays * 1.5m + (_baseKmPrice * numberOfKm * 1.5m);
                case CarCategory.Minivan:
                    return _baseDayRental * numberOfDays * 1.7m + (_baseKmPrice * numberOfKm * 1.7m);
                default:
                    throw new InvalidOperationException("Unknown car category");
            }
        }
    }
}
