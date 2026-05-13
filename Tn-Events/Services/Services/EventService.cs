using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Request;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICategoryRepository _categoryRepository;

        public EventService(IEventRepository eventRepository, ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return EventMapper.ToDtoList(events);
        }

        public async Task<List<EventResponseDto>> GetAllEventsAdminAsync()
        {
            var events = await _eventRepository.GetAllAdminAsync();
            return EventMapper.ToDtoList(events);
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return null;
            return EventMapper.ToDto(ev);
        }

        public async Task CreateEventAsync(CreateEventRequestDto dto)
        {
            var ev = new Event
            {
                Id = 0,
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Capacity = dto.Capacity,
                IsCancelled = false,
                CategoryId = dto.CategoryId
            };

            await _eventRepository.CreateAsync(ev);
        }

        public async Task ToggleCancelEventAsync(int id)
        {
            await _eventRepository.ToggleCancelAsync(id);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        }
    }
}
