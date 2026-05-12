using DAL.Repositories.Interfaces;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return EventMapper.ToDtoList(events);
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return null;
            return EventMapper.ToDto(ev);
        }
    }
}
