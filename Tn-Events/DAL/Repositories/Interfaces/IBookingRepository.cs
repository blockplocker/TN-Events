using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetAllWaitingListAsync();
        Task<List<Booking>> GetByUserIdAsync(string userId);
        Task<List<Booking>> GetByEventIdAsync(int eventId);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking> CreateAsync(Booking booking);
        void UpdateRange(IEnumerable<Booking> bookings);
        Task SaveChangesAsync();
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}