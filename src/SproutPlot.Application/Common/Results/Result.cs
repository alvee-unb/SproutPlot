namespace SproutPlot.Application.Common.Results;

/// <summary>
/// Lightweight operation result used to model expected success/failure
/// outcomes (e.g. invalid credentials) without throwing exceptions.
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };

    public static Result Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };
}

/// <summary>
/// Operation result carrying a value on success.
/// </summary>
/// <typeparam name="T">Type of the returned value.</typeparam>
public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) =>
        new() { Succeeded = true, Value = value };

    public static new Result<T> Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };
}
