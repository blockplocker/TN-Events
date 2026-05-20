using DAL.Models;

namespace Services.Dto.Response
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public BookingStatus BookingStatus { get; set; }
        public int? WaitingNumber { get; set; }
    }
}