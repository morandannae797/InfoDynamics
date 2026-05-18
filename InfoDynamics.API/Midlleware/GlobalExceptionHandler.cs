using InfoDynamics.Aplicacion.CustomException;
using Microsoft.AspNetCore.Diagnostics; 

namespace InfoDynamics.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            
            _logger.LogError(exception, "Excepción capturada: {Message}", exception.Message);

          
            var (statusCode, message) = exception switch
            {
                EntityNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
                UnauthorizedException e => (StatusCodes.Status401Unauthorized, e.Message),
                ConflictException e => (StatusCodes.Status409Conflict, e.Message),
                InvalidOperationException e => (StatusCodes.Status409Conflict, e.Message),
                ArgumentException e => (StatusCodes.Status409Conflict, e.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrio un error inesperado.")
            };

          
            context.Response.StatusCode = statusCode;

          
            await context.Response.WriteAsJsonAsync(new { message }, cancellationToken);

            
            return true;
        }
    }
}