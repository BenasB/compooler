using Compooler.Domain.Entities.RideEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class GeographicCoordinatesObjectType : ObjectType<GeographicCoordinates>
{
    protected override void Configure(IObjectTypeDescriptor<GeographicCoordinates> descriptor)
    {
        descriptor.BindFieldsExplicitly();

        descriptor.Field(x => x.Latitude);
        descriptor.Field(x => x.Longitude);
    }
}
