// InfoDynamics.API/Middleware/ExceptionMiddleware.cs
using InfoDynamics.Aplicacion.CustomException;
using System.Text.Json;
using InfoDynamics.Aplicacion.CustomException;

namespace InfoDynamics.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción capturada: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                EntityNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
                UnauthorizedException e => (StatusCodes.Status401Unauthorized, e.Message),
                ConflictException e => (StatusCodes.Status409Conflict, e.Message),
                InvalidOperationException e => (StatusCodes.Status409Conflict, e.Message),
                ArgumentException e => (StatusCodes.Status409Conflict, e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado.")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var body = JsonSerializer.Serialize(new { message });
            return context.Response.WriteAsync(body);
        }
    }
}