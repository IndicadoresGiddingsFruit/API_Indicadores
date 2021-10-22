using CsvHelper.Configuration.Attributes;
using ApiIndicadores.Context;
using ApiIndicadores.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        //zonas
        public async Task<ActionResult<IEnumerable<ProdZonasRastreoCat>>> GetZonas()
        {
            try
            {
                return await _context.ProdZonasRastreoCat.OrderBy(x => x.DescZona).ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        } 

        //Asesores
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
                                where a.Depto == depto && a.Activo == true && a.Codigo != null || a.IdAgen == 304
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
