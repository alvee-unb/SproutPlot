namespace SproutPlot.Application.Common.Results;

/// <summary>Categorises a failed result so callers can map it to the right HTTP status.</summary>
public enum ResultErrorType
{
    None = 0,
    Validation,
    NotFound,
    Conflict,
}

/// <summary>
/// Lightweight operation result used to model expected success/failure
/// outcomes (e.g. invalid credentials, missing resources) without throwing.
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }

    public ResultErrorType ErrorType { get; init; } = ResultErrorType.None;

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };

    public static Result Failure(params string[] errors) =>
        new() { Succeeded = false, ErrorType = ResultErrorType.Validation, Errors = errors };

    public static Result NotFound(params string[] errors) =>
        new() { Succeeded = false, ErrorType = ResultErrorType.NotFound, Errors = errors };

    public static Result Conflict(params string[] errors) =>
        new() { Succeeded = false, ErrorType = ResultErrorType.Conflict, Errors = errors };
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
        new() { Succeeded = false, ErrorType = ResultErrorType.Validation, Errors = errors };

    public static new Result<T> NotFound(params string[] errors) =>
        new() { Succeeded = false, ErrorType = ResultErrorType.NotFound, Errors = errors };

    public static new Result<T> Conflict(params string[] errors) =>
        new() { Succeeded = false, ErrorType = ResultErrorType.Conflict, Errors = errors };
}
