namespace Services.Dto.Request
{
    public class CreateBookingRequestDto
    {
        public required string UserId { get; set; }
        public required int EventId { get; set; }
    }
}