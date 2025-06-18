using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Application.DTOs
{
    public class InventarioDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public bool EsActivo { get; set; }

    }
}

