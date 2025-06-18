using Producto.Application.DTOs;
using Producto.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ProductoEntity = Producto.Domain.Entities.Producto;
using Producto.Application.Interfaces;

namespace Producto.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ProductoDbContext _context;

        public ProductoService(ProductoDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductoDto>> ListarAsync(bool? esActivo = null)
        {
            var query = _context.Productos.AsQueryable();

            if (esActivo.HasValue)
                query = query.Where(p => p.EsActivo == esActivo.Value);

            return await query
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Descripcion = p.Descripcion,
                    EsActivo = p.EsActivo
                })
                .ToListAsync();
        }

        public async Task<ProductoDto?> ObtenerPorIdAsync(int id, bool? esActivo = null)
        {
            var query = _context.Productos.AsQueryable();

            if (esActivo.HasValue)
                query = query.Where(p => p.EsActivo == esActivo.Value);

            var producto = await query.FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return null;

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
                EsActivo = producto.EsActivo
            };
        }


        public async Task<int> CrearAsync(ProductoDto dto)
        {
            var producto = new ProductoEntity
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Descripcion = dto.Descripcion
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto.Id;
        }

        public async Task<bool> ActualizarAsync(ProductoDto dto)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (producto == null)
                throw new InvalidOperationException("El producto no existe.");

            if (!producto.EsActivo)
                throw new InvalidOperationException("No se puede actualizar un producto inactivo.");

            producto.Nombre = dto.Nombre;
            producto.Precio = dto.Precio;
            producto.Descripcion = dto.Descripcion;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<bool> ActualizarEstadoAsync(int id, bool esActivo)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null)
                throw new InvalidOperationException("El producto no existe.");

            if (producto.EsActivo == esActivo)
                throw new InvalidOperationException($"El producto ya está {(esActivo ? "activo" : "inactivo")}.");

            producto.EsActivo = esActivo;
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
