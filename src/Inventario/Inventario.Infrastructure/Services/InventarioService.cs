using Inventario.Application.DTOs;
using Inventario.Application.Services;
using InventarioEntity = Inventario.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly InventarioDbContext _context;

        public InventarioService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearInventarioAsync(InventarioDto dto)
        {
            var inventario = new InventarioEntity
            {
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                FechaActualizacion = DateTime.UtcNow
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();
            return inventario.Id;
        }

        public async Task<InventarioDto?> ObtenerInventarioPorProductoIdAsync(int productoId)
        {
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.ProductoId == productoId);
            if (inventario == null) return null;

            return new InventarioDto
            {
                ProductoId = inventario.ProductoId,
                Cantidad = inventario.Cantidad
            };
        }

        public async Task<List<InventarioDto>> ListarAsync()
        {
            return await _context.Inventarios
                .Select(i => new InventarioDto
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
                })
                .ToListAsync();
        }
    }
}
