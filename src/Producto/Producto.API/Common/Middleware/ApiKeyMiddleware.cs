using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Producto.API.Common.Middleware
{

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsJsonAsync(new
                {
                    errors = new[] {
                        new {
                            status = "401",
                            title = "API Key requerida",
                            detail = "No se proporcionó la API Key en el encabezado 'X-API-KEY'."
                        }
                    }
                });
                return;
            }

            var apiKey = configuration["ApiKey"];

            if (apiKey is null || !apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsJsonAsync(new
                {
                    errors = new[] {
                        new {
                            status = "403",
                            title = "API Key inválida",
                            detail = "La clave proporcionada no es válida."
                        }
                    }
                });
                return;
            }

            await _next(context);
        }
    }
}
