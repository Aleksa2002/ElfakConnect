using System;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Endpoints;

public class Refresh : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/refresh", Handle)
        .WithSummary("Refreshes access tokens");

    public record Response(string Id, string Username, string Email, DateTime CreatedAt);

    private static async Task<IResult> Handle(
        IAccessTokenService accessTokenService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Problem(AuthErrors.MissingRefreshToken);
        }

        var userResult = await accessTokenService.VerifyAndRevokeRefreshToken(refreshToken);
       
        return await userResult.MatchAsync(
            async (user) =>
            {
                await accessTokenService.GenerateBothTokensAndSetCookies(user);
                return Results.Ok(new Response(
                    user.Id.ToString(),
                    user.Username,
                    user.Email,
                    user.CreatedAtUtc));
            },
            error => Task.FromResult(Results.Problem(error)));
    }
}
