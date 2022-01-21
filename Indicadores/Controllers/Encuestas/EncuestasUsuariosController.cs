using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestasUsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EncuestasUsuariosController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EncuestasUsuariosController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EncuestasUsuarios>>> Get()
        {
            try
            {
                return await _context.EncuestasUsuarios.ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{idEncuesta}")]
        public async Task<ActionResult<EncuestasUsuarios>> Get(int idEncuesta)
        {
            try
            {
                var item = (from u in _context.EncuestasUsuarios
                            join c in _context.SIPGUsuarios on u.IdUsuario equals c.Id
                            where u.IdEncuesta == idEncuesta
                            group u by new
                            {
                                IdEncuesta = u.IdEncuesta,
                                IdUsuario = u.IdUsuario,
                                Usuario = c.Completo
                            } into x
                            select new
                            {
                                IdEncuesta = x.Key.IdEncuesta,
                                IdUsuario = x.Key.IdUsuario,
                                Usuario = x.Key.Usuario
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

        // POST Agregar Respuestas por Usuario
        [HttpPost("{idEncuesta}/{idUsuario}")]
        public async Task<IActionResult> Post(int idEncuesta, int idUsuario, [FromBody] List<EncuestasLog> model)
        {
            try
            {                
                foreach (var item in model)
                {
                    var EncuestasUsuarios = _context.EncuestasUsuarios.FirstOrDefault(m => m.IdEncuesta == idEncuesta && m.IdUsuario == idUsuario);

                    EncuestasUsuarios.Fecha_respuesta = DateTime.Now;
                    await _context.SaveChangesAsync();
                     
                    EncuestasLog EncuestasLog = new EncuestasLog();
                    EncuestasLog.IdAsingUsuario = EncuestasUsuarios.Id;
                    EncuestasLog.IdRelacion = item.IdRelacion;
                    EncuestasLog.RespuestaLibre = item.RespuestaLibre;
                    _context.EncuestasLog.Add(EncuestasLog);
                    await _context.SaveChangesAsync();
                }

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<EncuestasUsuariosController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<EncuestasUsuarios>> Put(int id, [FromBody] EncuestasUsuarios model)
        {
            try
            {
                //var model = _context.EncuestasUsuarios.FirstOrDefault(m => m.IdEncuesta == id);
                if (model != null)
                {
                    await _context.SaveChangesAsync();
                    return Ok(model);
                }
                else
                {
                    return BadRequest("Algo salió mal");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{id}/{identificador}")]
        public async Task<ActionResult<EncuestasUsuarios>> Patch(int id, string identificador)
        {
            try
            {
                var model = _context.EncuestasUsuarios.Find(id);
                if (model != null)
                {
                    //model.Identificador = identificador;
                    await _context.SaveChangesAsync();
                    return Ok();
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

        // DELETE eliminar usuario de encuesta
        [HttpDelete("{id}/{IdUsuario}")]
        public ActionResult Delete(int id, int IdUsuario)
        {
            try
            {
                var encuesta_usuario = _context.EncuestasUsuarios.FirstOrDefault(m => m.IdEncuesta == id && m.IdUsuario == IdUsuario);
                if (encuesta_usuario != null)
                {
                    _context.EncuestasUsuarios.Remove(encuesta_usuario);
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
