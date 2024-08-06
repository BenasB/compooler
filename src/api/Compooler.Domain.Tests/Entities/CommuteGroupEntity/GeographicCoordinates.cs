using Compooler.Domain.Entities.CommuteGroupEntity;

namespace Compooler.Domain.Tests.Entities.CommuteGroupEntity;

public class GeographicCoordinatesTests
{
    private const double ValidLatitude = 0;
    private const double ValidLongitude = 0;

    [Fact]
    public void Create_LatitudeBelowMinus90_Failed()
    {
        var coords = GeographicCoordinates.Create(-91, ValidLongitude);

        Assert.True(coords.IsFailed);
        Assert.Equal(new GeographicCoordinatesErrors.InvalidLatitudeError(), coords.Error);
    }

    [Fact]
    public void Create_LatitudeAbove90_Failed()
    {
        var coords = GeographicCoordinates.Create(91, ValidLongitude);

        Assert.True(coords.IsFailed);
        Assert.Equal(new GeographicCoordinatesErrors.InvalidLatitudeError(), coords.Error);
    }

    [Fact]
    public void Create_LongitudeBelowMinus180_Failed()
    {
        var coords = GeographicCoordinates.Create(ValidLatitude, -181);

        Assert.True(coords.IsFailed);
        Assert.Equal(new GeographicCoordinatesErrors.InvalidLongitudeError(), coords.Error);
    }

    [Fact]
    public void Create_LongitudeAbove180_Failed()
    {
        var coords = GeographicCoordinates.Create(ValidLatitude, 181);

        Assert.True(coords.IsFailed);
        Assert.Equal(new GeographicCoordinatesErrors.InvalidLongitudeError(), coords.Error);
    }

    [Fact]
    public void Create_ValidCoordinates_Success()
    {
        var coords = GeographicCoordinates.Create(ValidLatitude, ValidLongitude);

        Assert.False(coords.IsFailed);
        Assert.Equal(
            [ValidLatitude, ValidLongitude],
            [coords.Value.Latitude, coords.Value.Longitude]
        );
    }
}
