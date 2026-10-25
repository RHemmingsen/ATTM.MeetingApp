using System.Text.Json.Serialization;
using ATTM.MeetingApp.API.Middleware;
using ATTM.MeetingApp.Application;
using ATTM.MeetingApp.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Configuration comes from the default sources only (appsettings, user-secrets in Development,
// environment variables). The host supplies secrets through them, e.g. ConnectionStrings__Database.

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCustomExceptionHandler();

// Swagger UI is the demo's frontend, so it is enabled in every environment.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
