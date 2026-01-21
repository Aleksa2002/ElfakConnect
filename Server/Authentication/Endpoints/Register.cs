using System;
using Server.Common;
using Server.Common.Extensions;
using Server.Data.Interfaces;
using Server.Authentication.Interfaces;

namespace Server.Authentication.Endpoints;

public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/register", Handle)
        .WithSummary("Regesters a new user")
        .WithRequestValidation<Request>();

    public record Request(string Email, string Password);
    public record Response(string Id, string Email);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            // RuleFor(x => x.Username)
            // .NotEmpty().WithMessage("Username is required")
            // .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
            // .MaximumLength(20).WithMessage("Username must be at most 20 characters long")
            // .Matches("^[a-zA-Z0-9_]+$")
            // .WithMessage("Username can only contain letters, numbers, and underscores");
            RuleFor(x => x.Email).ElfakEmail();
            RuleFor(x => x.Password).Password();
        }
    }

    private static async Task<IResult> Handle(
        Request request,
        IUserRepository userRepository,
        IVerificationService verificationService,
        IAccessTokenService accessTokenService,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        CancellationToken cancellationToken)
    {
        var userResult = await userRepository.CreateAsync(
            request.Email,
            passwordHash: passwordHasher.Hash(request.Password));

        if (!userResult.IsSuccess)
        {
            return ApiResponse.Problem(userResult.Error);
        }
        var user = userResult.Value;

        var code = await verificationService.CreateCode(user.Id);
        
        await Task.WhenAll(
            emailService.SendVerificationEmailAsync(user, code),
            accessTokenService.GenerateBothTokensAndSetCookies(user));


        return ApiResponse.Ok(new Response(
            user.Id.ToString(),
            user.Email));
    }
}