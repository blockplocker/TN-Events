using System;
using System.Collections.Generic;
using System.Text;
using DAL.Data;

namespace DAL.Models
{
    public class Booking
    {
        public required int Id { get; set; }
        public required string UserId { get; set; }
        public required int EventId { get; set; }
        public int? WaitingNumber { get; set; }
        public required BookingStatus BookingStatus { get; set; }
        public  ApplicationUser? User { get; set; }
        public  Event? Event { get; set; }
    }
}