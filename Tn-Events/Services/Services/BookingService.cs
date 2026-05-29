using DAL.Models;
using DAL.Repositories.Interfaces;
using Services.Dto.Request;
using Services.Dto.Response;
using Services.Mappers;
using Services.Services.Interfaces;
using DalBookingStatus = DAL.Models.BookingStatus;
using DtoBookingStatus = Services.Dto.BookingStatus;

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
                    b.BookingStatus != DalBookingStatus.Cancelled);

                if (existingActiveBooking != null)
                {
                    throw new InvalidOperationException("You already have an active booking for this event.");
                }

                var existingBookings = await _bookingRepository.GetByEventIdAsync(dto.EventId);
                var confirmedCount = existingBookings.Count(b => b.BookingStatus == DalBookingStatus.Confirmed);
                var status = confirmedCount < ev.Capacity ? DalBookingStatus.Confirmed : DalBookingStatus.Waitinglist;

                int? waitingNumber = null;
                if (status == DalBookingStatus.Waitinglist)
                {
                    waitingNumber = existingBookings.Count(b => b.BookingStatus == DalBookingStatus.Waitinglist) + 1;
                }

                var booking = await _bookingRepository.CreateAsync(BookingMapper.Map(dto, status, waitingNumber));
                return BookingMapper.ToDto(booking);
            });
        }

        public async Task<Dictionary<int, BookingResponseDto>> GetUserActiveBookingsByEventAsync(string userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return BookingMapper.ToDtoList(bookings)
                .Where(b => b.BookingStatus != DtoBookingStatus.Cancelled)
                .GroupBy(b => b.EventId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(b => b.Id).First());
        }

        public async Task<List<BookingResponseDto>> GetUserConfirmedBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetConfirmedBookingsFromUserIdAsync(userId);
            return BookingMapper.ToDtoList(bookings).ToList();
        }

        public async Task<List<BookingResponseDto>> GetUserWaitingListBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetWaitingListBookingsFromUserIdAsync(userId);
            return BookingMapper.ToDtoList(bookings).ToList();
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            return await _bookingRepository.ExecuteInTransactionAsync(async () =>
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                if (booking == null) return false;

                var previousStatus = booking.BookingStatus;
                if (previousStatus != DalBookingStatus.Confirmed && previousStatus != DalBookingStatus.Waitinglist)
                {
                    return false;
                }

                var eventBookings = await _bookingRepository.GetByEventIdAsync(booking.EventId);
                var waitingListBookings = eventBookings
                    .Where(b => b.BookingStatus == DalBookingStatus.Waitinglist && b.Id != booking.Id)
                    .OrderBy(b => b.WaitingNumber ?? int.MaxValue)
                    .ThenBy(b => b.Id)
                    .ToList();

                var bookingsToUpdate = new List<Booking>();

                booking.BookingStatus = DalBookingStatus.Cancelled;
                booking.WaitingNumber = null;
                bookingsToUpdate.Add(booking);

                if (previousStatus == DalBookingStatus.Confirmed && waitingListBookings.Count > 0)
                {
                    var FirstWaitingListBooking = waitingListBookings[0];
                    FirstWaitingListBooking.BookingStatus = DalBookingStatus.Confirmed;
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
            });
        }
    }
}