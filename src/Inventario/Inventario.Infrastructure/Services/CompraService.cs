using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Inventario.Domain.Entities;
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
            try
            {
                if (dto.Cantidad <= 0)
                {
                    return new CompraResultadoDto
                    {
                        Exito = false,
                        Mensaje = "La cantidad a comprar debe ser mayor que cero."
                    };
                }

                var producto = await _productoApi.ObtenerProductoPorIdAsync(dto.ProductoId);

                if (producto == null)
                {
                    return new CompraResultadoDto
                    {
                        Exito = false,
                        Mensaje = "Producto no encontrado o no disponible actualmente."
                    };
                }

                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i => i.ProductoId == dto.ProductoId && i.EsActivo);

                if (inventario == null)
                {
                    return new CompraResultadoDto
                    {
                        Exito = false,
                        Mensaje = "El producto aún no tiene inventario asignado."
                    };
                }

                if (inventario.Cantidad < dto.Cantidad)
                {
                    return new CompraResultadoDto
                    {
                        Exito = false,
                        Mensaje = $"Stock insuficiente. Disponible: {inventario.Cantidad}"
                    };
                }

                inventario.Cantidad -= dto.Cantidad;
                inventario.FechaActualizacion = DateTime.UtcNow;

                //Guardar en histórico
                _context.ComprasHistorico.Add(new CompraHistorico
                {
                    ProductoId = dto.ProductoId,
                    Cantidad = dto.Cantidad,
                    FechaCompra = DateTime.UtcNow,
                    Origen = "api"
                });

                await _context.SaveChangesAsync();

                return new CompraResultadoDto
                {
                    Exito = true,
                    Mensaje = "Compra procesada correctamente."
                };
            }
            catch (HttpRequestException)
            {
                return new CompraResultadoDto
                {
                    Exito = false,
                    Mensaje = "No se pudo conectar al servicio de productos. Verifica la red o configuración."
                };
            }
            catch (Exception)
            {
                return new CompraResultadoDto
                {
                    Exito = false,
                    Mensaje = "Ocurrió un error interno al procesar la compra."
                };
            }
        }


        public async Task<List<CompraHistoricoDto>> ListarHistoricoAsync(int? productoId, DateTime? fechaInicio, DateTime? fechaFin, string? origen)
        {
            var query = _context.ComprasHistorico.AsQueryable();

            if (productoId.HasValue)
                query = query.Where(h => h.ProductoId == productoId.Value);

            if (fechaInicio.HasValue)
                query = query.Where(h => h.FechaCompra >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(h => h.FechaCompra <= fechaFin.Value);

            if (!string.IsNullOrWhiteSpace(origen))
                query = query.Where(h => h.Origen == origen);

            return await query
                .OrderByDescending(h => h.FechaCompra)
                .Select(h => new CompraHistoricoDto
                {
                    Id = h.Id,
                    ProductoId = h.ProductoId,
                    Cantidad = h.Cantidad,
                    FechaCompra = h.FechaCompra,
                    Origen = h.Origen
                })
                .ToListAsync();
        }


    }

}
