namespace Services.Dto.Request
{
    public class CreateEventRequestDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Address { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int Capacity { get; set; }
        public required int CategoryId { get; set; }
    }
}
