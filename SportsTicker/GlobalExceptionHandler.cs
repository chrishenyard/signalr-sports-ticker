using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SportsTicker;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, exception.Message);
        return await HandleApiExceptionAsync(httpContext, exception, token);
    }

    private static async Task<bool> HandleApiExceptionAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ProblemDetails
        {
            Title = exception.GetType().Name,
            Detail = exception.Message,
            Status = httpContext.Response.StatusCode,
            Instance = httpContext.Request.Path
        };

        var json = JsonSerializer.Serialize(response);

        await httpContext.Response.WriteAsync(json, token);
        return true;
    }
}
