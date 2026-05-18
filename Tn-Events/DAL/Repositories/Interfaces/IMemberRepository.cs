using DAL.Data;

namespace DAL.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        Task<List<(ApplicationUser User, bool IsAdmin)>> GetAllWithRolesAsync();
        Task<bool> ToggleAdminAsync(string userId, bool makeAdmin);
    }
}
