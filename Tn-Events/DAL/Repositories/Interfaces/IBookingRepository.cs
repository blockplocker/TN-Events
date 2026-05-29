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
        Task<List<Booking>> GetConfirmedBookingsFromUserIdAsync(string userId);
        Task<List<Booking>> GetWaitingListBookingsFromUserIdAsync(string userId);
        Task<Booking> CreateAsync(Booking booking);
        void UpdateRange(IEnumerable<Booking> bookings);
        Task SaveChangesAsync();
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
        Task CancelBookingsByEventIdAsync(int eventId);
        Task<List<Booking>> GetWaitingListByEventIdAsync(int eventId);
        Task<int> GetConfirmedCountByEventIdAsync(int eventId);
    }
}
