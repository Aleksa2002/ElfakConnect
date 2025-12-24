using System;
using Server.Common;
using Server.Common.Extensions;
using Server.Data.Interfaces;
using Server.Authentication.Interfaces;

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
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenService accessTokenService
        )
    {
        var user = await userRepository.FindByEmailAsync(request.Email);

        if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Results.Problem(AuthErrors.InvalidCredentials);
        }

        await accessTokenService.GenerateBothTokensAndSetCookies(user);

        return Results.Ok(new Response(
            user.Id.ToString(),
            user.Username,
            user.Email,
            user.CreatedAtUtc));
    }
}
