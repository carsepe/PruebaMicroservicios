using Inventario.API.Common.Errors;
using System.Net;
using System.Text.Json;

namespace Inventario.API.Common.Middleware
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var error = new JsonApiError
            {
                Title = "Error",
                Detail = "Ocurrió un error inesperado.",
                Status = "500"
            };

            switch (exception)
            {
                case InvalidOperationException ex:
                    error.Title = "Validación de negocio";
                    error.Detail = ex.Message;
                    error.Status = "400";
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentNullException ex:
                    error.Title = "Parámetro requerido ausente";
                    error.Detail = ex.Message;
                    error.Status = "422";
                    context.Response.StatusCode = 422;
                    break;

                case HttpRequestException ex:
                    error.Title = "Error al comunicar con otro servicio";
                    error.Detail = ex.Message;
                    error.Status = "502";
                    context.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                    break;

                default:
                    _logger.LogError(exception, "Error inesperado");
                    error.Title = "Error interno";
                    error.Detail = exception.Message;
                    error.Status = "500";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new { errors = new[] { error } });
            await context.Response.WriteAsync(result);
        }
    }
}
