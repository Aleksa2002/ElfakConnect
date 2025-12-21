using System;

namespace Server.Common.Api.Extensions;

public static class ResultExtensions
{
    // For non-generic Result (void operations)
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }
    // For generic Result<T> (value returning operations)
    public static TOut Match<TValue, TOut>(
        this Result<TValue> result,
        Func<TValue, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }
}
