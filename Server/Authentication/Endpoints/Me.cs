using System;
using System.Security.Claims;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;
using Server.Data.Repositories;

namespace Server.Authentication.Endpoints;

public class Me : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/me", Handle)
        .WithSummary("Gets the current user");

    public record Response(string Id, string Username, string Email, DateTime CreatedAt);
    private static async Task<IResult> Handle(
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository)
    {
        var userId = claimsPrincipal.GetUserId();
        var user = await userRepository.FindByIdAsync(userId);

        return user != null ?
            Results.Ok(new Response(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.CreatedAtUtc)) :
            Results.Problem(AuthErrors.InvalidAccessToken);
    }
}
