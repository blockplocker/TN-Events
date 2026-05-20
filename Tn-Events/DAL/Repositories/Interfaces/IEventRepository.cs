using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync();
        Task<List<Event>> GetAllAdminAsync();
        Task<List<Event>> GetUpcomingAsync(int count);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event ev);
        Task UpdateAsync(Event ev);
        Task ToggleCancelAsync(int id);
    }
}
