using System;
using Abp.Demo.Booking;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Abp.Demo.Resources;

public class Resource : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public int Capacity { get; private set; }
    public ResourceType Type { get; private set; }
    public bool IsActive { get; private set; }

    protected Resource() { }

    public Resource(
        Guid id,
        string name,
        string description,
        string location,
        int capacity,
        ResourceType type,
        bool isActive = true)
        : base(id)
    {
        SetName(name);
        SetDescription(description);
        SetLocation(location);
        SetCapacity(capacity);
        Type = type;
        IsActive = isActive;
    }

    public Resource SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), BookingConsts.MaxNameLength);
        return this;
    }

    public Resource SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(description), BookingConsts.MaxDescriptionLength);
        return this;
    }

    public Resource SetLocation(string location)
    {
        Location = Check.NotNullOrWhiteSpace(location, nameof(location), BookingConsts.MaxLocationLength);
        return this;
    }

    public Resource SetCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

        Capacity = capacity;
        return this;
    }

    public Resource Activate()
    {
        IsActive = true;
        return this;
    }

    public Resource Deactivate()
    {
        IsActive = false;
        return this;
    }
}
