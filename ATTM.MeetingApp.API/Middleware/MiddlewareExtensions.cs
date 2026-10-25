namespace ATTM.MeetingApp.API.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        => builder.UseMiddleware<ExceptionHandlerMiddleware>();
}
