using System.Net.Http.Json;
using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;

namespace Inventario.Infrastructure.Services
{
    public class ProductoApiClient : IProductoApiClient
    {
        private readonly HttpClient _httpClient;

        public ProductoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductoDto?> ObtenerProductoPorIdAsync(int productoId)
        {
            return await _httpClient.GetFromJsonAsync<ProductoDto>($"/productos/{productoId}");
        }
    }
}
