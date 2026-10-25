using System.Text.Json.Serialization;
using ATTM.MeetingApp.API.Middleware;
using ATTM.MeetingApp.Application;
using ATTM.MeetingApp.Persistence;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configuration: appsettings + user-secrets (Development) are loaded by default.
// Key Vault secrets override them when a vault is configured; name secrets like ConnectionStrings--Database.
if (builder.Configuration["KeyVault:VaultUri"] is { Length: > 0 } vaultUri)
{
    builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());
}

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
