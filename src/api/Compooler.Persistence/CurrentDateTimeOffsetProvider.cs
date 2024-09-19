using Compooler.Domain;

namespace Compooler.Persistence;

public class CurrentDateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now.ToUniversalTime();
}
