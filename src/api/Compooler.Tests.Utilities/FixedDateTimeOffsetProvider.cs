using Compooler.Domain;

namespace Compooler.Tests.Utilities;

public class FixedDateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public required DateTimeOffset Now { get; init; }
    public DateTimeOffset Future => Now.AddDays(1);
    public DateTimeOffset Past => Now.AddDays(-1);
}
