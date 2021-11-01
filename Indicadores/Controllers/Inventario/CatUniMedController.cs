using ApiIndicadores.Context;
using ApiIndicadores.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatUniMedController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CatUniMedController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<ArticulosController>
        [HttpGet]
        public ActionResult<CatUniMed> Get()
        {
            try
            {
                var item = _context.CatUniMed.Distinct();
                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
