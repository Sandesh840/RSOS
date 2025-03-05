using System.Net;
using Application.DTOs.Base;
using Application.Exceptions;
using Newtonsoft.Json;

namespace RSOS.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        ResponseDTO<object> problem = new();
        
        switch (ex)
        {
            case BadRequestException badRequestException:
            {
                statusCode = HttpStatusCode.BadRequest;
                problem = new ResponseDTO<object>
                {
                    Status = "Bad Request",
                    StatusCode = statusCode,
                    Message = badRequestException.Message,
                    Result = nameof(BadRequestException),
                };
                break;
            }

            case NotFoundException notFound:
            {
                statusCode = HttpStatusCode.NotFound;
                problem = new ResponseDTO<object>
                {
                    Status = "Not Found",
                    StatusCode = statusCode,
                    Message = notFound.Message,
                    Result = nameof(NotFoundException),
                };
                break;
            }

            default:
            {
                problem = new ResponseDTO<object>
                {
                    Status = "Internal Server Error",
                    Message = ex.Message,
                    StatusCode = statusCode,
                    Result = nameof(HttpStatusCode.InternalServerError),
                };
                break;
            }
        }

        httpContext.Response.StatusCode = (int)statusCode;
        
        var logMessage = JsonConvert.SerializeObject(problem);
        
        _logger.LogError(logMessage);
        
        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}