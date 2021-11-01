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
    public class CatMovtosAlmController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CatMovtosAlmController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<CatMovtosAlmController>
        [HttpGet("{tipo}")]
        public ActionResult<CatMovtosAlm> Get(string tipo)
        {
            try
            {
                if (tipo == "E") {
                    var item = _context.CatMovtosAlm.Where(x => x.Tipo == tipo).OrderBy(x => x.Descripcion).Distinct();
                    return Ok(item.ToList());
                }
                else
                {
                    var item = _context.CatMovtosAlm.Where(x => x.Tipo == "S" && x.Tipo=="T").OrderBy(x => x.Descripcion).Distinct();
                    return Ok(item.ToList());
                }               
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
