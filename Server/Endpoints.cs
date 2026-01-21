using System;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Server.Authentication.Endpoints;
using Server.Common;
using Server.Authentication.Endpoints.Microsoft;
using Server.Users.Endpoints;

namespace Server;

public static class Endpoints
{
    private static readonly OpenApiSecurityScheme securityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("/api")
            .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapUsersEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication");
            
        endpoints.MapPublicGroup()
            .MapEndpoint<Login>()
            .MapEndpoint<Register>()
            .MapEndpoint<LoginMicrosoft>()
            .MapEndpoint<LoginMicrosoftCallback>()
            .MapEndpoint<RegisterMicrosoftCallback>()
            .MapEndpoint<RegisterMicrosoft>()
            .MapEndpoint<Refresh>();    

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<Verify>()
            .MapEndpoint<CompleteOnboarding>();
    }

     private static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/users")
            .WithTags("Users");

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<GetCurrentUser>();
    }
    
    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
