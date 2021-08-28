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
                           //join s in _context.ProdMuestreoSector on new { c.Cod_Prod, c.Cod_Campo } equals new { s.Cod_Prod, s.Cod_Campo } into CatPr
                           join t in _context.CatTiposProd on c.Tipo equals t.Tipo
                            join pr in _context.CatProductos on new { c.Tipo, c.Producto } equals new { pr.Tipo, pr.Producto } into CatPr
                            from pr in CatPr.DefaultIfEmpty()
                            join l in _context.CatLocalidades on c.CodLocalidad equals l.CodLocalidad
                            where c.Cod_Prod == Cod_Prod && c.Cod_Campo==Cod_Campo
                            select new
                            {
                                Cod_Prod = c.Cod_Prod,
                                Productor = p.Nombre,
                                Cod_Campo = c.Cod_Campo,
                                Campo = c.Descripcion,
                                Compras_oportunidad = c.Compras_Oportunidad,
                                Ubicacion = l.Descripcion,
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

            //var nom_prod = _context.ProdProductoresCat.Where(x => x.Cod_Prod == Cod_Prod).FirstOrDefault();
            //List<ProdCamposCat> ListCampos = _context.ProdCamposCat.Where(x => x.Cod_Prod == Cod_Prod).ToList();
            //return Tuple.Create(nom_prod, ListCampos);           
        }

        //Asesores
        [HttpGet("{tipo}", Name = "GetAsesor")]
        public ActionResult GetAsesor(string tipo)
        {
            try
            {
                var item = (dynamic)null;
                if (tipo != "")
                {
                    item = (from a in _context.ProdAgenteCat
                            where a.Depto==tipo && a.Activo==true && a.Codigo!=null
                            select new
                            {
                                IdAgen = a.IdAgen,
                                Asesor=a.Nombre,
                                Tipo=a.Depto
                            }).Distinct().OrderBy(x=>x.Asesor).ToList();
                }                
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            //var nom_prod = _context.ProdProductoresCat.Where(x => x.Cod_Prod == Cod_Prod).FirstOrDefault();
            //List<ProdCamposCat> ListCampos = _context.ProdCamposCat.Where(x => x.Cod_Prod == Cod_Prod).ToList();
            //return Tuple.Create(nom_prod, ListCampos);           
        }
        
        //public JsonResult DescCampo(string Cod_Prod, short Cod_Campo)
        //{
        //    var descipcion = _context.ProdCamposCat.Where(x => x.Cod_Prod == Cod_Prod && x.Cod_Campo == Cod_Campo).FirstOrDefault();
        //    return Json(descipcion);
        //}      

        ////Ubicacion del campo
        //public JsonResult GetUbicacion_campo(string Cod_Prod, short Cod_Campo)
        //{
        //    var ubicacion = _context.ProdCamposCat.Where(x => x.Cod_Prod == Cod_Prod && x.Cod_Campo == Cod_Campo).FirstOrDefault();
        //    return Json(ubicacion);
        //}

        //public JsonResult GetLocalidad_campo(string Cod_Prod, int Cod_Campo)
        //{
        //    var localidad = _context.MuestreosClass.FromSqlRaw($"SELECT L.Descripcion From ProdCamposCat C left join CatLocalidades L on C.CodLocalidad=L.CodLocalidad where C.Cod_Prod='" + Cod_Prod + "' and C.Cod_Campo=" + Cod_Campo + "").First();
        //    return Json(localidad);
        //}

        //public JsonResult GetTipoProducto(string Cod_Prod, int Cod_Campo)
        //{
        //    var Tipo_Producto = _context.MuestreosClass.FromSqlRaw($"select T.Descripcion as Tipo, P.Descripcion AS Producto from ProdCamposCat C Left join CatTiposProd T ON C.Tipo=T.Tipo left join CatProductos P on C.Tipo=P.Tipo and C.Producto=P.Producto where C.cod_prod='" + Cod_Prod + "' and C.Cod_Campo=" + Cod_Campo + "").First();
        //    return Json(Tipo_Producto);
        //}

        ////Lista de variedades
        //public JsonResult GetVariedadesList(string Cultivo)
        //{
        //    int tipo = 0;
        //    if (Cultivo == "ZARZAMORA") { tipo = 1; }
        //    if (Cultivo == "FRAMBUESA") { tipo = 2; }
        //    if (Cultivo == "ARANDANO") { tipo = 3; }
        //    if (Cultivo == "FRESA") { tipo = 4; }

        //   var ListCampos = _context.CatProductos.Where(x => x.Tipo == tipo).ToList();
        //    return Json(ListCampos);
        //}

        //public JsonResult IncidenciasList(int Id_Muestreo)
        //{
        //    var resultados =_context.MuestreosClass.FromSqlRaw($"SELECT isnull(Incidencia,'') as Incidencia, isnull(Propuesta,'') as Propuesta from ProdCalidadMuestreo " +
        //        "where Id_Muestreo=" + Id_Muestreo + " order by Fecha desc").ToList();
        //    return Json(resultados);
        //}
    }
}
