using DAL.Models;
using Services.Dto.Request;
using Services.Services;

namespace Test.Unit;

public class BookingServiceTests
{
    [Fact]
    public async Task GetAllBookingsAsync_ReturnsMappedBookings()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        var booking = ServiceTestData.Booking(id: 7, userId: "user-7", eventId: 9, status: BookingStatus.Confirmed);
        bookingRepository.Bookings.Add(booking);

        var service = new BookingService(bookingRepository, eventRepository);

        var result = await service.GetAllBookingsAsync();

        Assert.Single(result);
        var dto = result[0];
        Assert.Equal(7, dto.Id);
        Assert.Equal("user-7", dto.UserId);
        Assert.Equal("First Last", dto.UserName);
        Assert.Equal(9, dto.EventId);
        Assert.Equal(BookingStatus.Confirmed, dto.BookingStatus);
        Assert.Equal("Concert", dto.EventTitle);
    }

    [Fact]
    public async Task CreateBookingAsync_ConfirmsBooking_WhenCapacityIsAvailable()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        eventRepository.Events.Add(ServiceTestData.Event(id: 3, capacity: 2, bookedCount: 1));

        var service = new BookingService(bookingRepository, eventRepository);

        var result = await service.CreateBookingAsync(new CreateBookingRequestDto
        {
            UserId = "user-1",
            EventId = 3
        });

        Assert.Equal(BookingStatus.Confirmed, result.BookingStatus);
        Assert.Null(result.WaitingNumber);
        Assert.Single(bookingRepository.Bookings);
        Assert.Equal(3, bookingRepository.LastCreated?.EventId);
        Assert.Equal("user-1", bookingRepository.LastCreated?.UserId);
    }

    [Fact]
    public async Task CreateBookingAsync_PlacesBookingOnWaitingList_WhenEventIsFull()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        eventRepository.Events.Add(ServiceTestData.Event(id: 4, capacity: 1, bookedCount: 1));
        bookingRepository.Bookings.Add(ServiceTestData.Booking(id: 11, userId: "other-user", eventId: 4, status: BookingStatus.Confirmed));

        var service = new BookingService(bookingRepository, eventRepository);

        var result = await service.CreateBookingAsync(new CreateBookingRequestDto
        {
            UserId = "user-2",
            EventId = 4
        });

        Assert.Equal(BookingStatus.Waitinglist, result.BookingStatus);
        Assert.Equal(1, result.WaitingNumber);
        Assert.Equal(BookingStatus.Waitinglist, bookingRepository.LastCreated?.BookingStatus);
        Assert.Equal(1, bookingRepository.LastCreated?.WaitingNumber);
    }

    [Fact]
    public async Task CreateBookingAsync_Throws_WhenUserAlreadyHasActiveBookingForEvent()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        eventRepository.Events.Add(ServiceTestData.Event(id: 5, capacity: 3));
        bookingRepository.Bookings.Add(ServiceTestData.Booking(id: 20, userId: "user-3", eventId: 5, status: BookingStatus.Confirmed));

        var service = new BookingService(bookingRepository, eventRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateBookingAsync(new CreateBookingRequestDto
        {
            UserId = "user-3",
            EventId = 5
        }));

        Assert.Equal("You already have an active booking for this event.", exception.Message);
        Assert.DoesNotContain(bookingRepository.Bookings, b => b.Id != 20);
    }

    [Fact]
    public async Task CancelBookingAsync_PromotesFirstWaitingListBooking_AndRenumbersRemainingWaitingList()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        bookingRepository.Bookings.AddRange(
            new[]
            {
                ServiceTestData.Booking(id: 1, userId: "user-1", eventId: 10, status: BookingStatus.Confirmed),
                ServiceTestData.Booking(id: 2, userId: "user-2", eventId: 10, status: BookingStatus.Waitinglist, waitingNumber: 1),
                ServiceTestData.Booking(id: 3, userId: "user-3", eventId: 10, status: BookingStatus.Waitinglist, waitingNumber: 2),
                ServiceTestData.Booking(id: 4, userId: "user-4", eventId: 10, status: BookingStatus.Cancelled, waitingNumber: null)
            });

        var service = new BookingService(bookingRepository, eventRepository);

        var result = await service.CancelBookingAsync(1);

        Assert.True(result);
        Assert.True(bookingRepository.SaveChangesCalled);
        Assert.NotNull(bookingRepository.UpdatedRange);
        Assert.Equal(3, bookingRepository.UpdatedRange!.Count);

        var cancelled = bookingRepository.Bookings.Single(b => b.Id == 1);
        var promoted = bookingRepository.Bookings.Single(b => b.Id == 2);
        var remaining = bookingRepository.Bookings.Single(b => b.Id == 3);

        Assert.Equal(BookingStatus.Cancelled, cancelled.BookingStatus);
        Assert.Null(cancelled.WaitingNumber);
        Assert.Equal(BookingStatus.Confirmed, promoted.BookingStatus);
        Assert.Null(promoted.WaitingNumber);
        Assert.Equal(BookingStatus.Waitinglist, remaining.BookingStatus);
        Assert.Equal(1, remaining.WaitingNumber);
    }

    [Fact]
    public async Task CancelBookingAsync_ReturnsFalse_WhenBookingDoesNotExist()
    {
        var bookingRepository = new BookingRepositoryFake();
        var eventRepository = new EventRepositoryFake();
        var service = new BookingService(bookingRepository, eventRepository);

        var result = await service.CancelBookingAsync(99);

        Assert.False(result);
        Assert.False(bookingRepository.SaveChangesCalled);
    }
}
