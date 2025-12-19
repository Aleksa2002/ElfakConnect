using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Common;
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

    private static async Task<Results<Ok<Response>, ProblemHttpResult>> Handle(Request request, IMongoDatabase database, CancellationToken cancellationToken)
    {
        var usersCollection = database.GetCollection<User>(User.CollectionName);
        var userExists = await usersCollection
            .Find(x => x.Username == request.Username || x.Email == request.Email)
            .AnyAsync(cancellationToken);

        if (userExists)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Username or email already in use",
                Status = StatusCodes.Status400BadRequest,
                Type = "https://example.com/probs/validation"
            };
            return TypedResults.Problem(problemDetails);
        }

        await usersCollection.InsertOneAsync(new User
        {
            Username = request.Username,
            PasswordHash = request.Password, // TODO: Hash password
            Email = request.Email
        }, cancellationToken: cancellationToken);

        var createdUser = await usersCollection
            .Find(x => x.Username == request.Username)
            .FirstAsync(cancellationToken);

        return TypedResults.Ok(new Response
        (
            createdUser.Id.ToString(),
            createdUser.Username,
            createdUser.Email,
            createdUser.CreatedAt
        ));
    }
}