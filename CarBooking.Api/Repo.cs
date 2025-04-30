using Microsoft.EntityFrameworkCore;

namespace CarBooking.Api
{
    public interface IBookingRepo
    {
        Task<Booking?> GetBookingByIdAsync(int bookingNumber);
        Task<List<Booking>> GetAllBookingsAsync(int pageSize, int pageNumber);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int bookingNumber);
    }
    public class BookingRepo : IBookingRepo
    {
        private readonly BookingDbContext _dbContext;
        public BookingRepo(BookingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Booking?> GetBookingByIdAsync(int bookingNumber)
        {
            return await _dbContext.Bookings.FindAsync(bookingNumber);
        }
        public async Task<List<Booking>> GetAllBookingsAsync(int pageSize, int pageNumber)
        {
            return await _dbContext.Bookings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task AddBookingAsync(Booking booking)
        {
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateBookingAsync(Booking booking)
        {
            _dbContext.Bookings.Update(booking);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteBookingAsync(int bookingNumber)
        {
            var booking = await GetBookingByIdAsync(bookingNumber);
            if (booking != null)
            {
                _dbContext.Bookings.Remove(booking);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
