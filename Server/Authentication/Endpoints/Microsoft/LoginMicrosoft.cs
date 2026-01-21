using System;
using Microsoft.AspNetCore.Authentication;
using Server.Common;
using Server.Common.Extensions;

namespace Server.Authentication.Endpoints.Microsoft;

public class LoginMicrosoft : IEndpoint
{
     public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login/microsoft", Handle)
        .WithSummary("Logs in a user with Microsoft");

    public record Request(string SuccessReturnUrl, string FailureReturnUrl);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.SuccessReturnUrl).Url();
            RuleFor(x => x.FailureReturnUrl).Url();
        }
    }

    private static IResult Handle(
        [AsParameters] Request request,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        var redirectUri = linkGenerator.GetPathByName(
            LoginMicrosoftCallback.EndpointName,
            values: new
            {
                SuccessReturnUrl = request.SuccessReturnUrl,
                FailureReturnUrl = request.FailureReturnUrl
            });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        };

        return Results.Challenge(properties, ["Microsoft"]);
    }
}
