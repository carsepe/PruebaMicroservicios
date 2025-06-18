using Xunit;
using Moq;
using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Infrastructure;
using Inventario.Infrastructure.Services;
using Inventario.Domain.Entities;
using InventarioEntity = Inventario.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Inventario.Tests.Services
{
    public class CompraServiceTests
    {
        private readonly Mock<IProductoApiClient> _productoApiMock;
        private readonly InventarioDbContext _dbContext;
        private readonly CompraService _service;

        public CompraServiceTests()
        {
            _productoApiMock = new Mock<IProductoApiClient>();

            var options = new DbContextOptionsBuilder<InventarioDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new InventarioDbContext(options);
            _service = new CompraService(_dbContext, _productoApiMock.Object);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaRetornarExito_CuandoTodoEsValido()
        {
            var productoId = 1;

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

            var dto = new CompraDto { ProductoId = productoId, Cantidad = 3 };
            var resultado = await _service.ProcesarCompraAsync(dto);

            Assert.True(resultado.Exito);
            Assert.Equal("Compra procesada correctamente.", resultado.Mensaje);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaFallar_SiProductoNoExiste()
        {
            var dto = new CompraDto { ProductoId = 999, Cantidad = 1 };

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(dto.ProductoId))
                .ReturnsAsync((ProductoDto?)null);

            var resultado = await _service.ProcesarCompraAsync(dto);

            Assert.False(resultado.Exito);
            Assert.Equal("Producto no encontrado.", resultado.Mensaje);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaFallar_SiInventarioNoExiste()
        {
            var productoId = 2;

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = true });

            var dto = new CompraDto { ProductoId = productoId, Cantidad = 1 };
            var resultado = await _service.ProcesarCompraAsync(dto);

            Assert.False(resultado.Exito);
            Assert.Equal("Inventario no encontrado.", resultado.Mensaje);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaFallar_SiInventarioEstaInactivo()
        {
            var productoId = 3;

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = true });

            await _dbContext.Inventarios.AddAsync(new InventarioEntity
            {
                ProductoId = productoId,
                Cantidad = 5,
                EsActivo = false,
                FechaActualizacion = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var dto = new CompraDto { ProductoId = productoId, Cantidad = 1 };
            var resultado = await _service.ProcesarCompraAsync(dto);

            Assert.False(resultado.Exito);
            Assert.Equal("Inventario no encontrado.", resultado.Mensaje);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaFallar_SiStockInsuficiente()
        {
            var productoId = 4;

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto { Id = productoId, EsActivo = true });

            await _dbContext.Inventarios.AddAsync(new InventarioEntity
            {
                ProductoId = productoId,
                Cantidad = 2,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var dto = new CompraDto { ProductoId = productoId, Cantidad = 5 };
            var resultado = await _service.ProcesarCompraAsync(dto);

            Assert.False(resultado.Exito);
            Assert.Equal("Stock insuficiente.", resultado.Mensaje);
        }
    }
}
