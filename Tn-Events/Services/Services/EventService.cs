using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Request;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class EventService(IEventRepository eventRepository, ICategoryRepository categoryRepository, IBookingRepository bookingRepository) : IEventService
    {
        private readonly IEventRepository _eventRepository = eventRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        public async Task<List<EventResponseDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return EventMapper.ToDtoList(events);
        }

        public async Task<List<EventResponseDto>> GetUpcomingEventsAsync(int count)
        {
            var events = await _eventRepository.GetUpcomingAsync(count);
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

        public async Task<bool> UpdateEventAsync(UpdateEventRequestDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(dto.Id);
            if (ev == null) return false;

            var oldCapacity = ev.Capacity;
            var updatedEvent = EventMapper.Map(ev, dto);
            await _eventRepository.UpdateAsync(updatedEvent);

            if (dto.Capacity > oldCapacity)
            {
                await MoveFromWaitlistAsync(dto.Id, dto.Capacity);
            }

            return true;
        }

        public async Task ToggleCancelEventAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return;

            bool willBeCancelled = !ev.IsCancelled;
            await _eventRepository.ToggleCancelAsync(id);

            if (willBeCancelled)
            {
                await _bookingRepository.CancelBookingsByEventIdAsync(id);
            }
        }

        private async Task MoveFromWaitlistAsync(int eventId, int newCapacity)
        {
            var confirmedCount = await _bookingRepository.GetConfirmedCountByEventIdAsync(eventId);
            var availableSlots = newCapacity - confirmedCount;

            if (availableSlots <= 0) return;

            var waitingList = await _bookingRepository.GetWaitingListByEventIdAsync(eventId);

            var toPromote = waitingList.Take(availableSlots).ToList();
            var remaining = waitingList.Skip(availableSlots).ToList();

            foreach (var booking in toPromote)
            {
                booking.BookingStatus = BookingStatus.Confirmed;
                booking.WaitingNumber = null;
            }

            for (var i = 0; i < remaining.Count; i++)
            {
                remaining[i].WaitingNumber = i + 1;
            }

            _bookingRepository.UpdateRange(toPromote.Concat(remaining));
            await _bookingRepository.SaveChangesAsync();
        }
    }

}
