using System;
using MongoDB.Driver;
using Server.Authentication.Services;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/signup", Handle)
        .WithSummary("Creates a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Email);
    public record Response(string Id, string Username, string Email, DateTime CreatedAt);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(
        Request request,
        IMongoDatabase database,
        IPasswordHasher passwordHasher,
        CancellationToken cancellationToken)
    {
        var usersCollection = database.GetCollection<User>(User.CollectionName);

        var emailExists = await usersCollection
            .Find(x => x.Email == request.Email)
            .AnyAsync(cancellationToken);

        if (emailExists)
        {
            var problemDetails = AuthErrors.EmailNotUnique(request.Email).ToProblemDetails();
            return TypedResults.Problem(problemDetails);
        }    
        
        var usernameExists = await usersCollection
            .Find(x => x.Username == request.Username)
            .AnyAsync(cancellationToken);

        if (usernameExists)
        {
            var problemDetails = AuthErrors.UsernameNotUnique(request.Username).ToProblemDetails();
            return TypedResults.Problem(problemDetails);
        }    
        
        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            Email = request.Email
        };
        await usersCollection.InsertOneAsync(newUser, cancellationToken: cancellationToken);

        return TypedResults.Ok(new Response
        (
            newUser.Id.ToString(),
            newUser.Username,
            newUser.Email,
            newUser.CreatedAt
        ));
    }
}