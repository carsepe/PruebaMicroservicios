using Microsoft.AspNetCore.Mvc;
using Producto.Application.DTOs;
using Producto.Application.Services;

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

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoDto dto)
        {
            var id = await _productoService.CrearProductoAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var productos = await _productoService.ListarAsync();
            return Ok(productos);
        }

    }
}
