using Xunit;
using Moq;
using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Infrastructure.Services;
using Inventario.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using InventarioEntity = Inventario.Domain.Entities.Inventario;

namespace Inventario.IntegrationTests.Services
{
    public class CompraIntegrationTests
    {
        private readonly Mock<IProductoApiClient> _productoApiMock;
        private readonly InventarioDbContext _dbContext;
        private readonly CompraService _service;

        public CompraIntegrationTests()
        {
            _productoApiMock = new Mock<IProductoApiClient>();

            var options = new DbContextOptionsBuilder<InventarioDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new InventarioDbContext(options);
            _service = new CompraService(_dbContext, _productoApiMock.Object);
        }

        [Fact]
        public async Task ProcesarCompra_DeberiaRetornarExito_SiProductoYStockExisten()
        {
            // Arrange
            var productoId = 1;

            _productoApiMock.Setup(p => p.ObtenerProductoPorIdAsync(productoId))
                .ReturnsAsync(new ProductoDto
                {
                    Id = productoId,
                    EsActivo = true
                });

            await _dbContext.Inventarios.AddAsync(new InventarioEntity
            {
                ProductoId = productoId,
                Cantidad = 10,
                EsActivo = true,
                FechaActualizacion = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            var dto = new CompraDto
            {
                ProductoId = productoId,
                Cantidad = 2
            };

            // Act
            var resultado = await _service.ProcesarCompraAsync(dto);

            // Assert
            Assert.True(resultado.Exito);
            Assert.Equal("Compra procesada correctamente.", resultado.Mensaje);
        }
    }
}
