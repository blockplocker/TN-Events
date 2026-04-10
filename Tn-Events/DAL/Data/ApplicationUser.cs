using Microsoft.AspNetCore.Identity;
using DAL.Models;

namespace DAL.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}
