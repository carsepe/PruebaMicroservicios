
using Inventario.Application.DTOs;

namespace Inventario.Application.Services
{
    public interface IInventarioService
    {
        Task<int> CrearInventarioAsync(InventarioDto dto);
        Task<InventarioDto?> ObtenerInventarioPorProductoIdAsync(int productoId);
        Task<List<InventarioDto>> ListarAsync();
    }
}
