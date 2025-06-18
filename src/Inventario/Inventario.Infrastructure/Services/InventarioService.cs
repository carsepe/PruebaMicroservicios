using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using InventarioEntity = Inventario.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly InventarioDbContext _context;
        private readonly IProductoApiClient _productoApi;


        public InventarioService(InventarioDbContext context, IProductoApiClient productoApi)
        {
            _context = context;
            _productoApi = productoApi;
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

        public async Task<CompraResultadoDto> ProcesarCompraAsync(CompraDto dto)
        {
            var producto = await _productoApi.ObtenerProductoPorIdAsync(dto.ProductoId);
            if (producto == null)
                return new CompraResultadoDto { Exito = false, Mensaje = "Producto no encontrado." };

            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.ProductoId == dto.ProductoId);
            if (inventario == null)
                return new CompraResultadoDto { Exito = false, Mensaje = "Inventario no encontrado." };

            if (inventario.Cantidad < dto.Cantidad)
                return new CompraResultadoDto { Exito = false, Mensaje = "Stock insuficiente." };

            inventario.Cantidad -= dto.Cantidad;
            inventario.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new CompraResultadoDto { Exito = true, Mensaje = "Compra procesada correctamente." };
        }

    }
}
