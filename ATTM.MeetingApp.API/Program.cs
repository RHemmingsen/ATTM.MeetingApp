using ATTM.MeetingApp.Persistence;
using ATTM.MeetingApp.Application;
using Microsoft.EntityFrameworkCore;
using ATTM.MeetingApp.API.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
try
{
    var context = scope.ServiceProvider.GetService<MeetingDbContext>();
    if (context == null)
    {
        return;
    }
    //Undgå som udgangspunkt at bruge EnsureDeletedAsync i produktion
    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler()
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

app.Run();
