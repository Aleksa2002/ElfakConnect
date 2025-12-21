using System;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Endpoints;

public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/register", Handle)
        .WithSummary("Regesters a new user")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Email);
    public record Response(string Id, string Username, string Email, DateTime CreatedAt);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 20)
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain alphanumeric characters and underscores");
            RuleFor(x => x.Email).ElfakEmail();
            RuleFor(x => x.Password).Password();
        }
    }

    private static async Task<IResult> Handle(
        Request request,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {

        var userResult = await authenticationService.Register(
            request.Username,
            request.Password,
            request.Email,
            cancellationToken);
        
        return userResult.Match(
            user => Results.Ok(new Response(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.CreatedAt)),
            error => Results.Problem(error.ToProblemDetails()));
    }
}