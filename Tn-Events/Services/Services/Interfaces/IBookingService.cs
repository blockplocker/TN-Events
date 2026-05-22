using DAL.Models;
using Services.Dto.Request;
using Services.Dto.Response;

namespace Services.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponseDto>> GetAllBookingsAsync();
        Task<List<BookingResponseDto>> GetAllWaitingListBookingsAsync();
        Task<List<BookingResponseDto>> GetUserBookingsAsync(string userId);
        Task<List<BookingResponseDto>> GetEventBookingsAsync(int eventId);
        Task<BookingResponseDto?> GetBookingByIdAsync(int id);
        Task<BookingResponseDto> CreateBookingAsync(CreateBookingRequestDto dto);
        Task<bool> CancelBookingAsync(int id);
    }
}