using System.Net;
using System.Text.Json;

namespace BankAccountGateway.Middlewares;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionMessageAsync(context);
        }
    }

    private static Task HandleExceptionMessageAsync(HttpContext context)
    {
        var statusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        
        var errorMessage = "An error occurred while processing your request.";

        var result = JsonSerializer.Serialize(
            new
            {
                StatusCode = statusCode,
                ErrorMessage = errorMessage
            });
        return context.Response.WriteAsync(result);
    }
}