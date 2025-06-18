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

        public async Task<int> CrearAsync(InventarioDto dto)
        {
            var producto = await _productoApi.ObtenerProductoPorIdAsync(dto.ProductoId);
            if (producto == null)
                throw new InvalidOperationException("El producto no existe.");

            if (producto.EsActivo == false)
                throw new InvalidOperationException("No se puede crear inventario para un producto inactivo.");

            var existente = await _context.Inventarios
                .AnyAsync(i => i.ProductoId == dto.ProductoId);

            if (existente)
                throw new InvalidOperationException("Ya existe inventario para este producto.");

            var inventario = new InventarioEntity
            {
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                FechaActualizacion = DateTime.UtcNow,
                EsActivo = true
            };

            _context.Inventarios.Add(inventario);
            await _context.SaveChangesAsync();
            return inventario.Id;
        }


        public async Task<bool> ActualizarAsync(InventarioDto dto)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.ProductoId == dto.ProductoId);

            if (inventario == null)
                throw new InvalidOperationException("El inventario no existe.");

            if (!inventario.EsActivo)
                throw new InvalidOperationException("No se puede actualizar un inventario inactivo.");

            inventario.Cantidad = dto.Cantidad;
            inventario.FechaActualizacion = DateTime.UtcNow;

            _context.Inventarios.Update(inventario);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<InventarioDto?> ObtenerInventarioPorProductoIdAsync(int productoId, bool? esActivo = null)
        {
            var query = _context.Inventarios.AsQueryable();
            if (esActivo.HasValue)
                query = query.Where(i => i.EsActivo == esActivo.Value);

            var inventario = await query.FirstOrDefaultAsync(i => i.ProductoId == productoId);
            if (inventario == null) return null;

            return new InventarioDto
            {
                ProductoId = inventario.ProductoId,
                Cantidad = inventario.Cantidad,
                EsActivo = inventario.EsActivo
            };
        }

        public async Task<List<InventarioDto>> ListarAsync(bool? esActivo = null)
        {
            var query = _context.Inventarios.AsQueryable();
            if (esActivo.HasValue)
                query = query.Where(i => i.EsActivo == esActivo.Value);

            return await query.Select(i => new InventarioDto
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                EsActivo = i.EsActivo
            }).ToListAsync();
        }

        public async Task<bool> ActualizarEstadoAsync(int id, bool esActivo)
        {
            var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.Id == id);
            if (inventario == null)
                throw new InvalidOperationException("El inventario no existe.");

            if (inventario.EsActivo == esActivo)
                throw new InvalidOperationException($"El inventario ya está {(esActivo ? "activo" : "inactivo")}.");

            inventario.EsActivo = esActivo;
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<CompraResultadoDto> ProcesarCompraAsync(CompraDto dto)
        {
            var producto = await _productoApi.ObtenerProductoPorIdAsync(dto.ProductoId);
            if (producto == null)
                return new CompraResultadoDto { Exito = false, Mensaje = "Producto no encontrado." };

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.ProductoId == dto.ProductoId && i.EsActivo);

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
