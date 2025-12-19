global using Server.Data;
global using Server.Data.Models;
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
using Server;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Server.Authentication.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddMongoDB(builder.Configuration);

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapEndpoints();

app.Run();
