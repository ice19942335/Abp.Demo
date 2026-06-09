using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Abp.Demo.Resources;

public interface IResourceAppService
    : ICrudAppService<ResourceDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateResourceDto>
{
}
