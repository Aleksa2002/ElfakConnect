using System;
using Microsoft.AspNetCore.Authentication;
using Server.Common;
using Server.Common.Extensions;

namespace Server.Authentication.Endpoints.Microsoft;

public class RegisterMicrosoft : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/register/microsoft", Handle)
        .WithSummary("Regesters a new user with Microsoft")
        .WithRequestValidation<Request>();


    public record Request(
        string OnboardingUrl,
        string FailureUrl,
        string AlreadyExistsUrl);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.OnboardingUrl).Url();
            RuleFor(x => x.FailureUrl).Url();
            RuleFor(x => x.AlreadyExistsUrl).Url();
        }
    }

    private static IResult Handle(
        [AsParameters] Request request,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var redirectUri = linkGenerator.GetPathByName(
            RegisterMicrosoftCallback.EndpointName,
            values: new
            {
                OnboardingUrl = request.OnboardingUrl,
                FailureUrl = request.FailureUrl,
                AlreadyExistsUrl = request.AlreadyExistsUrl
            });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        };

        return Results.Challenge(properties, ["Microsoft"]);
    }
}
