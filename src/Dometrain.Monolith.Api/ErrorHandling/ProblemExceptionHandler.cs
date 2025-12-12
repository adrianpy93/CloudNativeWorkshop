#region

using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

#endregion

namespace Dometrain.Monolith.Api.ErrorHandling;

public class ProblemExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException) return false;

        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails =
            {
                Title = "A problem occurred",
                Detail = string.Join(", ", validationException.Errors),
                Type = nameof(ValidationException)
            },
            Exception = exception
        });
    }
}