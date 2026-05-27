using Services.Services;

namespace Test.Unit;

public class MemberServiceTests
{
    [Fact]
    public async Task GetAllMembersAsync_ProjectsMembersAndBookingCounts()
    {
        var repository = new MemberRepositoryFake();
        repository.Members.Add((ServiceTestData.User(id: "user-1", firstName: "firstname-1", lastName: "lastname-1", email: null, bookingCount: 3), true));
        repository.Members.Add((ServiceTestData.User(id: "user-2", firstName: "firstname-2", lastName: "lastname-2", email: "email@example.com", bookingCount: 1), false));
        var service = new MemberService(repository);

        var result = await service.GetAllMembersAsync();

        Assert.Equal(2, result.Count);
        Assert.Collection(result,
            first =>
            {
                Assert.Equal("user-1", first.UserId);
                Assert.Equal("firstname-1", first.FirstName);
                Assert.Equal("lastname-1", first.LastName);
                Assert.Equal(string.Empty, first.Email);
                Assert.True(first.IsAdmin);
                Assert.Equal(3, first.BookingCount);
            },
            second =>
            {
                Assert.Equal("user-2", second.UserId);
                Assert.Equal("firstname-2", second.FirstName);
                Assert.Equal("lastname-2", second.LastName);
                Assert.Equal("email@example.com", second.Email);
                Assert.False(second.IsAdmin);
                Assert.Equal(1, second.BookingCount);
            });
    }

    [Fact]
    public async Task ToggleAdminAsync_ReturnsRepositoryResult_AndPassesArguments()
    {
        var repository = new MemberRepositoryFake { ToggleResult = false };
        var service = new MemberService(repository);

        var result = await service.ToggleAdminAsync("user-9", true);

        Assert.False(result);
        Assert.Equal("user-9", repository.LastToggleUserId);
        Assert.True(repository.LastToggleMakeAdmin);
    }
}
