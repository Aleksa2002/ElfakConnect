using System;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Endpoints;

public class Refresh : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh", Handle)
        .WithSummary("Refreshes access tokens");

    private static async Task<IResult> Handle(
        IAuthenticationService authenticationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];
        var result =  await authenticationService.RefreshAccessTokens(refreshToken, cancellationToken);
        return result.Match(
            () => Results.Ok(),
            error => Results.Problem(error.ToProblemDetails()));
    }
}
