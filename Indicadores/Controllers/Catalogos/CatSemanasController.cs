using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatSemanasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CatSemanasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<CatSemanasController>
        [HttpGet]
        public ActionResult Get()
        {
            var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
            var semanas = (from a in _context.CatSemanas
                           where a.Temporada == catSemanas.Temporada
                    select new
                    {
                        Temporada = a.Temporada,
                        Semana = a.Semana,
                        Inicio = a.Inicio
                    }).Distinct().OrderBy(x => x.Inicio).ToList();

            return Ok(semanas);
        }
    }
}
