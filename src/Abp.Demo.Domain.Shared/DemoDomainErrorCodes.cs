namespace Abp.Demo;

public static class DemoDomainErrorCodes
{
    public const string BookingConflict = "Booking:00001";
    public const string ResourceNotActive = "Booking:00002";
    public const string InvalidBookingStatusTransition = "Booking:00003";
    public const string BookingInvalidTimeRange = "Booking:00004";
}
