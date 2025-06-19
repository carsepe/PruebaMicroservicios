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
        /// <response code="400">Error de validación o stock insuficiente</response>
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
    }
}
