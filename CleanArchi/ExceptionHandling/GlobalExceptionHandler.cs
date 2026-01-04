using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Web.ExceptionHandling
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An unhandled exception occurred.");

            // If the response has already started, we cannot modify it
            if (httpContext.Response.HasStarted)
            {
                logger.LogWarning("The response has already started, cannot write error response.");
                return true;
            }

            try
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.GetType().ToString(),
                    Detail = exception.Message
                };

                return await problemDetailsService.TryWriteAsync(
                    new ProblemDetailsContext()
                    {
                        HttpContext = httpContext,
                        Exception = exception,
                        ProblemDetails = problemDetails
                    }
                );
            }
            catch (Exception writeException)
            {
                logger.LogError(writeException, "Failed to write error response.");
            }

            return true;
        }
    }
}
