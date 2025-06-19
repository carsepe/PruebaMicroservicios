using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar compras en el sistema de inventario.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Procesa una compra y descuenta del inventario disponible.
        /// </summary>
        /// <param name="dto">Objeto que contiene el ID del producto y la cantidad a comprar.</param>
        /// <returns>Un resultado en formato JSON:API indicando si la compra fue exitosa.</returns>
        /// <response code="200">Compra procesada correctamente</response>
        [HttpPost]
        public async Task<IActionResult> Comprar([FromBody] CompraDto dto)
        {
            var resultado = await _compraService.ProcesarCompraAsync(dto);

            return Ok(new
            {
                data = new
                {
                    type = "compras",
                    id = Guid.NewGuid().ToString(),
                    attributes = new
                    {
                        exito = resultado.Exito,
                        mensaje = resultado.Mensaje
                    }
                }
            });
        }

        /// <summary>
        /// Lista el historial de compras con filtros opcionales.
        /// </summary>
        [HttpGet("historico")]
        public async Task<IActionResult> ObtenerHistorico([FromQuery] int? productoId, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] string? origen)
        {
            var lista = await _compraService.ListarHistoricoAsync(productoId, fechaInicio, fechaFin, origen);

            return Ok(new
            {
                data = lista.Select(item => new
                {
                    type = "compraHistorico",
                    id = item.Id,
                    attributes = item
                })
            });
        }

    }
}
