using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : IBookingRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllWaitingListAsync()
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .Where(b => b.BookingStatus == BookingStatus.Waitinglist)
                .ToListAsync();
        }


        public async Task<List<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetConfirmedBookingsFromUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && b.BookingStatus == BookingStatus.Confirmed)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetWaitingListBookingsFromUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && b.BookingStatus == BookingStatus.Waitinglist)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetByEventIdAsync(int eventId)
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .Where(b => b.EventId == eventId)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Event!).ThenInclude(e => e.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public void UpdateRange(IEnumerable<Booking> bookings)
        {
            _context.Bookings.UpdateRange(bookings);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task CancelBookingsByEventIdAsync(int eventId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.EventId == eventId &&
                    (b.BookingStatus == BookingStatus.Confirmed || b.BookingStatus == BookingStatus.Waitinglist))
                .ToListAsync();

            foreach (var booking in bookings)
            {
                booking.BookingStatus = BookingStatus.Cancelled;
                booking.WaitingNumber = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetWaitingListByEventIdAsync(int eventId)
        {
            return await _context.Bookings
                .Where(b => b.EventId == eventId && b.BookingStatus == BookingStatus.Waitinglist)
                .OrderBy(b => b.WaitingNumber ?? int.MaxValue)
                .ThenBy(b => b.Id)
                .ToListAsync();
        }

        public async Task<int> GetConfirmedCountByEventIdAsync(int eventId)
        {
            return await _context.Bookings
                .CountAsync(b => b.EventId == eventId && b.BookingStatus == BookingStatus.Confirmed);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}