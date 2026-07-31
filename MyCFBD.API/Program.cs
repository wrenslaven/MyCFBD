using Scalar.AspNetCore;
using MyCFBD.API.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITeamList, TeamList>(); // The same for every request https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { } // Needed to expose this program to the other projects


