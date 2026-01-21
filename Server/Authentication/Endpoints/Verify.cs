using System;
using System.Security.Claims;
using Server.Common;
using Server.Common.Extensions;
using Server.Authentication.Interfaces;

namespace Server.Authentication.Endpoints;

public class Verify : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/verify", Handle)
        .WithSummary("Verifies a user's email address");
    
    public record Request(string Code);
    public record Response(string Id, string Email);

    private static async Task<IResult> Handle(
        Request request,
        ClaimsPrincipal claimsPrincipal,
        IVerificationService verificationService)
    {
        var userId = claimsPrincipal.GetUserId();
        var verifyResult = await verificationService.VerifyUser(userId, request.Code);
        
        return verifyResult.Match(
            user => ApiResponse.Ok(new Response(
                user.Id.ToString(),
                user.Email)),
            error => ApiResponse.Problem(error));
    }
}
