using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Request;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class EventService(IEventRepository eventRepository, ICategoryRepository categoryRepository) : IEventService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
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
            await _eventRepository.CreateAsync(EventMapper.Map(dto));
        }

        public async Task UpdateEventAsync(UpdateEventRequestDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(dto.Id);
            if (ev == null) return;

            var updatedEvent = EventMapper.Map(ev, dto);
            await _eventRepository.UpdateAsync(updatedEvent);
        }

        public async Task ToggleCancelEventAsync(int id)
        {
            await _eventRepository.ToggleCancelAsync(id);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return CategoryMapper.ToDtoList(categories);
        }
    }
}
