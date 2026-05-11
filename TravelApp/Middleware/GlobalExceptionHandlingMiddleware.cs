using System.Runtime.ExceptionServices;
using TravelApp.Dtos;
using TravelApp.Exceptions;

namespace TravelApp.Middleware;

public sealed class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            logger.LogError(exception, "An exception occurred after the response started.");
            ExceptionDispatchInfo.Capture(exception).Throw();
        }

        var statusCode = StatusCodes.Status500InternalServerError;
        var error = "An unexpected error occurred.";
        var type = nameof(Exception);

        if (exception is TravelAppException travelAppException)
        {
            statusCode = travelAppException switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            error = travelAppException.Message;
            type = travelAppException.GetType().Name;
        }
        else
        {
            logger.LogError(exception, "An unhandled exception occurred.");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ErrorResponse(error, type));
    }
}
