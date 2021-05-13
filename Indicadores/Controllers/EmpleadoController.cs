using Indicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmpleadoController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<EmpleadoController>
        [HttpGet(Name = "GetAllEmpleados")]
        public ActionResult Get()
        {
            try
            {
                var empleados = (from e in _context.EncuestasCat
                                 join t in _context.EncuestasTipo on e.IdTipo equals t.Id
                                 group e by new
                                 {
                                     IdEncuesta = e.Id,
                                     Nombre = e.Nombre,
                                     Descripcion = e.Descripcion,
                                     Fecha = e.Fecha,
                                     Fecha_modificacion = e.Fecha_modificacion,
                                     Estatus = e.Estatus,
                                     IdTipo = e.IdTipo,
                                     Tipo = t.Descripcion
                                 } into x
                                 select new
                                 {
                                     IdEncuesta = x.Key.IdEncuesta,
                                     Nombre = x.Key.Nombre,
                                     Descripcion = x.Key.Descripcion,
                                     Fecha = x.Key.Fecha,
                                     Fecha_modificacion = x.Key.Fecha_modificacion,
                                     Estatus = x.Key.Estatus,
                                     IdTipo = x.Key.IdTipo,
                                     Tipo = x.Key.Tipo
                                 }).Distinct();

                return Ok(empleados.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // GET api/<EmpleadoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmpleadoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmpleadoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmpleadoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
