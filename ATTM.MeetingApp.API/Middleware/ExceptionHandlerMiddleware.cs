using ATTM.MeetingApp.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ATTM.MeetingApp.API.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemAsync(context, exception);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var problem = new ProblemDetails();

        switch (exception)
        {
            case ValidationException validationException:
                problem.Status = StatusCodes.Status400BadRequest;
                problem.Title = validationException.Message;
                problem.Extensions["errors"] = validationException.ValidationErrors;
                break;
            case NotFoundException:
                problem.Status = StatusCodes.Status404NotFound;
                problem.Title = exception.Message;
                break;
            default:
                logger.LogError(exception, "Unhandled exception");
                problem.Status = StatusCodes.Status500InternalServerError;
                problem.Title = "An unexpected error occurred.";
                break;
        }

        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
    }
}
