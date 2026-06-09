using System;
using Volo.Abp.Domain.Repositories;

namespace Abp.Demo.Resources;

public interface IResourceRepository : IRepository<Resource, Guid>
{
}
