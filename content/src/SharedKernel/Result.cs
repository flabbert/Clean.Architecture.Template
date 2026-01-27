using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Clean.Architecture.Template.SharedKernel;

public class Result
{
    protected Result(bool isSuccess, Error error, string? message = null)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Error = error;
        Message = message;

    }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; }
    public Error Error { get; }
    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result Failure(Error error) => new(false, error, error.Description);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error, error.Description);
}

public class Result<TValue>(TValue? value, bool isSuccess, Error error, string? message = null) : Result(isSuccess, error, message)
{
    [NotNull]
    public TValue Value => IsSuccess ? value! : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    public static Result<TValue> ValidationFailure(Error error) => new(default, false, error);
}
