using Server.Data;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.Configure<MongoDBOptions>(builder.Configuration.GetSection("MongoDB"));


app.MapGet("/", () => "Hello World!");

app.Run();
