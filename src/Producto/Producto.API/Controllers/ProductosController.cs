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

        /// <summary>
        /// Lista todos los productos registrados.
        /// </summary>
        /// <remarks>Permite filtrar por estado activo/inactivo.</remarks>
        /// <param name="esActivo">Filtrar por estado (true o false).</param>
        /// <response code="200">Lista obtenida correctamente.</response>
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] bool? esActivo)
        {
            var productos = await _productoService.ListarAsync(esActivo);
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <remarks>Devuelve un producto si existe y coincide con el filtro de estado.</remarks>
        /// <param name="id">ID del producto</param>
        /// <param name="esActivo">Filtrar por estado opcional</param>
        /// <response code="200">Producto encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id, [FromQuery] bool? esActivo)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id, esActivo);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <remarks>El nombre debe ser único, sin importar mayúsculas o minúsculas.</remarks>
        /// <param name="dto">Datos del producto</param>
        /// <response code="200">Producto creado correctamente</response>
        /// <response code="400">Error de validación de negocio</response>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ProductoDto dto)
        {
            var id = await _productoService.CrearAsync(dto);
            return Ok(new { id });
        }

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <remarks>No se puede actualizar un producto inactivo.</remarks>
        /// <param name="id">ID del producto</param>
        /// <param name="dto">Datos actualizados</param>
        /// <response code="200">Producto actualizado</response>
        /// <response code="400">El ID no coincide con el DTO</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProductoDto dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide");

            var actualizado = await _productoService.ActualizarAsync(dto);
            if (!actualizado) return NotFound();

            return Ok();
        }

        /// <summary>
        /// Cambia el estado activo/inactivo de un producto.
        /// </summary>
        /// <remarks>Ideal para eliminación lógica o restauración.</remarks>
        /// <param name="id">ID del producto</param>
        /// <param name="esActivo">Nuevo estado</param>
        /// <response code="204">Estado actualizado</response>
        /// <response code="404">Producto no encontrado</response>
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
