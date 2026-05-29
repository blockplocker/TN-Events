using DAL.Models;
using Services.Dto.Request;
using Services.Dto.Response;
using DalBookingStatus = DAL.Models.BookingStatus;
using DtoBookingStatus = Services.Dto.BookingStatus;

namespace Services.Mappers
{
    public static class BookingMapper
    {
        public static Booking Map(CreateBookingRequestDto dto, DalBookingStatus status, int? waitingNumber = null)
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
                BookingStatus = (DtoBookingStatus)booking.BookingStatus,
                WaitingNumber = booking.WaitingNumber,
                StartDate = booking.Event?.StartDate ?? default,
                EndDate = booking.Event?.EndDate ?? default,
                Description = booking.Event?.Description ?? string.Empty,
                CategoryName = booking.Event?.Category?.Name ?? string.Empty
            };
        }

        public static List<BookingResponseDto> ToDtoList(List<Booking> bookings)
        {
            return bookings.Select(ToDto).ToList();
        }
    }
}