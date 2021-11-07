using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Inventario
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosInventarioController : ControllerBase
    { 
        private readonly AppDbContext _context;
        public MovimientosInventarioController(AppDbContext context)
        {
            this._context = context;
        }

        // GET api/<MuestreoController>/5
        [HttpGet("{fechaInicio}/{fechaFinal}")]
        //id es IdAgen o id de usuario        
        public async Task<ActionResult<MovimientosInventarioClass>> Get(DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                var fecha_Inicio =fechaInicio.ToString("yyyy-MM-dd");
                var fecha_Final = fechaFinal.ToString("yyyy-MM-dd");

                var tem_actual = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                var Fini_temp = (from x in _context.CatSemanas
                               where x.Temporada == tem_actual.Temporada
                               select new { Inicio = x.Inicio }).OrderBy(x=>x.Inicio).FirstOrDefault();

                var inicio_Temp = Fini_temp.Inicio.ToString("yyyy-MM-dd");

                var data = _context.MovimientosInventarioClass.FromSqlRaw($"sp_GetMovimientosInventario '" + inicio_Temp + "', '" + fecha_Inicio + "', '" + fecha_Final+"'").ToListAsync();
                
                return Ok(await data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        // GET api/<MovimientosInventarioController>/5

       

        // PUT api/<MovimientosInventarioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MovimientosInventarioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
