using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportHive.Exceptions;
namespace OperateExseption
{
   public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        ProblemDetails problem;

        switch (ex)
        {
            case NotFoundException notFound:
                problem = new ProblemDetails
                {
                    Title = "Not Found",
                    Detail = notFound.Message,
                    Status = StatusCodes.Status404NotFound
                };
                break;

            case ValidationException validation:
                problem = new ValidationProblemDetails
                {
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest
                };
                break;

            case UnauthorizedAccessException:
                problem = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized
                };
                break;

            default:
                problem = new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError
                };
                break;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? 500;

        return context.Response.WriteAsJsonAsync(problem);
    }
}

}
