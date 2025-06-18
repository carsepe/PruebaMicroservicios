using Xunit;
using Producto.Application.DTOs;
using Producto.Infrastructure.Data;
using Producto.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ProductoEntity = Producto.Domain.Entities.Producto;
using System;
using System.Threading.Tasks;

namespace Producto.Tests.Services
{
    public class ProductoServiceTests
    {
        private readonly ProductoDbContext _context;
        private readonly ProductoService _service;

        public ProductoServiceTests()
        {
            var options = new DbContextOptionsBuilder<ProductoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ProductoDbContext(options);
            _service = new ProductoService(_context);
        }

        [Fact]
        public async Task CrearAsync_DeberiaCrearProducto()
        {
            var dto = new ProductoDto
            {
                Nombre = "Nuevo producto",
                Precio = 1000,
                Descripcion = "Prueba"
            };

            var id = await _service.CrearAsync(dto);

            Assert.True(id > 0);
        }

        [Fact]
        public async Task ListarAsync_DeberiaRetornarSoloActivos()
        {
            await _context.Productos.AddRangeAsync(
                new ProductoEntity { Nombre = "A", Precio = 100, EsActivo = true },
                new ProductoEntity { Nombre = "B", Precio = 200, EsActivo = false }
            );
            await _context.SaveChangesAsync();

            var resultado = await _service.ListarAsync(true);

            Assert.Single(resultado);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DeberiaRetornarProducto_SiExiste()
        {
            var producto = new ProductoEntity
            {
                Nombre = "Existente",
                Precio = 500,
                EsActivo = true
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var resultado = await _service.ObtenerPorIdAsync(producto.Id);

            Assert.NotNull(resultado);
            Assert.Equal("Existente", resultado?.Nombre);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_DeberiaRetornarNull_SiNoExiste()
        {
            var resultado = await _service.ObtenerPorIdAsync(999);
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaActualizarProducto()
        {
            var producto = new ProductoEntity
            {
                Nombre = "Viejo",
                Precio = 100,
                EsActivo = true
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var dto = new ProductoDto
            {
                Id = producto.Id,
                Nombre = "Actualizado",
                Precio = 150,
                Descripcion = "Editado"
            };

            var result = await _service.ActualizarAsync(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaFallar_SiNoExiste()
        {
            var dto = new ProductoDto { Id = 999, Nombre = "X", Precio = 100 };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarAsync(dto));
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaFallar_SiProductoInactivo()
        {
            var producto = new ProductoEntity
            {
                Nombre = "Inactivo",
                Precio = 300,
                EsActivo = false
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var dto = new ProductoDto
            {
                Id = producto.Id,
                Nombre = "Intento",
                Precio = 400
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarAsync(dto));
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaCambiarEstado()
        {
            var producto = new ProductoEntity
            {
                Nombre = "Cambiar",
                Precio = 100,
                EsActivo = true
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var result = await _service.ActualizarEstadoAsync(producto.Id, false);

            Assert.True(result);
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaFallar_SiNoExiste()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ActualizarEstadoAsync(999, true));
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaFallar_SiEstadoYaCoincide()
        {
            var producto = new ProductoEntity
            {
                Nombre = "YaActivo",
                Precio = 500,
                EsActivo = true
            };
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ActualizarEstadoAsync(producto.Id, true));
        }
    }
}
