using Carter;
//using FootballDataPlatform.Api.Teams;
using FootballDataPlatform.Application;
using FootballDataPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
app.MapCarter();
// app.MapCreateTeamEndpoint();
// app.MapGetTeamEndpoint();

app.Run();

public partial class Program;