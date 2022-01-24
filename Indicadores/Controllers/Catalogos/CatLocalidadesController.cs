using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatLocalidadesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CatLocalidadesController(AppDbContext context)
        {
            this._context = context;
        }

        //GET: Localidaddes de  "GUA", "JAL", "MIC", "SIN" 
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                List<string> estados = new List<string>() { "GUA", "JAL", "MIC", "SIN" };
                var item = (from l in _context.CatLocalidades
                            join m in _context.MunicipioSAT on new {l.CodMunicipio, l.CodEstado} equals new { m.CodMunicipio , m.CodEstado }
                            join e in _context.EstadoSAT on l.CodEstado equals e.CodEstado
                            where estados.Contains(e.CodEstado)
                            group l by new
                            {
                               CodLocalidad=l.CodLocalidad,
                               Localidad=l.Descripcion,
                               CodMunicipio=l.CodMunicipio,
                               Municipio=m.Descripcion,
                               CodEstado=l.CodEstado,
                               Estado=e.Descripcion,
                               Completo = l.Descripcion + " " + m.Descripcion + " " + e.Descripcion
                            } into x
                            select new
                            {
                                CodLocalidad = x.Key.CodLocalidad,
                                Localidad = x.Key.Localidad,
                                CodMunicipio = x.Key.CodMunicipio,
                                Municipio = x.Key.Municipio,
                                CodEstado = x.Key.CodEstado,
                                Estado = x.Key.Estado,
                                Completo = x.Key.Completo
                            }).Distinct();
                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
