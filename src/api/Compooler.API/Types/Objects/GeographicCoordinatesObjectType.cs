using Compooler.Domain.Entities.CommuteGroupEntity;
using JetBrains.Annotations;

namespace Compooler.API.Types.Objects;

[PublicAPI]
public class GeographicCoordinatesObjectType : ObjectType<GeographicCoordinates>
{
    protected override void Configure(IObjectTypeDescriptor<GeographicCoordinates> descriptor)
    {
        descriptor.Field(x => x.Latitude);
        descriptor.Field(x => x.Longitude);
    }
}
