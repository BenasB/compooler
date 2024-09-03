using JetBrains.Annotations;

namespace Compooler.API.Types.Queries.Inputs;

[PublicAPI]
public record RideRankingInput(
    double StartLatitude,
    double StartLongitude,
    double FinishLatitude,
    double FinishLongitude
);
