using Microsoft.AspNetCore.Mvc;
using Producto.Application.DTOs;
using Producto.Application.Interfaces;

namespace Producto.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool? esActivo)
        {
            var productos = await _productoService.ListarAsync(esActivo);
            return Ok(productos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id, [FromQuery] bool? esActivo)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id, esActivo);
            if (producto == null) return NotFound();
            return Ok(producto);
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoDto dto)
        {
            var id = await _productoService.CrearAsync(dto);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoDto dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide");

            var actualizado = await _productoService.ActualizarAsync(dto);
            if (!actualizado) return NotFound();

            return Ok();
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromQuery] bool esActivo)
        {
            var actualizado = await _productoService.ActualizarEstadoAsync(id, esActivo);
            if (!actualizado)
                return NotFound();

            return NoContent();
        }

    }
}
