using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Inventario.Application.DTOs;
using Inventario.Infrastructure.Services;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Inventario.Tests.Services
{
    public class ProductoApiClientTests
    {
        private ProductoApiClient CrearClienteConRespuesta(HttpResponseMessage response)
        {
            // Cargar configuración desde appsettings.json si está presente
            string baseUrl = "https://localhost:7109"; // fallback por defecto
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                baseUrl = configuration["Apis:Producto"] ?? baseUrl;
            }
            catch
            {
                // Silenciar errores de configuración para pruebas unitarias
            }

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseUrl)
            };

            return new ProductoApiClient(httpClient);
        }

        [Fact]
        public async Task ObtenerProductoPorIdAsync_DeberiaRetornarProducto_SiExiste()
        {
            var producto = new ProductoDto
            {
                Id = 1,
                Nombre = "Producto de prueba",
                Precio = 1000,
                EsActivo = true
            };

            var json = JsonSerializer.Serialize(producto);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

            var cliente = CrearClienteConRespuesta(response);

            var resultado = await cliente.ObtenerProductoPorIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal("Producto de prueba", resultado?.Nombre);
        }

        [Fact]
        public async Task ObtenerProductoPorIdAsync_DeberiaRetornarNull_SiNoExiste()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound);
            var cliente = CrearClienteConRespuesta(response);

            var resultado = await cliente.ObtenerProductoPorIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerProductoPorIdAsync_DeberiaLanzarExcepcion_SiStatusNoEsExitoso()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var cliente = CrearClienteConRespuesta(response);

            await Assert.ThrowsAsync<HttpRequestException>(() => cliente.ObtenerProductoPorIdAsync(1));
        }
    }
}
