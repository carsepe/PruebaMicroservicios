using Xunit;
using Moq;
using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Infrastructure.Services;
using Inventario.Infrastructure;
using InventarioEntity = Inventario.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Inventario.Tests.Services
{
    public class InventarioServiceTests
    {
        private readonly Mock<IProductoApiClient> _productoApiMock;
        private readonly InventarioDbContext _dbContext;
        private readonly InventarioService _service;

        public InventarioServiceTests()
        {
            _productoApiMock = new Mock<IProductoApiClient>();
            var options = new DbContextOptionsBuilder<InventarioDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new InventarioDbContext(options);
            _service = new InventarioService(_dbContext, _productoApiMock.Object);
        }

        [Fact]
        public async Task CrearAsync_DeberiaCrearInventario_CuandoProductoExisteYActivo()
        {
            var productoId = 1;
            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = true });

            var dto = new InventarioDto { ProductoId = productoId, Cantidad = 5 };

            var id = await _service.CrearAsync(dto);

            Assert.True(id > 0);
        }

        [Fact]
        public async Task CrearAsync_DeberiaFallar_SiProductoNoExiste()
        {
            var dto = new InventarioDto { ProductoId = 999, Cantidad = 5 };

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(dto.ProductoId))
                .ReturnsAsync((ProductoDto?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CrearAsync(dto));
        }

        [Fact]
        public async Task CrearAsync_DeberiaFallar_SiProductoEstaInactivo()
        {
            var productoId = 2;
            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = false });

            var dto = new InventarioDto { ProductoId = productoId, Cantidad = 5 };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CrearAsync(dto));
        }

        [Fact]
        public async Task CrearAsync_DeberiaFallar_SiInventarioYaExiste()
        {
            var productoId = 3;
            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = true });

            await _dbContext.Inventarios.AddAsync(new InventarioEntity
            {
                ProductoId = productoId,
                Cantidad = 10,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var dto = new InventarioDto { ProductoId = productoId, Cantidad = 5 };

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CrearAsync(dto));
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaActualizarEstado()
        {
            var inventario = new InventarioEntity
            {
                ProductoId = 10,
                Cantidad = 10,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            };
            _dbContext.Inventarios.Add(inventario);
            await _dbContext.SaveChangesAsync();

            var result = await _service.ActualizarEstadoAsync(inventario.Id, false);
            Assert.True(result);
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaFallar_SiInventarioNoExiste()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarEstadoAsync(999, true));
        }

        [Fact]
        public async Task ActualizarEstadoAsync_DeberiaFallar_SiEstadoYaCoincide()
        {
            var inventario = new InventarioEntity
            {
                ProductoId = 11,
                Cantidad = 5,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            };
            _dbContext.Inventarios.Add(inventario);
            await _dbContext.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarEstadoAsync(inventario.Id, true));
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaActualizarCantidad()
        {
            var inventario = new InventarioEntity
            {
                ProductoId = 20,
                Cantidad = 5,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            };
            _dbContext.Inventarios.Add(inventario);
            await _dbContext.SaveChangesAsync();

            var dto = new InventarioDto { ProductoId = 20, Cantidad = 15 };
            var result = await _service.ActualizarAsync(dto);

            Assert.True(result);
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaFallar_SiInventarioNoExiste()
        {
            var dto = new InventarioDto { ProductoId = 999, Cantidad = 10 };
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarAsync(dto));
        }

        [Fact]
        public async Task ActualizarAsync_DeberiaFallar_SiInventarioEstaInactivo()
        {
            var inventario = new InventarioEntity
            {
                ProductoId = 30,
                Cantidad = 5,
                EsActivo = false,
                FechaActualizacion = DateTime.UtcNow
            };
            _dbContext.Inventarios.Add(inventario);
            await _dbContext.SaveChangesAsync();

            var dto = new InventarioDto { ProductoId = 30, Cantidad = 15 };
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ActualizarAsync(dto));
        }

        [Fact]
        public async Task ListarAsync_DeberiaRetornarSoloActivos()
        {
            _dbContext.Inventarios.AddRange(
                new InventarioEntity { ProductoId = 1, Cantidad = 5, EsActivo = true, FechaActualizacion = DateTime.UtcNow },
                new InventarioEntity { ProductoId = 2, Cantidad = 3, EsActivo = false, FechaActualizacion = DateTime.UtcNow }
            );
            await _dbContext.SaveChangesAsync();

            var resultado = await _service.ListarAsync(true);
            Assert.Single(resultado);
        }

        [Fact]
        public async Task ObtenerInventarioPorProductoIdAsync_DeberiaRetornarNull_SiNoExiste()
        {
            var resultado = await _service.ObtenerInventarioPorProductoIdAsync(999, true);
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObtenerInventarioPorProductoIdAsync_DeberiaRetornarInventario_SiExiste()
        {
            _dbContext.Inventarios.Add(new InventarioEntity
            {
                ProductoId = 77,
                Cantidad = 9,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var resultado = await _service.ObtenerInventarioPorProductoIdAsync(77, true);
            Assert.NotNull(resultado);
            Assert.Equal(9, resultado.Cantidad);
        }
    }
}
