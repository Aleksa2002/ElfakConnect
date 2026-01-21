using System;
using System.Security.Claims;
using Server.Authentication;
using Server.Common;
using Server.Common.Extensions;
using Server.Data.Interfaces;

namespace Server.Users.Endpoints;

public class GetCurrentUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/me", Handle)
        .WithSummary("Gets the current user");

    public record Response(string Id, string? Username, string Email, DateTime CreatedAt);
    private static async Task<IResult> Handle(
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository)
    {
        var userId = claimsPrincipal.GetUserId();
        var user = await userRepository.FindByIdAsync(userId);

        return user != null 
            ? ApiResponse.Ok(new Response(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.CreatedAtUtc)) 
            : ApiResponse.Problem(UserErrors.UserNotFound(userId));
    }
}