using Producto.Application.DTOs;

namespace Producto.Application.Interfaces
{
    public interface IProductoService
    {
        Task<int> CrearProductoAsync(ProductoDto dto);
        Task<ProductoDto?> ObtenerProductoPorIdAsync(int id);
        Task<List<ProductoDto>> ListarAsync();

    }
}
