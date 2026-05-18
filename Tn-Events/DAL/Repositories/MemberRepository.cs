using DAL.Data;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<(ApplicationUser User, bool IsAdmin)>> GetAllWithRolesAsync()
        {
            var users = await _context.Users
                .Include(u => u.Bookings)
                .ToListAsync();

            var result = new List<(ApplicationUser, bool)>();

            foreach (var user in users)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                result.Add((user, isAdmin));
            }

            return result;
        }

        public async Task<bool> ToggleAdminAsync(string userId, bool makeAdmin)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = makeAdmin
                ? await _userManager.AddToRoleAsync(user, "Admin")
                : await _userManager.RemoveFromRoleAsync(user, "Admin");

            return result.Succeeded;
        }
    }
}
