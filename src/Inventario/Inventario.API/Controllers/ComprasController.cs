using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

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
