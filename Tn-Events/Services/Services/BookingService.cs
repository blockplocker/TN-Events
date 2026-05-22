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

        public async Task<List<BookingResponseDto>> GetAllWaitingListBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllWaitingListAsync();
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
            return await _bookingRepository.ExecuteInTransactionAsync(async () =>
            {
                var ev = await _eventRepository.GetByIdAsync(dto.EventId)
                    ?? throw new InvalidOperationException("Event not found.");

                if (ev.IsCancelled)
                {
                    throw new InvalidOperationException("Cannot book a cancelled event.");
                }

                var userBookings = await _bookingRepository.GetByUserIdAsync(dto.UserId);
                var existingActiveBooking = userBookings.FirstOrDefault(b =>
                    b.EventId == dto.EventId &&
                    b.BookingStatus != BookingStatus.Cancelled);

                if (existingActiveBooking != null)
                {
                    throw new InvalidOperationException("You already have an active booking for this event.");
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
            });
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return false;

            var previousStatus = booking.BookingStatus;
            if (previousStatus != BookingStatus.Confirmed && previousStatus != BookingStatus.Waitinglist)
            {
                return false;
            }

            var eventBookings = await _bookingRepository.GetByEventIdAsync(booking.EventId);
            var waitingListBookings = eventBookings
                .Where(b => b.BookingStatus == BookingStatus.Waitinglist && b.Id != booking.Id)
                .OrderBy(b => b.WaitingNumber ?? int.MaxValue)
                .ThenBy(b => b.Id)
                .ToList();

            var bookingsToUpdate = new List<Booking>();

            booking.BookingStatus = BookingStatus.Cancelled;
            booking.WaitingNumber = null;
            bookingsToUpdate.Add(booking);

            if (previousStatus == BookingStatus.Confirmed && waitingListBookings.Count > 0)
            {
                var FirstWaitingListBooking = waitingListBookings[0];
                FirstWaitingListBooking.BookingStatus = BookingStatus.Confirmed;
                FirstWaitingListBooking.WaitingNumber = null;
                bookingsToUpdate.Add(FirstWaitingListBooking);
                waitingListBookings.RemoveAt(0);
            }

            for (var i = 0; i < waitingListBookings.Count; i++)
            {
                waitingListBookings[i].WaitingNumber = i + 1;
                bookingsToUpdate.Add(waitingListBookings[i]);
            }

            _bookingRepository.UpdateRange(bookingsToUpdate);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }
    }
}