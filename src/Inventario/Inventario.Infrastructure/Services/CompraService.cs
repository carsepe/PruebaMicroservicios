using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Infrastructure.Services
{
    public class CompraService : ICompraService
    {
        private readonly InventarioDbContext _context;
        private readonly IProductoApiClient _productoApi;

        public CompraService(InventarioDbContext context, IProductoApiClient productoApi)
        {
            _context = context;
            _productoApi = productoApi;
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
