using System;
using System.Diagnostics.CodeAnalysis;

namespace Server.Common;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None )||
             (!isSuccess && error == Error.None))
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error); 
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;
    protected internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");
    public static Result<T> Create(T? value)
    {
        return value is not null 
            ? Success(value) 
            : Failure<T>(Error.NullValue);
    }

    public static implicit operator Result<T>(T? value) => Create(value);
    public static implicit operator Result<T>(Error error) => Failure<T>(error);
    public static Result<T> Success(T value) => new(value, true, Error.None);
}
