﻿using Indicadores.Context;
using Indicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Indicadores.Controllers
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


        [HttpGet("{id}", Name = "GetIdEncuestasUsuarios")]
        public async Task<ActionResult<EncuestasUsuarios>> Get(int id)
        {
            try
            {
                var item = (from u in _context.EncuestasUsuarios
                            join c in _context.SIPGUsuarios on u.IdUsuario equals c.Id
                            where u.IdEncuesta == id
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

        // POST api/<EncuestasUsuariosController>
        [HttpPost("{id}/{idUsuario}")]
        public async Task<IActionResult> Post(int id, int idUsuario, [FromBody] List<EncuestasRelacion> model)
        {
            try
            {
                var EncuestasUsuarios = _context.EncuestasUsuarios.FirstOrDefault(m => m.IdEncuesta == id && m.IdUsuario == idUsuario);
                if (EncuestasUsuarios != null)
                {
                    EncuestasUsuarios.Fecha = DateTime.Now;
                    await _context.SaveChangesAsync();                    

                    foreach (var item in model)
                    {
                        EncuestasLog EncuestasLog = new EncuestasLog();
                        EncuestasLog.IdAsingUsuario = EncuestasUsuarios.Id;
                        EncuestasLog.IdRelacion = item.Id;
                        _context.EncuestasLog.Add(EncuestasLog);
                        await _context.SaveChangesAsync();
                    }
                    return Ok(); // Created5AtRoute("GetAllEncuestas", model);
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
                    return CreatedAtRoute("GetIdEncuesta", new { id = id, IdUsuario = 0 }, model);
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

        // DELETE api/<EncuestasUsuariosController>/5
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