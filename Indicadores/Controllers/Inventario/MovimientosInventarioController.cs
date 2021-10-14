using ApiIndicadores.Classes;
using ApiIndicadores.Context;
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
        [HttpGet]
        //id es IdAgen o id de usuario        
        public async Task<ActionResult<MovimientosInventarioClass>> Get()
        {
            try
            {
                var data = _context.MovimientosInventarioClass.FromSqlRaw($"sp_GetMovimientosInventario").ToListAsync();
                return Ok(await data);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }
        // GET api/<MovimientosInventarioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MovimientosInventarioController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

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
