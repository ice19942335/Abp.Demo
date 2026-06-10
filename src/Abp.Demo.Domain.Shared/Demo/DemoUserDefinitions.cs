using System;
using System.Collections.Generic;

namespace Abp.Demo.Demo;

public static class DemoUserDefinitions
{
    public const string DefaultPassword = "1q2w3E*";
    public const string AdminUserName = "admin";
    public const string AdminEmail = "admin@abp.io";

    public static readonly Guid AdminUserId = Guid.Parse("e0000000-0000-0000-0000-000000000001");
    public static readonly Guid ManagerUserId = Guid.Parse("e0000000-0000-0000-0000-000000000002");
    public static readonly Guid EmployeeUserId = Guid.Parse("e0000000-0000-0000-0000-000000000003");
    public static readonly Guid ViewerUserId = Guid.Parse("e0000000-0000-0000-0000-000000000004");

    public static readonly IReadOnlyList<DemoUserDefinition> All =
    [
        new(
            AdminUserId,
            AdminUserName,
            AdminEmail,
            "admin",
            "Administrator",
            DefaultPassword),
        new(
            ManagerUserId,
            "manager",
            "manager@demo.local",
            DemoRoles.Manager,
            "Manager",
            DefaultPassword),
        new(
            EmployeeUserId,
            "employee",
            "employee@demo.local",
            DemoRoles.Employee,
            "Employee",
            DefaultPassword),
        new(
            ViewerUserId,
            "viewer",
            "viewer@demo.local",
            DemoRoles.Viewer,
            "Viewer",
            DefaultPassword)
    ];

    public static readonly IReadOnlyList<DemoUserDefinition> AdditionalUsers =
    [
        new(
            ManagerUserId,
            "manager",
            "manager@demo.local",
            DemoRoles.Manager,
            "Manager",
            DefaultPassword),
        new(
            EmployeeUserId,
            "employee",
            "employee@demo.local",
            DemoRoles.Employee,
            "Employee",
            DefaultPassword),
        new(
            ViewerUserId,
            "viewer",
            "viewer@demo.local",
            DemoRoles.Viewer,
            "Viewer",
            DefaultPassword)
    ];
}
