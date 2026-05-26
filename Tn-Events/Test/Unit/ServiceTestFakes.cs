using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;

namespace Test.Unit;

internal static class ServiceTestData
{
    public static Category Category(int id = 1, string name = "Music") => new()
    {
        Id = id,
        Name = name
    };

    public static Event Event(int id = 1, int categoryId = 1, string title = "Concert", bool isCancelled = false, int capacity = 10, int bookedCount = 0)
    {
        var category = Category(categoryId);
        var ev = new Event
        {
            Id = id,
            Title = title,
            Description = "Description",
            Address = "Address",
            StartDate = new DateTime(2026, 1, 1, 1, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 1, 1, 2, 0, 0, DateTimeKind.Utc),
            Capacity = capacity,
            IsCancelled = isCancelled,
            CategoryId = categoryId,
            Category = category
        };

        ev.Bookings = Enumerable.Range(1, bookedCount)
            .Select(i => new Booking
            {
                Id = i,
                UserId = $"user-{i}",
                EventId = id,
                BookingStatus = BookingStatus.Confirmed,
                Event = ev
            })
            .ToList();

        return ev;
    }

    public static Booking Booking(int id = 1, string userId = "user-1", int eventId = 1, BookingStatus status = BookingStatus.Confirmed, int? waitingNumber = null)
    {
        var ev = Event(eventId);
        return new Booking
        {
            Id = id,
            UserId = userId,
            EventId = eventId,
            BookingStatus = status,
            WaitingNumber = waitingNumber,
            User = new ApplicationUser
            {
                Id = userId,
                FirstName = "First",
                LastName = "Last",
                UserName = $"{userId}@example.com"
            },
            Event = ev
        };
    }

    public static ApplicationUser User(string id = "user-1", string firstName = "First", string lastName = "Last", string? email = "user@example.com", int bookingCount = 0)
    {
        return new ApplicationUser
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Bookings = Enumerable.Range(1, bookingCount)
                .Select(i => new Booking
                {
                    Id = i,
                    UserId = id,
                    EventId = i,
                    BookingStatus = BookingStatus.Confirmed
                })
                .ToList()
        };
    }
}

internal sealed class BookingRepositoryFake : IBookingRepository
{
    public List<Booking> Bookings { get; } = [];
    public List<Booking>? UpdatedRange { get; private set; }
    public Booking? LastCreated { get; private set; }
    public int? LastGetByEventId { get; private set; }
    public string? LastGetByUserId { get; private set; }
    public int? LastGetById { get; private set; }
    public bool SaveChangesCalled { get; private set; }

    public Task<List<Booking>> GetAllAsync() => Task.FromResult(Bookings.ToList());

    public Task<List<Booking>> GetAllWaitingListAsync() => Task.FromResult(Bookings.Where(b => b.BookingStatus == BookingStatus.Waitinglist).ToList());

    public Task<List<Booking>> GetByUserIdAsync(string userId)
    {
        LastGetByUserId = userId;
        return Task.FromResult(Bookings.Where(b => b.UserId == userId).ToList());
    }
    public Task<List<Booking>> GetConfirmedBookingsFromUserIdAsync(string userId)
    {
        LastGetByUserId = userId;
        return Task.FromResult(Bookings.Where(b => b.UserId == userId && b.BookingStatus == BookingStatus.Confirmed).ToList());
    }
    public Task<List<Booking>> GetWaitingListBookingsFromUserIdAsync(string userId)
    {
        LastGetByUserId = userId;
        return Task.FromResult(Bookings.Where(b => b.UserId == userId && b.BookingStatus == BookingStatus.Waitinglist).ToList());
    }

    public Task<List<Booking>> GetByEventIdAsync(int eventId)
    {
        LastGetByEventId = eventId;
        return Task.FromResult(Bookings.Where(b => b.EventId == eventId).ToList());
    }

