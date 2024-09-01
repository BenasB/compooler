namespace Compooler.Domain;

public interface IDateTimeOffsetProvider
{
    public DateTimeOffset Now { get; }
}
