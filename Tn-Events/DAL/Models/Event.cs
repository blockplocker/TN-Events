using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class Event
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Address { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int Capacity { get; set; }
        public required bool IsCancelled { get; set; }
        public required int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
