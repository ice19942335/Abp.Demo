using System;
using Abp.Demo.Booking;
using Volo.Abp.Application.Dtos;

namespace Abp.Demo.Resources;

public class ResourceDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public int Capacity { get; set; }
    public ResourceType Type { get; set; }
    public bool IsActive { get; set; }
}
