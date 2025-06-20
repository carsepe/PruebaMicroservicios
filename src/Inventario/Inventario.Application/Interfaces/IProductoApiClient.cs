﻿using Inventario.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Application.Interfaces
{
    public interface IProductoApiClient
    {
        Task<ProductoDto?> ObtenerProductoPorIdAsync(int productoId);
    }
}
