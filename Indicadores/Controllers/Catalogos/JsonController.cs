using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiIndicadores.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonController : Controller
    {
        private readonly AppDbContext _context;
        public JsonController(AppDbContext context)
        {
            this._context = context;
        }
                
        //Asesores por departamento
        [HttpGet("{depto}")]
        public ActionResult GetAsesor(string depto)
        {
            try
            {
                var item = (dynamic)null;
                if (depto != "")
                {
                    if (depto == "C")
                    {
                        item = (from a in _context.ProdAgenteCat
                                where a.Depto == depto && a.Activo == true && a.Codigo != null || a.IdAgen == 304 || a.IdAgen == 328
                                select new
                                {
                                    IdAgen = a.IdAgen,
                                    Asesor = a.Nombre,
                                    Tipo = a.Depto
                                }).Distinct().OrderBy(x => x.Asesor).ToList();
                    }
                    else
                    {
                        item = (from a in _context.ProdAgenteCat
                                where a.Depto == depto && a.Activo == true && a.Codigo != null
                                select new
                                {
                                    IdAgen = a.IdAgen,
                                    Asesor = a.Nombre,
                                    Tipo = a.Depto
                                }).Distinct().OrderBy(x => x.Asesor).ToList();
                    }

                }
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
