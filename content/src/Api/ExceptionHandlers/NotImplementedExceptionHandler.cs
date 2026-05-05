using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.ExceptionHandlers;

public sealed class NotImplementedExceptionHandler(ILogger<NotImplementedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotImplementedException)
            return false;

        logger.LogWarning(exception, "Not implemented: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status501NotImplemented,
            Title = "Not Implemented",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.2"
        };

        httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
