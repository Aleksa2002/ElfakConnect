using System;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithSummary("Logs in a user")
        .WithRequestValidation<Request>();

    public record Request(string Email, string Password);
    public record Response(string Id, string Username, string Email, DateTime CreatedAt);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Email).ElfakEmail();
            RuleFor(x => x.Password).Password();
        }
    }

    private static async Task<IResult> Handle(
        Request request,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        var userResult = await authenticationService.Login(request.Email, request.Password, cancellationToken);
        
        return userResult.Match(
            user => Results.Ok(new Response(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.CreatedAt)),
            error => Results.Problem(error.ToProblemDetails()));
    }

}
