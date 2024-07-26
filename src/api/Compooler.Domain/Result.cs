using System.Diagnostics.CodeAnalysis;

namespace Compooler.Domain;

public class Result<T>
{
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailed { get; set; }

    public T? Value { get; set; }
}
