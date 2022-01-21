using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Muestreos
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EvaluacionController(AppDbContext context)
        {
            this._context = context;
        }      

        // GET api/<EvaluacionController>/5
        [HttpGet("{idAgen}")]
        public ActionResult Get(short idAgen)
        {
            try
            {
                var item = _context.EvaluacionClass.FromSqlRaw($"sp_GetEvaluacion " + idAgen + "").ToList();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
