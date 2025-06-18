using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producto.Application.DTOs
{
    public class ProductoDto
    {
        public string Nombre { get; set; } = default!;
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
    }
}

