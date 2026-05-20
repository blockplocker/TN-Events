using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Request;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository) : IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IEventRepository _eventRepository = eventRepository;

        public async Task<List<BookingResponseDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return BookingMapper.ToDtoList(bookings);
        }

        public async Task<List<BookingResponseDto>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return BookingMapper.ToDtoList(bookings);
        }

        public async Task<List<BookingResponseDto>> GetEventBookingsAsync(int eventId)
        {
            var bookings = await _bookingRepository.GetByEventIdAsync(eventId);
            return BookingMapper.ToDtoList(bookings);
        }

        public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking == null ? null : BookingMapper.ToDto(booking);
        }

        public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingRequestDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(dto.EventId)
                ?? throw new InvalidOperationException("Event not found.");

            if (ev.IsCancelled)
            {
                throw new InvalidOperationException("Cannot book a cancelled event.");
            }

            var existingBookings = await _bookingRepository.GetByEventIdAsync(dto.EventId);
            var confirmedCount = existingBookings.Count(b => b.BookingStatus == BookingStatus.Confirmed);
            var status = confirmedCount < ev.Capacity ? BookingStatus.Confirmed : BookingStatus.Waitinglist;

            int? waitingNumber = null;
            if (status == BookingStatus.Waitinglist)
            {
                waitingNumber = existingBookings.Count(b => b.BookingStatus == BookingStatus.Waitinglist) + 1;
            }

            var booking = await _bookingRepository.CreateAsync(BookingMapper.Map(dto, status, waitingNumber));
            return BookingMapper.ToDto(booking);
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            booking.BookingStatus = status;
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }
    }
}