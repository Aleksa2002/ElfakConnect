using System;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;
using Server.Data.Repositories;

namespace Server.Authentication.Endpoints;

public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/register", Handle)
        .WithSummary("Regesters a new user")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Email);
    public record Response(string Id, string Username, string Email, DateTime CreatedAtUtc);
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
        IUserRepository userRepository,
        IVerificationService verificationService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
        var userResult = await userRepository.CreateAsync(
            request.Username,
            request.Email,
            passwordHasher.Hash(request.Password));

        if (!userResult.IsSuccess)
        {
            return Results.Problem(userResult.Error);
        }
        var user = userResult.Value;

        var code = await verificationService.CreateCode(user.Id);
        await emailService.SendVerificationEmailAsync(user, code);

        return Results.Ok(new Response(
            user.Id.ToString(),
            user.Username,
            user.Email,
            user.CreatedAtUtc));
    }
}