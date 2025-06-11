using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientesApi.Data;
using ClientesApi.Models;
using ClientesApi.Models.DTOs;

namespace ClientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesDbContext _context;

        public ClientesController(ClientesDbContext context)
        {
            _context = context;
        }

        // Endpoint usando Store Procedure
        [HttpGet("sp")]
        public async Task<ActionResult<PaginatedResponse<Cliente>>> GetClientesSP(int pageNumber = 1, int pageSize = 10)
        {
            var parameters = new[]
            {
                new SqlParameter("@PageNumber", pageNumber),
                new SqlParameter("@PageSize", pageSize)
            };

            var clientes = await _context.Clientes
                .FromSqlRaw("EXEC GetClientesPaginados @PageNumber, @PageSize", parameters)
                .Include(c => c.pais)
                .ToListAsync();

            var totalRecords = await _context.Clientes.CountAsync();

            return new PaginatedResponse<Cliente>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = clientes
            };
        }

        // Endpoint usando LINQ
        [HttpGet("linq")]
        public async Task<ActionResult<PaginatedResponse<Cliente>>> GetClientesLinq(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Clientes.Include(c => c.pais);
            
            var totalRecords = await query.CountAsync();
            var clientes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<Cliente>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = clientes
            };
        }
    }
}