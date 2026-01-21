using System;
using System.Security.Claims;
using Server.Common;
using Server.Common.Extensions;
using Server.Data.Interfaces;

namespace Server.Authentication.Endpoints;

public class CompleteOnboarding : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/complete-onboarding", Handle)
        .WithSummary("Completes the onboarding process for a user");

    public record Request(string Username);
    public record Response(string Id, string Username, string Email, DateTime CreatedAt);
    
    private static async Task<IResult> Handle(
        Request request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository)
    {
        var userId = claimsPrincipal.GetUserId();
        var user = await userRepository.FindByIdAsync(userId);

        if (user == null)
        {
            return ApiResponse.Problem(AuthErrors.InvalidCredentials);
        }

        return ApiResponse.Ok(new Response(
            user.Id.ToString(),
            user.Username!,
            user.Email,
            user.CreatedAtUtc));
    }
}
