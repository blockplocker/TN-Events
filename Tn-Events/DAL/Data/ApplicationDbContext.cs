using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<DAL.Models.Category> Categories { get; set; }
        public DbSet<DAL.Models.Event> Events { get; set; }
        public DbSet<DAL.Models.Booking> Bookings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
