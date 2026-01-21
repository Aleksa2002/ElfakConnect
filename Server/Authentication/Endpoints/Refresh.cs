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

    private static async Task<IResult> Handle(
        IAccessTokenService accessTokenService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var refreshToken = httpContext.Request.Cookies["REFRESH_TOKEN"];
        
        if (string.IsNullOrEmpty(refreshToken))
        {
            return ApiResponse.Problem(AuthErrors.MissingRefreshToken);
        }

        var userResult = await accessTokenService.VerifyAndRevokeRefreshToken(refreshToken);
       
        return await userResult.MatchAsync(
            async (user) =>
            {
                await accessTokenService.GenerateBothTokensAndSetCookies(user);
                return ApiResponse.Ok();
            },
            error => Task.FromResult(ApiResponse.Problem(error)));
    }
}
