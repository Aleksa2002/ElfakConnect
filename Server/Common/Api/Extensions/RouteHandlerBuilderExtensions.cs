using System;
using Server.Common.Api.Filters;

namespace Server.Common.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}
