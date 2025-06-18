using Inventario.Application.DTOs;
using Inventario.Application.Services;
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

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] InventarioDto dto)
        {
            var id = await _inventarioService.CrearInventarioAsync(dto);
            return Ok(id);
        }

        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerPorProducto(int productoId)
        {
            var inventario = await _inventarioService.ObtenerInventarioPorProductoIdAsync(productoId);
            if (inventario == null) return NotFound();
            return Ok(inventario);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var inventarios = await _inventarioService.ListarAsync();
            return Ok(inventarios);
        }
    }
}
