namespace Abp.Demo.Permissions;

public static class DemoPermissions
{
    public const string GroupName = "BookingSystem";

    public static class Resources
    {
        public const string Default = GroupName + ".Resources";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Bookings
    {
        public const string Default = GroupName + ".Bookings";
        public const string Create = Default + ".Create";
        public const string Cancel = Default + ".Cancel";
    }
}
