global using Server.Data;
global using Server.Data.Models;
global using FluentValidation;
using Server;
using Scalar.AspNetCore;
using Server.Authentication.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using Server.Authentication.Interfaces;
using Server.Data.Interfaces;
using Server.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;


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

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMongoDB(builder.Configuration);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", options =>
    {
        options.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("http://localhost:5173");
    });
});

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie().AddOpenIdConnect("Microsoft", options =>
{
    options.Authority = "https://login.microsoftonline.com/f5501f2a-b2b9-4122-8f2d-aa0f0366d907";
    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] 
        ?? throw new InvalidOperationException("Microsoft ClientId is not configured.");
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] 
        ?? throw new InvalidOperationException("Microsoft ClientSecret is not configured.");

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.Scope.Add("profile");
    options.Scope.Add("email");
}
).AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection("Jwt")
        .Get<JwtOptions>() ?? throw new InvalidOperationException("JWT options are not configured properly.");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    MongoDBIndexInitializer.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.MapGet("/api/account/login/microsoft", ([FromQuery] string returnUrl, [FromServices] LinkGenerator linkGenerator,
    HttpContext context) =>
{
    var redirectUri = linkGenerator.GetPathByName(context, "MicrosoftLoginCallback") + $"?returnUrl={returnUrl}";
    var properties = new AuthenticationProperties
    {
        RedirectUri = redirectUri
    };

    return Results.Challenge(properties, new[] { "Microsoft" });
});

app.MapGet("/api/account/login/microsoft/callback", async ([FromQuery] string returnUrl,
    HttpContext context) =>
{
    var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    if (!result.Succeeded)
    {
        return Results.Unauthorized();
    }

    foreach (var claim in result.Principal.Claims)
    {
        Console.WriteLine($"{claim.Type}: {claim.Value}");
    }

    return Results.Redirect(returnUrl);

}).WithName("MicrosoftLoginCallback");

app.UseExceptionHandler();

app.Run();
