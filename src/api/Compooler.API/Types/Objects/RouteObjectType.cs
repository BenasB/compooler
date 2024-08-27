using JetBrains.Annotations;
using Route = Compooler.Domain.Entities.RideEntity.Route;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class RouteObjectType : ObjectType<Route>
{
    protected override void Configure(IObjectTypeDescriptor<Route> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Start);
        descriptor.Field(x => x.Finish);
    }
}
