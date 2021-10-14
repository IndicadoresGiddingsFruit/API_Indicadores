using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AgentesController(AppDbContext context)
        {
            this._context = context;
        }

        // GET api/<AgentesController>/5
        [HttpGet("{Cod_Prod}", Name = "GetAsesores")]
        public ActionResult GetAsesores(string Cod_Prod)
        {
            try
            {
                var item = (from c in _context.ProdCamposCat
                            join p in _context.ProdProductoresCat on c.Cod_Prod equals p.Cod_Prod
                            join a in _context.ProdAgenteCat on c.IdAgen equals a.IdAgen
                            where c.Cod_Prod == Cod_Prod
                            select new
                            {
                                Productor = p.Nombre,
                                IdAgen = a.IdAgen,
                                Asesor = a.Nombre
                            }).Distinct().ToList();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
