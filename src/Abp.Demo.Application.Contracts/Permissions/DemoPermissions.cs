namespace Abp.Demo.Permissions;

public static class DemoPermissions
{
    public const string GroupName = DemoPermissionNames.GroupName;

    public static class Resources
    {
        public const string Default = DemoPermissionNames.Resources.Default;
        public const string Create = DemoPermissionNames.Resources.Create;
        public const string Edit = DemoPermissionNames.Resources.Edit;
        public const string Delete = DemoPermissionNames.Resources.Delete;
    }

    public static class Bookings
    {
        public const string Default = DemoPermissionNames.Bookings.Default;
        public const string Create = DemoPermissionNames.Bookings.Create;
        public const string Cancel = DemoPermissionNames.Bookings.Cancel;
    }
}
