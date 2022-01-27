using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiIndicadores.Controllers.Recepcion
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecepcionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RecepcionController(AppDbContext context)
        {
            this._context = context;
        }

        //Get semana actual
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var semana_actual = _context.CatSemanas.Where(x=>DateTime.Now >=x.Inicio && DateTime.Now <= x.Fin).Distinct();
                return Ok(semana_actual);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //COMPARATIVA DE RENDIMIENTO PROYECTADO VS ENTREGADO temporada actual
        [HttpGet("{idAgen}")]
        public ActionResult Get(int idAgen)
        {
            try
            {
                var data = _context.RecepcionClass.FromSqlRaw($"sp_GetRecepcion " + idAgen +" ").ToList();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        } 
    }
}
