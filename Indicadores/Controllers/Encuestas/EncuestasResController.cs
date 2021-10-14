using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Controllers
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
        [HttpGet("{id}/{idUsuario}")]
        public async Task<ActionResult<EncuestasRes>> Get(int id, int idUsuario)
        {
            try
            {
                if (idUsuario != 0)
                {
                    var data = _context.EncuestasClass.FromSqlRaw($"sp_GetRespuestas " + id + "," + idUsuario + "").ToList();
                    return Ok(data);
                }
                else
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
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<EncuestasResController>
        [HttpPost("{idpregunta}")]
        public async Task<ActionResult<EncuestasRes>> Post(int idpregunta,[FromBody] List<EncuestasRes> model)
        {
            try
            {   
                foreach (var item in model)
                {
                    EncuestasRelacion encuestasRelacion = new EncuestasRelacion();
                    int idrespuesta = 0;
                    if (item.Respuesta != "")
                    {
                        var modeloExistente = _context.EncuestasRes.FirstOrDefault(m => m.Respuesta == item.Respuesta);
                        if (modeloExistente == null)
                        {
                            _context.EncuestasRes.Add(item);
                            await _context.SaveChangesAsync();
                            idrespuesta = (int)item.Id;
                        }
                        else
                        {
                            idrespuesta = (int)modeloExistente.Id;
                        }
                        var modeloRelacion = _context.EncuestasRelacion.Where(m => m.IdPregunta == idpregunta && m.IdRespuesta == idrespuesta).FirstOrDefault();
                        if (modeloRelacion == null)
                        {
                            encuestasRelacion.IdPregunta = idpregunta;
                            encuestasRelacion.IdRespuesta = idrespuesta;
                            _context.EncuestasRelacion.Add(encuestasRelacion);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
