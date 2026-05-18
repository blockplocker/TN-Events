using Services.Dto.Response;

namespace Services.Services.Interfaces
{
    public interface IMemberService
    {
        Task<List<MemberDto>> GetAllMembersAsync();
        Task<bool> ToggleAdminAsync(string userId, bool makeAdmin);
    }
}
