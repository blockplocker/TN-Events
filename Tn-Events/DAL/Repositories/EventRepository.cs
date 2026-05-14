using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Bookings)
                .Where(e => e.EndDate > DateTime.UtcNow)
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<List<Event>> GetAllAdminAsync()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Bookings)
                .OrderBy(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateAsync(Event ev)
        {
            _context.Events.Add(ev);
            await _context.SaveChangesAsync();
            return ev;
        }

        public async Task ToggleCancelAsync(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return;

            ev.IsCancelled = !ev.IsCancelled;
            await _context.SaveChangesAsync();
        }
    }
}
