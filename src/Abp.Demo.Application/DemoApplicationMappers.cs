using Abp.Demo.Resources;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Abp.Demo;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ResourceMapper : MapperBase<Resource, ResourceDto>
{
    public override partial ResourceDto Map(Resource source);

    public override partial void Map(Resource source, ResourceDto destination);
}
