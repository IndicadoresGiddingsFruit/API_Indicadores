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
    public class EntradasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EntradasController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EntradasController>
        [HttpGet]
        public ActionResult<EntradasAlm> Get()
        {
            try
            {
                var listEntradas = (from x in _context.EntradasAlm
                                    join a in _context.CatArticulos on x.Cod_Artic equals a.Cod_Artic
                                    select new
                                    {
                                        IdEntrada=x.Id,
                                        Cod_Artic = x.Cod_Artic,
                                        Descripcion = a.Descripcion,
                                        Fecha = x.Fecha,
                                        Cantidad = x.Cantidad
                                    }).OrderByDescending(x => x.Fecha).ToList();

                return Ok(listEntradas);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<EntradasController>
        [HttpPost]
        public async Task<ActionResult<EntradasAlm>> Post([FromBody] EntradasAlm model)
        {
            try
            {
                //var articuloExistente = _context.EntradasAlm.FirstOrDefault(m => m.Cod_Artic == model.Cod_Artic);
                //if (articuloExistente == null)
                //{
                model.Fecha = DateTime.Now;
                _context.EntradasAlm.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
                //}
                //else
                //{
                //    return BadRequest("La encuesta ya existe");
                //}
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<EntradasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        ////Delete  
        [HttpDelete("{id}")]
        public async Task<ActionResult<EntradasAlm>> Delete(int id)
        {
            try
            {
                var model = _context.EntradasAlm.Find(id);
                if (model != null)
                {
                    _context.EntradasAlm.Remove(model);
                    await _context.SaveChangesAsync();
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