    public Task<Booking?> GetByIdAsync(int id)
    {
        LastGetById = id;
        return Task.FromResult(Bookings.FirstOrDefault(b => b.Id == id));
    }

    public Task<Booking> CreateAsync(Booking booking)
    {
        LastCreated = booking;
        booking.Id = booking.Id == 0 ? Bookings.Count == 0 ? 1 : Bookings.Max(b => b.Id) + 1 : booking.Id;
        Bookings.Add(booking);
        return Task.FromResult(booking);
    }

    public void UpdateRange(IEnumerable<Booking> bookings)
    {
        UpdatedRange = bookings.ToList();
    }

    public Task SaveChangesAsync()
    {
        SaveChangesCalled = true;
        return Task.CompletedTask;
    }

    public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action) => action();
}

internal sealed class EventRepositoryFake : IEventRepository
{
    public List<Event> Events { get; } = [];
    public Event? LastCreated { get; private set; }
    public Event? LastUpdated { get; private set; }
    public int? LastToggledId { get; private set; }
    public int? LastUpcomingCount { get; private set; }
    public int? LastGetById { get; private set; }

    public Task<List<Event>> GetAllAsync() => Task.FromResult(Events.ToList());

    public Task<List<Event>> GetAllAdminAsync() => Task.FromResult(Events.ToList());

    public Task<List<Event>> GetUpcomingAsync(int count)
    {
        LastUpcomingCount = count;
        return Task.FromResult(Events.Take(count).ToList());
    }

    public Task<Event?> GetByIdAsync(int id)
    {
        LastGetById = id;
        return Task.FromResult(Events.FirstOrDefault(e => e.Id == id));
    }

    public Task<Event> CreateAsync(Event ev)
    {
        LastCreated = ev;
        ev.Id = ev.Id == 0 ? Events.Count == 0 ? 1 : Events.Max(e => e.Id) + 1 : ev.Id;
        Events.Add(ev);
        return Task.FromResult(ev);
    }

    public Task UpdateAsync(Event ev)
    {
        LastUpdated = ev;
        var index = Events.FindIndex(e => e.Id == ev.Id);
        if (index >= 0)
        {
            Events[index] = ev;
        }

        return Task.CompletedTask;
    }

    public Task ToggleCancelAsync(int id)
    {
        LastToggledId = id;
        var ev = Events.FirstOrDefault(e => e.Id == id);
        if (ev != null)
        {
            ev.IsCancelled = !ev.IsCancelled;
        }

        return Task.CompletedTask;
    }
}

internal sealed class CategoryRepositoryFake : ICategoryRepository
{
    public List<Category> Categories { get; } = [];
    public Category? LastCreated { get; private set; }
    public int? LastDeletedId { get; private set; }

    public Task<List<Category>> GetAllAsync() => Task.FromResult(Categories.ToList());

    public Task<Category> CreateAsync(Category category)
    {
        LastCreated = category;
        category.Id = category.Id == 0 ? Categories.Count == 0 ? 1 : Categories.Max(c => c.Id) + 1 : category.Id;
        Categories.Add(category);
        return Task.FromResult(category);
    }

    public Task DeleteAsync(int id)
    {
        LastDeletedId = id;
        Categories.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}

internal sealed class MemberRepositoryFake : IMemberRepository
{
    public List<(ApplicationUser User, bool IsAdmin)> Members { get; } = [];
    public string? LastToggleUserId { get; private set; }
    public bool? LastToggleMakeAdmin { get; private set; }
    public bool ToggleResult { get; set; } = true;

    public Task<List<(ApplicationUser User, bool IsAdmin)>> GetAllWithRolesAsync() => Task.FromResult(Members.ToList());

    public Task<bool> ToggleAdminAsync(string userId, bool makeAdmin)
    {
        LastToggleUserId = userId;
        LastToggleMakeAdmin = makeAdmin;
        return Task.FromResult(ToggleResult);
    }
}
