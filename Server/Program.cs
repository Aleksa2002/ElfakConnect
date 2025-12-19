global using Server.Data;
global using Server.Data.Models;
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
using Server;
using Scalar.AspNetCore;
using Server.Authentication.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
        ctx.ProblemDetails.Extensions.TryAdd("requestId", ctx.HttpContext.TraceIdentifier);
        Activity? activity = ctx.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        ctx.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);

    };
});

builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMongoDB(builder.Configuration);

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();

app.UseExceptionHandler();

app.Run();
