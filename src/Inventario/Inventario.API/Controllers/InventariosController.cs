using Inventario.Application.DTOs;
using Inventario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.API.Controllers
{
    /// <summary>
    /// Controlador para operaciones sobre el inventario de productos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class InventariosController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventariosController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        /// <summary>
        /// Obtiene el inventario asociado a un producto específico.
        /// </summary>
        /// <param name="productoId">ID del producto.</param>
        /// <param name="esActivo">Filtrar por estado activo/inactivo (opcional).</param>
        /// <returns>Inventario del producto si existe.</returns>
        /// <response code="200">Inventario encontrado</response>
        /// <response code="404">No existe inventario para el producto</response>
        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerPorProducto(int productoId, [FromQuery] bool? esActivo)
        {
            var inventario = await _inventarioService.ObtenerInventarioPorProductoIdAsync(productoId, esActivo);
            if (inventario == null) return NotFound();
            return Ok(inventario);
        }

        /// <summary>
        /// Lista todos los inventarios.
        /// </summary>
        /// <param name="esActivo">Filtrar por estado activo/inactivo (opcional).</param>
        /// <returns>Lista de inventarios.</returns>
        /// <response code="200">Inventarios encontrados</response>
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool? esActivo)
        {
            var inventarios = await _inventarioService.ListarAsync(esActivo);
            return Ok(inventarios);
        }

        /// <summary>
        /// Crea un nuevo registro de inventario.
        /// </summary>
        /// <param name="dto">Datos del inventario a registrar.</param>
        /// <returns>ID del nuevo inventario.</returns>
        /// <response code="200">Inventario creado exitosamente</response>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] InventarioDto dto)
        {
            var id = await _inventarioService.CrearAsync(dto);
            return Ok(new { id });
        }

        /// <summary>
        /// Actualiza un inventario existente.
        /// </summary>
        /// <param name="dto">Datos del inventario a actualizar.</param>
        /// <returns>NoContent si se actualizó correctamente.</returns>
        /// <response code="204">Actualización exitosa</response>
        /// <response code="404">Inventario no encontrado</response>
        [HttpPut]
        public async Task<IActionResult> Actualizar([FromBody] InventarioDto dto)
        {
            var actualizado = await _inventarioService.ActualizarAsync(dto);
            if (!actualizado) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Cambia el estado (activo/inactivo) de un inventario.
        /// </summary>
        /// <param name="id">ID del inventario.</param>
        /// <param name="esActivo">Nuevo estado.</param>
        /// <returns>NoContent si se actualizó correctamente.</returns>
        /// <response code="204">Estado actualizado</response>
        /// <response code="404">Inventario no encontrado</response>
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromQuery] bool esActivo)
        {
            var actualizado = await _inventarioService.ActualizarEstadoAsync(id, esActivo);
            if (!actualizado) return NotFound();
            return NoContent();
        }
    }
}
