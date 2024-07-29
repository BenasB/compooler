using System.Diagnostics.CodeAnalysis;

namespace Compooler.Domain;

public sealed class Result
{
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailed { get; }

    public Error? Error { get; }

    private Result(bool isFailed, Error? error)
    {
        IsFailed = isFailed;
        Error = error;
    }

    public static Result Success() => new(isFailed: false, error: null);

    public static Result Failure(Error error) => new(isFailed: true, error: error);
}

public sealed class Result<T>
{
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailed { get; }

    public Error? Error { get; }
    public T? Value { get; }

    private Result(bool isFailed, Error? error, T? value)
    {
        IsFailed = isFailed;
        Error = error;
        Value = value;
    }

    public static Result<T> Success(T value) => new(isFailed: false, error: null, value: value);

    public static Result<T> Failure(Error error) =>
        new(isFailed: true, error: error, value: default);
}
