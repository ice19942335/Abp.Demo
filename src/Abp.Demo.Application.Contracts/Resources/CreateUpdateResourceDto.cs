using System.ComponentModel.DataAnnotations;
using Abp.Demo.Booking;

namespace Abp.Demo.Resources;

public class CreateUpdateResourceDto
{
    [Required]
    [StringLength(BookingConsts.MaxNameLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(BookingConsts.MaxDescriptionLength)]
    public string Description { get; set; } = null!;

    [Required]
    [StringLength(BookingConsts.MaxLocationLength)]
    public string Location { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    [Required]
    public ResourceType Type { get; set; }

    public bool IsActive { get; set; } = true;
}
