using System;

namespace Abp.Demo.Demo;

public class DemoUserDefinition
{
    public Guid Id { get; }

    public string UserName { get; }

    public string Email { get; }

    public string RoleName { get; }

    public string RoleDisplayName { get; }

    public string Password { get; }

    public DemoUserDefinition(
        Guid id,
        string userName,
        string email,
        string roleName,
        string roleDisplayName,
        string password)
    {
        Id = id;
        UserName = userName;
        Email = email;
        RoleName = roleName;
        RoleDisplayName = roleDisplayName;
        Password = password;
    }
}
