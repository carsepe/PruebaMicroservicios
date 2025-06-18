using Producto.Application.DTOs;
using Producto.Application.Services;
using ProductoEntity = Producto.Domain.Entities.Producto;
using Producto.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Producto.Infrastructure.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ProductoDbContext _context;

        public ProductoService(ProductoDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearProductoAsync(ProductoDto dto)
        {
            var producto = new ProductoEntity
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Descripcion = dto.Descripcion
            };

            _context.Productos.Add(producto);


            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto.Id;
        }

        public async Task<ProductoDto?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null) return null;

            return new ProductoDto
            {
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion
            };
        }
    }
}
