using DAL.Models;

namespace Services.Dto.Response
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }   
        public int EventId { get; set; }
        public required string EventTitle { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public int? WaitingNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public required string Description { get; set; }
        public required string CategoryName { get; set; }
    }
}