﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Application.DTOs
{
    public class CompraResultadoDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
