using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public ComprasController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Comprar([FromBody] CompraDto dto)
        {
            try
            {
                var resultado = await _inventarioService.ProcesarCompraAsync(dto);

                if (!resultado.Exito)
                    return BadRequest(new { error = resultado.Mensaje });

                return Ok(new
                {
                    data = new
                    {
                        type = "compra",
                        attributes = new
                        {
                            exito = resultado.Exito,
                            mensaje = resultado.Mensaje
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Ocurrió un error inesperado.",
                    detalle = ex.Message
                });
            }
        }
    }
}
