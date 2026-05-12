using DAL.Models;
using Services.Dto.Response;

namespace Services.Mappers
{
    public static class EventMapper
    {
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
