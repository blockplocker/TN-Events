using Microsoft.AspNetCore.Identity;
using DAL.Models;

namespace DAL.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
