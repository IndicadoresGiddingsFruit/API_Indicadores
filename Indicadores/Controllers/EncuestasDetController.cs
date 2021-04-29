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
    public class EncuestasDetController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EncuestasDetController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EncuestasDetController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncuestasDet>>> Get()
        {
            return await _context.EncuestasDet.ToListAsync();
        }

        // GET api/<EncuestasDetController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            return Ok(_context.EncuestasDet.Find(id));
        }

        // POST api/<EncuestasDetController>
        [HttpPost("{id}")]
        public async Task<ActionResult<EncuestasDet>> Post(int id, [FromBody] EncuestasDet model)
        {
            try
            {
                var modeloExistente = _context.EncuestasCat.FirstOrDefault(m => m.Id == id);
                if (modeloExistente != null)
                {
                    modeloExistente.Fecha_modificacion = DateTime.Now;
                    await _context.SaveChangesAsync();

                    var item = _context.EncuestasDet.FirstOrDefault(m => m.Pregunta == model.Pregunta && m.IdEncuesta == id);
                    if (item == null)
                    {
                        _context.EncuestasDet.Add(model);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("La pregunta ya existe");
                    }
                }
                else
                {
                    return BadRequest("La encuesta no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<EncuestasDetController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EncuestasDet>> Put(int id, [FromBody] EncuestasDet model)
        {
            try
            {
                var item = _context.EncuestasDet.Find(id);
                if (item != null)
                {
                    item.Pregunta = model.Pregunta;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("La pregunta no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<EncuestasDetController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var model = _context.EncuestasDet.Find(id);
                if (model != null)
                {
                    _context.EncuestasDet.Remove(model);
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
