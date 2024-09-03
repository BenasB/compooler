using Compooler.Domain.Entities.RideEntity;
using Compooler.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Compooler.Persistence.Queries;

public static class RideRelevanceQuery
{
    public static IQueryable<Ride> FilterAndOrderByRelevance(
        this IQueryable<Ride> queryable,
        GeographicCoordinates start,
        GeographicCoordinates finish
    )
    {
        var startPoint = new Point(start.Longitude, start.Latitude);
        var finishPoint = new Point(finish.Longitude, finish.Latitude);

        // Filter out irrelevant rides
        const int maxProximityMeters = 15000;
        var filteredQuery = queryable
            .Where(ride =>
                EF.Property<Point>(ride.Route.Start, RideConfiguration.PointPropertyName)
                    .IsWithinDistance(startPoint, maxProximityMeters)
            )
            .Where(ride =>
                EF.Property<Point>(ride.Route.Finish, RideConfiguration.PointPropertyName)
                    .IsWithinDistance(finishPoint, maxProximityMeters)
            );

        // Gather and normalize parameters that will be used to calculate the score
        var normalizedQuery = filteredQuery
            .Select(ride => new
            {
                Ride = ride,
                StartDistance = EF.Property<Point>(
                        ride.Route.Start,
                        RideConfiguration.PointPropertyName
                    )
                    .Distance(startPoint),
                FinishDistance = EF.Property<Point>(
                        ride.Route.Finish,
                        RideConfiguration.PointPropertyName
                    )
                    .Distance(finishPoint)
            })
            .Select(x => new
            {
                x.Ride,
                NormalizedStartDistance = 1 - x.StartDistance / maxProximityMeters,
                NormalizedFinishDistance = 1 - x.FinishDistance / maxProximityMeters
            });

        // Calculate the score based on weights of each parameter
        const double startDistanceWeight = 0.5;
        const double finishDistanceWeight = 0.5;
        var result = normalizedQuery
            .Select(x => new
            {
                x.Ride,
                Score = x.NormalizedStartDistance * startDistanceWeight
                    + x.NormalizedFinishDistance * finishDistanceWeight
            })
            .OrderByDescending(x => x.Score)
            .Select(x => x.Ride);

        return result;
    }
}
