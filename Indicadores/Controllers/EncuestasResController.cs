using Indicadores.Context;
using Indicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Indicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestasResController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EncuestasResController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EncuestasResController>
        [HttpGet(Name = "GetAllEncuestasRes")]
        public async Task<ActionResult<IEnumerable<EncuestasRes>>> Get()
        {
            try
            {
                return await _context.EncuestasRes.ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<EncuestasResController>/5
        [HttpGet("{id}", Name = "GetIdEncuestaRes")]
        public async Task<ActionResult<EncuestasRes>> Get(int id)
        {
            try
            {
                var item = (from e in _context.EncuestasRelacion
                            join r in _context.EncuestasRes on e.IdRespuesta equals r.Id

                            where e.IdPregunta == id
                            group e by new
                            {
                                IdPregunta = e.IdPregunta,
                                IdRespuesta = r.Id,
                                Respuesta = r.Respuesta
                            } into x
                            select new
                            {
                                IdPregunta = x.Key.IdPregunta,
                                IdRespuesta = x.Key.IdRespuesta,
                                Respuesta = x.Key.Respuesta
                            }).Distinct();

                if (item == null)
                {
                    return NotFound();
                }
                return Ok(await item.ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<EncuestasResController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EncuestasResController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EncuestasResController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var res = _context.EncuestasRes.FirstOrDefault(m => m.Id == id);
                if (res != null)
                {
                    _context.EncuestasRes.Remove(res);
                    _context.SaveChanges();
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
