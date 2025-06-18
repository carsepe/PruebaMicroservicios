using Producto.Application.DTOs;

namespace Producto.Application.Services
{
    public interface IProductoService
    {
        Task<int> CrearProductoAsync(ProductoDto dto);
        Task<ProductoDto?> ObtenerProductoPorIdAsync(int id);
    }
}
