using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Expediente
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpedienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExpedienteController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: Visitas/Proyeccion/Rendimiento/Financiamiento por asesor
        [HttpGet("{idAgen}")]
        public ActionResult Get(short idAgen)
        {
            try
            {
                var visitas = _context.VisitasExpedienteClass.FromSqlRaw($"sp_GetExpediente 1, " + idAgen + "").ToList();
                var proyeccion = _context.ProyeccionExpedienteClass.FromSqlRaw($"sp_GetExpediente 2, " + idAgen + "").ToList();
                var rendimiento = _context.RendimientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 3, " + idAgen + "").ToList();
                var financiamiento = _context.FinanciamientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 4, " + idAgen + "").ToList();

                var res = Tuple.Create(visitas.ToList(), proyeccion.ToList(), rendimiento.ToList(), financiamiento.ToList());
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
