using ApiIndicadores.Context;
using ApiIndicadores.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticulosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ArticulosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<ArticulosController>
        [HttpGet]
        public ActionResult<CatArticulos> Get()
        {
            try
            {
                var item = _context.CatArticulos.FromSqlRaw("Select * from SeasonVen..CatArticulos").Distinct();
                 
                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
