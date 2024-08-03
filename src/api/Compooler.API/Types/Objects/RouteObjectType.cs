using JetBrains.Annotations;
using Route = Compooler.Domain.Entities.CommuteGroupEntity.Route;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class RouteObjectType : ObjectType<Route>
{
    protected override void Configure(IObjectTypeDescriptor<Route> descriptor)
    {
        descriptor.Field(x => x.Start);
        descriptor.Field(x => x.Finish);
    }
}
