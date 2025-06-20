﻿using Inventario.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Application.Interfaces
{
    public interface ICompraService
    {
        Task<CompraResultadoDto> ProcesarCompraAsync(CompraDto dto);
        Task<List<CompraHistoricoDto>> ListarHistoricoAsync(int? productoId, DateTime? fechaInicio, DateTime? fechaFin, string? origen);

    }

}
