using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestasTipoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EncuestasTipoController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EncuestasTipoController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncuestasTipo>>> Get()
        {
            var item = (from t in _context.EncuestasTipo
                        group t by new
                        {
                            IdTipo = t.Id,
                            Descripcion = t.Descripcion
                        } into x
                        select new
                        {
                            IdTipo = x.Key.IdTipo,
                            Descripcion = x.Key.Descripcion
                        }).Distinct();

            if (item == null)
            {
                return NotFound();
            }

            return Ok(await item.ToListAsync());
        }

        // GET api/<EncuestasTipoController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return Ok(_context.EncuestasTipo.Find(id));
        }

        // POST api/<EncuestasTipoController>
        [HttpPost]
        public async Task<ActionResult<EncuestasTipo>> Post([FromBody] EncuestasTipo model)
        {
            try
            {
                var item = _context.EncuestasTipo.FirstOrDefault(m => m.Descripcion == model.Descripcion);
                if (item == null)
                {
                    _context.EncuestasTipo.Add(model);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("La descripción ya fue agregada anteriormente");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
 
    }
}
