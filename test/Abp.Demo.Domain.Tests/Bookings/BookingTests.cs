using System;
using Abp.Demo.Booking;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Abp.Demo.Bookings;

public class BookingTests
{
    private static readonly Guid ResourceId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    private Booking CreateTestBooking()
    {
        return new Booking(
            Guid.NewGuid(),
            ResourceId,
            UserId,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1),
            "Test meeting");
    }

    [Fact]
    public void Should_Create_With_Pending_Status()
    {
        var booking = CreateTestBooking();
        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void Should_Confirm_From_Pending()
    {
        var booking = CreateTestBooking();
        booking.Confirm();
        booking.Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void Should_Cancel_From_Pending()
    {
        var booking = CreateTestBooking();
        booking.Cancel("Changed plans");
        booking.Status.ShouldBe(BookingStatus.Cancelled);
        booking.CancellationReason.ShouldBe("Changed plans");
    }

    [Fact]
    public void Should_Cancel_From_Confirmed()
    {
        var booking = CreateTestBooking();
        booking.Confirm();
        booking.Cancel("Emergency");
        booking.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public void Should_Complete_From_Confirmed()
    {
        var booking = CreateTestBooking();
        booking.Confirm();
        booking.Complete();
        booking.Status.ShouldBe(BookingStatus.Completed);
    }

    [Fact]
    public void Should_Not_Complete_From_Pending()
    {
        var booking = CreateTestBooking();
        Assert.Throws<BusinessException>(() => booking.Complete())
            .Code.ShouldBe(DemoDomainErrorCodes.InvalidBookingStatusTransition);
    }

    [Fact]
    public void Should_Not_Confirm_From_Completed()
    {
        var booking = CreateTestBooking();
        booking.Confirm();
        booking.Complete();
        Assert.Throws<BusinessException>(() => booking.Confirm())
            .Code.ShouldBe(DemoDomainErrorCodes.InvalidBookingStatusTransition);
    }

    [Fact]
    public void Should_Not_Cancel_From_Completed()
    {
        var booking = CreateTestBooking();
        booking.Confirm();
        booking.Complete();
        Assert.Throws<BusinessException>(() => booking.Cancel("Reason"))
            .Code.ShouldBe(DemoDomainErrorCodes.InvalidBookingStatusTransition);
    }

    [Fact]
    public void Should_Not_Confirm_From_Cancelled()
    {
        var booking = CreateTestBooking();
        booking.Cancel("Reason");
        Assert.Throws<BusinessException>(() => booking.Confirm())
            .Code.ShouldBe(DemoDomainErrorCodes.InvalidBookingStatusTransition);
    }

    [Fact]
    public void Should_Throw_When_EndTime_Before_StartTime()
    {
        var start = DateTime.UtcNow.AddDays(1);
        Assert.Throws<BusinessException>(() => new Booking(
            Guid.NewGuid(), ResourceId, UserId,
            start, start.AddHours(-1), "Test"))
            .Code.ShouldBe(DemoDomainErrorCodes.BookingInvalidTimeRange);
    }

    [Fact]
    public void Should_Throw_When_EndTime_Equals_StartTime()
    {
        var start = DateTime.UtcNow.AddDays(1);
        Assert.Throws<BusinessException>(() => new Booking(
            Guid.NewGuid(), ResourceId, UserId,
            start, start, "Test"))
            .Code.ShouldBe(DemoDomainErrorCodes.BookingInvalidTimeRange);
    }
}
