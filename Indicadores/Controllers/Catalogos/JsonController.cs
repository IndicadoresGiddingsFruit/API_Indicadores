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
                return await _context.ProdZonasRastreoCat.OrderBy(x=>x.DescZona).ToListAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Campos
        [HttpGet("{Cod_Prod}/{Cod_Campo}", Name = "GetData")]
        public ActionResult GetData(string Cod_Prod, short Cod_Campo)
        {
            try
            {
                var item = (dynamic)null;
                if (Cod_Campo != 0)
                {
                    item =(from p in _context.ProdProductoresCat
                            join c in _context.ProdCamposCat on p.Cod_Prod equals c.Cod_Prod

                           join t in _context.CatTiposProd on c.Tipo equals t.Tipo

                            join pr in _context.CatProductos on new { c.Tipo, c.Producto } equals new { pr.Tipo, pr.Producto } into CatPr
                            from pr in CatPr.DefaultIfEmpty()

                            join l in _context.CatLocalidades on new { c.CodLocalidad } equals new { l.CodLocalidad } into Loc
                            from l in Loc.DefaultIfEmpty()                            

                            where c.Cod_Prod == Cod_Prod && c.Cod_Campo==Cod_Campo
                            select new
                            {
                                Cod_Prod = c.Cod_Prod,
                                Productor = p.Nombre,
                                Cod_Campo = c.Cod_Campo,
                                Campo = c.Descripcion,
                                Compras_oportunidad = c.Compras_Oportunidad,
                                Ubicacion = c.Ubicacion,
                                Localidad=l.Descripcion,
                                Tipo = t.Descripcion,
                                Producto = pr.Descripcion
                            }).Distinct();                    
                }
                else
                {
                    item = (from p in _context.ProdProductoresCat
                            join c in _context.ProdCamposCat on p.Cod_Prod equals c.Cod_Prod
                            where c.Cod_Prod == Cod_Prod
                            select new
                            {
                                Cod_Prod = c.Cod_Prod,
                                Productor = p.Nombre,
                                Cod_Campo = c.Cod_Campo
                            }).ToList();
                }
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        
        }

        //Asesores
        [HttpGet("{depto}", Name = "GetAsesor")]
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
                                where a.Depto == depto && a.Activo == true && a.Codigo != null || a.IdAgen== 304
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
