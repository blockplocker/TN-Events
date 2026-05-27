using DAL.Data;
using DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class MemberRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IMemberRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<List<(ApplicationUser User, bool IsAdmin)>> GetAllWithRolesAsync()
        {
            var query = from user in _context.Users.Include(u => u.Bookings)
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId into userRoles
                        from userRole in userRoles.DefaultIfEmpty()
                        join role in _context.Roles on userRole!.RoleId equals role.Id into roles
                        from role in roles.DefaultIfEmpty()
                        group role by user into grouped
                        select new
                        {
                            User = grouped.Key,
                            IsAdmin = grouped.Any(r => r != null && r.Name == "Admin")
                        };

            var usersWithRoles = await query.ToListAsync();

            return usersWithRoles
                .Select(x => (x.User, x.IsAdmin))
                .ToList();
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
