using System;
using Server.Common;
using Server.Common.Extensions;
using Server.Authentication.Interfaces;

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
