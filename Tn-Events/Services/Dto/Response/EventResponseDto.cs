namespace Services.Dto.Response
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public bool IsCancelled { get; set; }
        public required string CategoryName { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSpots => Capacity - BookedCount;
        public bool IsFull => AvailableSpots <= 0;
    }
}
