using DAL.Models;
using Services.Dto.Request;
using Services.Dto.Response;

namespace Services.Mappers
{
    public static class BookingMapper
    {
        public static Booking Map(CreateBookingRequestDto dto, BookingStatus status, int? waitingNumber = null)
        {
            return new Booking
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                BookingStatus = status,
                WaitingNumber = waitingNumber
            };
        }

        public static BookingResponseDto ToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                UserName = booking.User is null
                    ? string.Empty
                    : $"{booking.User.FirstName} {booking.User.LastName}".Trim(),
                EventId = booking.EventId,
                EventTitle = booking.Event?.Title ?? string.Empty,
                BookingStatus = booking.BookingStatus,
                WaitingNumber = booking.WaitingNumber
            };
        }

        public static List<BookingResponseDto> ToDtoList(List<Booking> bookings)
        {
            return bookings.Select(ToDto).ToList();
        }
    }
}