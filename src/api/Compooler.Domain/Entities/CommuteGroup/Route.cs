namespace Compooler.Domain.Entities.CommuteGroup;

public sealed class Route
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }

    private Route(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Result<Route> New(double latitude, double longitude)
    {
        var res = new Result<Route>();

        if (res.IsFailed)
        {
            return res;
        }

        var a = res.Value.Latitude;

        return res;
    }
}
