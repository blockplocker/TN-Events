using DAL.Models;
using Services.Dto.Request;
using Services.Dto.Response;

namespace Services.Mappers
{
    public static class EventMapper
    {
        public static Event Map(CreateEventRequestDto dto)
        {
            return new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Address = dto.Address,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Capacity = dto.Capacity,
                IsCancelled = false,
                CategoryId = dto.CategoryId
            };
        }

        public static Event Map(Event e,UpdateEventRequestDto dto)
        {
            e.Title = dto.Title;
            e.Description = dto.Description;
            e.Address = dto.Address;
            e.StartDate = dto.StartDate;
            e.EndDate = dto.EndDate;
            e.Capacity = dto.Capacity;
            e.CategoryId = dto.CategoryId;

            return e;
            
        }

        public static EventResponseDto ToDto(Event e)
        {
            return new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Address = e.Address,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Capacity = e.Capacity,
                IsCancelled = e.IsCancelled,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.Name ?? string.Empty,
                BookedCount = e.Bookings?.Count(b => b.BookingStatus == BookingStatus.Confirmed) ?? 0
            };
        }

        public static List<EventResponseDto> ToDtoList(List<Event> events)
        {
            return events.Select(ToDto).ToList();
        }
    }
}
