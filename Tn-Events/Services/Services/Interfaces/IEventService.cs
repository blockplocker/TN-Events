using Services.Dto.Request;
using Services.Dto.Response;

namespace Services.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetAllEventsAsync();
        Task<List<EventResponseDto>> GetAllEventsAdminAsync();
        Task<List<EventResponseDto>> GetUpcomingEventsAsync(int count);
        Task<EventResponseDto?> GetEventByIdAsync(int id);
        Task CreateEventAsync(CreateEventRequestDto dto);
        Task<bool> UpdateEventAsync(UpdateEventRequestDto dto);
        Task ToggleCancelEventAsync(int id);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
