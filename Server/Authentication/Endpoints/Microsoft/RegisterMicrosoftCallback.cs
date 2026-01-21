using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Server.Authentication.Interfaces;
using Server.Common;
using Server.Common.Extensions;
using Server.Data.Interfaces;

namespace Server.Authentication.Endpoints.Microsoft;

public class RegisterMicrosoftCallback : IEndpoint
{
    public static readonly string EndpointName = "RegisterMicrosoftCallback";
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/register/microsoft/callback", Handle)
        .WithSummary("Microsoft register callback")
        .WithName(EndpointName);

    public record Request(
        string OnboardingUrl,
        string FailureUrl,
        string AlreadyExistsUrl);

    private static async Task<IResult> Handle(
        [AsParameters] Request request,
        IUserRepository userRepository,
        IAccessTokenService accessTokenService,
        HttpContext httpContext)
    {
        var result = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
            return Results.Redirect(request.FailureUrl);
        }

        var email = result.Principal.FindFirst("upn")?.Value;
        var microsoftId = result.Principal.FindFirst("oid")?.Value;

        if (email == null || microsoftId == null)
        {
            return Results.Redirect(request.FailureUrl);
        }

        var user = await userRepository.FindByMicrosoftIdAsync(microsoftId);

        if (user != null)
        {
            await accessTokenService.GenerateBothTokensAndSetCookies(user);
            return Results.Redirect(request.AlreadyExistsUrl);
        }

        var userResult = await userRepository.CreateAsync(
            email,
            microsoftId: microsoftId,
            isVerified: true);

        return await userResult.MatchAsync(
            async user => {
                await accessTokenService.GenerateBothTokensAndSetCookies(user);
                return Results.Redirect(request.OnboardingUrl);
            },
            error => {
                var returnUrl = QueryHelpers.AddQueryString(request.FailureUrl, "foo", "bar");
                return Task.FromResult(Results.Redirect(returnUrl));
            }
        ); 
    }
}
