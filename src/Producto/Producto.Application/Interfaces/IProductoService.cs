using Producto.Application.DTOs;

namespace Producto.Application.Interfaces
{
    public interface IProductoService
    {
        Task<int> CrearAsync(ProductoDto dto);
        Task<List<ProductoDto>> ListarAsync(bool? esActivo = null);
        Task<ProductoDto?> ObtenerPorIdAsync(int id, bool? esActivo = null);
        Task<bool> ActualizarAsync(ProductoDto dto);
        Task<bool> ActualizarEstadoAsync(int id, bool esActivo);
    }
}
