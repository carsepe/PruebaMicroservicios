using System.Net;
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
            var response = await _httpClient.GetAsync($"/productos/{productoId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductoDto>();
        }

    }
}
