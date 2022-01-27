using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Proyeccion
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProyeccionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProyeccionController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: Actualizaciones de la curva
        [HttpGet("{idAgen}")]
        public ActionResult Get(int idAgen)
        {
            try
            {
                if (idAgen == 1 || idAgen == 5 || idAgen == 50)
                {
                    var totales = _context.ProyeccionTotalClass.FromSqlRaw($"sp_GetProyeccion " + idAgen + "").ToList();
                    var meses = _context.ProyeccionMesSemanaClass.FromSqlRaw($"sp_GetProyeccionMeses " + idAgen + "").ToList();
                    var semanas = _context.ProyeccionMesSemanaClass.FromSqlRaw($"sp_GetProyeccionSemanas " + idAgen + "").ToList();

                    var res = Tuple.Create(totales.ToList(), meses.ToList(), semanas.ToList());
                    return Ok(res);
                }
                
                else
                {
                    var totales = _context.ProyeccionClass.FromSqlRaw($"sp_GetProyeccion " + idAgen + "").ToList();
                    var meses = _context.ProyeccionMesClass.FromSqlRaw($"sp_GetProyeccionMeses " + idAgen + "").ToList();
                    var res = Tuple.Create(totales.ToList(), meses.ToList());
                    return Ok(res);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
