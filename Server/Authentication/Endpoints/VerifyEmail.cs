using System;
using System.Security.Claims;
using Server.Common;
using Server.Common.Extensions;
using Server.Authentication.Interfaces;

namespace Server.Authentication.Endpoints;

public class VerifyEmail : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/verify-email", Handle)
        .WithSummary("Verifies a user's email address");
    
    public record Request(string Code);
    public record Response(string Id, string Username, string Email, DateTime CreatedAt);

    private static async Task<IResult> Handle(
        Request request,
        ClaimsPrincipal claimsPrincipal,
        IVerificationService verificationService)
    {
        var userId = claimsPrincipal.GetUserId();
        var verifyResult = await verificationService.VerifyUser(userId, request.Code);
        
        return verifyResult.Match(
            user => Results.Ok(new Response(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.CreatedAtUtc)),
            error => Results.Problem(error));
    }
}
