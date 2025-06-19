using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventariosController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventariosController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerPorProducto(int productoId, [FromQuery] bool? esActivo)
        {
            var inventario = await _inventarioService.ObtenerInventarioPorProductoIdAsync(productoId, esActivo);
            if (inventario == null) return NotFound();
            return Ok(inventario);
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool? esActivo)
        {
            var inventarios = await _inventarioService.ListarAsync(esActivo);
            return Ok(inventarios);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] InventarioDto dto)
        {
            var id = await _inventarioService.CrearAsync(dto);
            return Ok(new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] InventarioDto dto)
        {
            var actualizado = await _inventarioService.ActualizarAsync(dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromQuery] bool esActivo)
        {
            var actualizado = await _inventarioService.ActualizarEstadoAsync(id, esActivo);
            if (!actualizado) return NotFound();
            return NoContent();
        }
    }
}
