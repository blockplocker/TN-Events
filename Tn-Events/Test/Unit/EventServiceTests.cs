using DAL.Models;
using Services.Dto.Request;
using Services.Services;

namespace Test.Unit;

public class EventServiceTests
{
    [Fact]
    public async Task GetAllEventsAsync_ReturnsMappedEvents()
    {
        var eventRepository = new EventRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        eventRepository.Events.Add(ServiceTestData.Event(id: 1, bookedCount: 2));

        var service = new EventService(eventRepository, categoryRepository, new BookingRepositoryFake());

        var result = await service.GetAllEventsAsync();

        Assert.Single(result);
        var dto = result[0];
        Assert.Equal(1, dto.Id);
        Assert.Equal("Concert", dto.Title);
        Assert.Equal(2, dto.BookedCount);
        Assert.Equal(8, dto.AvailableSpots);
        Assert.False(dto.IsFull);
        Assert.Equal("Music", dto.CategoryName);
    }

    [Fact]
    public async Task GetUpcomingEventsAsync_UsesRequestedCount()
    {
        var eventRepository = new EventRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        eventRepository.Events.AddRange(new[]
        {
            ServiceTestData.Event(id: 1, title: "One"),
            ServiceTestData.Event(id: 2, title: "Two"),
            ServiceTestData.Event(id: 3, title: "Three")
        });

        var service = new EventService(eventRepository, categoryRepository, new BookingRepositoryFake());

        var result = await service.GetUpcomingEventsAsync(2);

        Assert.Equal(2, result.Count);
        Assert.Equal(2, eventRepository.LastUpcomingCount);
        Assert.Collection(result,
            first => Assert.Equal("One", first.Title),
            second => Assert.Equal("Two", second.Title));
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsNull_WhenEventDoesNotExist()
    {
        var service = new EventService(new EventRepositoryFake(), new CategoryRepositoryFake(), new BookingRepositoryFake());

        var result = await service.GetEventByIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEventAsync_CreatesNotCancelledEvent()
    {
        var eventRepository = new EventRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var service = new EventService(eventRepository, categoryRepository, new BookingRepositoryFake());
        var dto = new CreateEventRequestDto
        {
            Title = "New Event",
            Description = "Description",
            Address = "Address",
            StartDate = new DateTime(2026, 1, 1, 1, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 1, 1, 2, 0, 0, DateTimeKind.Utc),
            Capacity = 50,
            CategoryId = 7
        };

        await service.CreateEventAsync(dto);

        Assert.Single(eventRepository.Events);
        Assert.NotNull(eventRepository.LastCreated);
        Assert.Equal("New Event", eventRepository.LastCreated!.Title);
        Assert.False(eventRepository.LastCreated.IsCancelled);
        Assert.Equal(7, eventRepository.LastCreated.CategoryId);
    }

    [Fact]
    public async Task UpdateEventAsync_UpdatesExistingEvent()
    {
        var eventRepository = new EventRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var existing = ServiceTestData.Event(id: 8, title: "Old Title", categoryId: 5);
        eventRepository.Events.Add(existing);
        var service = new EventService(eventRepository, categoryRepository, new BookingRepositoryFake());
        var dto = new UpdateEventRequestDto
        {
            Id = 8,
            Title = "Updated Title",
            Description = "New Description",
            Address = "New Address",
            StartDate = new DateTime(2026, 1, 1, 1, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 1, 1, 2, 0, 0, DateTimeKind.Utc),
            Capacity = 75,
            CategoryId = 9
        };

        var result = await service.UpdateEventAsync(dto);

        Assert.True(result);
        Assert.Equal("Updated Title", existing.Title);
        Assert.Equal("New Description", existing.Description);
        Assert.Equal(9, existing.CategoryId);
        Assert.Same(existing, eventRepository.LastUpdated);
    }

    [Fact]
    public async Task ToggleCancelEventAsync_DelegatesToRepository()
    {
        var eventRepository = new EventRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var service = new EventService(eventRepository, categoryRepository, new BookingRepositoryFake());

        await service.ToggleCancelEventAsync(11);

        Assert.Equal(11, eventRepository.LastToggledId);
    }
  }
