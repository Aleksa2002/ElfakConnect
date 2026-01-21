using System;
using Microsoft.AspNetCore.Authentication;
using Server.Common;
using Server.Common.Extensions;

namespace Server.Authentication.Endpoints.Microsoft;

public class LoginMicrosoftCallback : IEndpoint
{
    public static readonly string EndpointName = "LoginMicrosoftCallback";
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/login/microsoft", Handle)
        .WithSummary("Microsoft login callback")
        .WithRequestValidation<Request>();


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
            RegisterMicrosoftCallback.EndpointName,
            values: new
            {
                successReturnUrl = request.SuccessReturnUrl,
                failureReturnUrl = request.FailureReturnUrl
            });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUri,
        };

        return Results.Challenge(properties, ["Microsoft"]);
    }
}