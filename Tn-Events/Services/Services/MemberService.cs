using DAL.Repositories.Interfaces;
using Services.Dto.Response;
using Services.Services.Interfaces;

namespace Services.Services
{
    public class MemberService(IMemberRepository memberRepository) : IMemberService
    {
        private readonly IMemberRepository _memberRepository = memberRepository;

        public async Task<List<MemberDto>> GetAllMembersAsync()
        {
            var members = await _memberRepository.GetAllWithRolesAsync();

            return members.Select(m => new MemberDto
            {
                UserId = m.User.Id,
                FirstName = m.User.FirstName,
                LastName = m.User.LastName,
                Email = m.User.Email ?? string.Empty,
                IsAdmin = m.IsAdmin,
                BookingCount = m.User.Bookings.Count
            }).ToList();
        }

        public async Task<bool> ToggleAdminAsync(string userId, bool makeAdmin)
        {
            return await _memberRepository.ToggleAdminAsync(userId, makeAdmin);
        }
    }
}
