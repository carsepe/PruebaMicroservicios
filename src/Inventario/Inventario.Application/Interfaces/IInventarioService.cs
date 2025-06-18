using Inventario.Application.DTOs;

namespace Inventario.Application.Interfaces
{
    public interface IInventarioService
    {
        Task<int> CrearAsync(InventarioDto dto);
        Task<List<InventarioDto>> ListarAsync(bool? esActivo = null);
        Task<InventarioDto?> ObtenerInventarioPorProductoIdAsync(int productoId, bool? esActivo = null);
        Task<bool> ActualizarAsync(InventarioDto dto);
        Task<bool> ActualizarEstadoAsync(int id, bool esActivo);
    }
}
