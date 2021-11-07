using CsvHelper.Configuration.Attributes;
using ApiIndicadores.Context;
using ApiIndicadores.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiIndicadores.Models.Catalogos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<RegionesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tbZonasAgricolas>>> GetZonas()
        {
            try
            {
                return await _context.tbZonasAgricolas.OrderBy(x => x.Descripcion).ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
