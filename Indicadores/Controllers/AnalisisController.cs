using Indicadores.Classes;
using Indicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using Indicadores.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Indicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisisController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AnalisisController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";

        // GET: api/<AnalisisController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdAnalisis_Residuo.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<AnalisisController>/5
        [HttpGet("{id}/{idAgen}")]
        public ActionResult Get(int id, short idAgen)
        {
            try
            {
                IQueryable<AnalisisClass> item = null;
                if (idAgen==1 || idAgen == 205 || idAgen == 153 || idAgen == 281 || idAgen == 167 || idAgen == 182)
                {
                    item = from r in _context.ProdAnalisis_Residuo
                           join c in _context.ProdCamposCat on new { r.Cod_Empresa, r.Cod_Prod, r.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                           from mcam in MuestreoCam.DefaultIfEmpty()
                           join s in _context.ProdMuestreoSector on r.IdSector equals s.id into MuestreoSc
                           from ms in MuestreoSc.DefaultIfEmpty()
                           join p in _context.ProdProductoresCat on r.Cod_Prod equals p.Cod_Prod into MuestreoProd
                           from prod in MuestreoProd.DefaultIfEmpty()
                           join l in _context.ProdZonasRastreoCat on mcam.IdZona equals l.IdZona into Zonas
                           from z in Zonas.DefaultIfEmpty()
                           join t in _context.CatTiposProd on mcam.Tipo equals t.Tipo into Tipos
                           from tp in Tipos.DefaultIfEmpty()
                           join pr in _context.CatProductos on new { mcam.Tipo, mcam.Producto } equals new { pr.Tipo, pr.Producto } into Productos
                           from prd in Productos.DefaultIfEmpty()
                           orderby r.Fecha
                           select new AnalisisClass
                           {
                               Cod_Prod = r.Cod_Prod,
                               Cod_Campo = r.Cod_Campo,
                               Sector = (short)ms.Sector,
                               Productor = prod.Nombre,
                               Tipo = tp.Descripcion,
                               Producto = prd.Descripcion,
                               Zona = z.DescZona,
                               Fecha_envio = (DateTime)r.Fecha_envio,
                               Fecha_entrega = (DateTime)r.Fecha_entrega,
                               Estatus =
                                (
                                    r.Estatus == "R" ? "CON RESIDUOS" :
                                    r.Estatus == "P" ? "EN PROCESO" :
                                    r.Estatus == "F" ? "FUERA DEL LIMITE" :
                                    r.Estatus == "L" ? "LIBERADO" : ""
                                ),
                               Num_analisis = r.Num_analisis,
                               Laboratorio = r.Laboratorio,
                               LiberacionUSA = (DateTime)r.LiberacionUSA,
                               LiberacionEU = (DateTime)r.LiberacionEU,
                               Comentarios = r.Comentarios,
                               IdAgen = mcam.IdAgen,
                               IdAgenC = (short?)(int)mcam.IdAgenC,
                               IdAgenI = mcam.IdAgenI,
                               Fecha = r.Fecha
                           };
                }
                else
                {
                    item = from r in _context.ProdAnalisis_Residuo
                           join c in _context.ProdCamposCat on new { r.Cod_Empresa, r.Cod_Prod, r.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                           from mcam in MuestreoCam.DefaultIfEmpty()
                           join s in _context.ProdMuestreoSector on r.IdSector equals s.id into MuestreoSc
                           from ms in MuestreoSc.DefaultIfEmpty()
                           join p in _context.ProdProductoresCat on r.Cod_Prod equals p.Cod_Prod into MuestreoProd
                           from prod in MuestreoProd.DefaultIfEmpty()
                           join l in _context.ProdZonasRastreoCat on mcam.IdZona equals l.IdZona into Zonas
                           from z in Zonas.DefaultIfEmpty()
                           join t in _context.CatTiposProd on mcam.Tipo equals t.Tipo into Tipos
                           from tp in Tipos.DefaultIfEmpty()
                           join pr in _context.CatProductos on new { mcam.Tipo, mcam.Producto } equals new { pr.Tipo, pr.Producto } into Productos
                           from prd in Productos.DefaultIfEmpty()
                           where mcam.IdAgen == idAgen || mcam.IdAgenC == idAgen || mcam.IdAgenI == idAgen
                           orderby r.Fecha
                           select new AnalisisClass
                           {
                               Cod_Prod = r.Cod_Prod,
                               Cod_Campo = r.Cod_Campo,
                               Sector = (short)ms.Sector,
                               Productor = prod.Nombre,
                               Tipo = tp.Descripcion,
                               Producto = prd.Descripcion,                               
                               Zona = z.DescZona,
                               Fecha_envio = (DateTime)r.Fecha_envio,
                               Fecha_entrega = (DateTime)r.Fecha_entrega,
                               Estatus =
                                (
                                    r.Estatus == "R" ? "CON RESIDUOS" :
                                    r.Estatus == "P" ? "EN PROCESO" :
                                    r.Estatus == "F" ? "FUERA DEL LIMITE" :
                                    r.Estatus == "L" ? "LIBERADO" : ""
                                ),
                               Num_analisis = r.Num_analisis,
                               Laboratorio = r.Laboratorio,
                               LiberacionUSA = (DateTime)r.LiberacionUSA,
                               LiberacionEU = (DateTime)r.LiberacionEU,
                               Comentarios = r.Comentarios,
                               Fecha = r.Fecha
                           };
                }
                if (id != 0)
                {
                    item = item.Where(x => x.IdAnalisis_Residuo == id).Distinct();
                }              
                else
                {
                    item = item.Distinct();
                }

                return Ok(item.OrderByDescending(x => x.Fecha).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<AnalisisController>
        [HttpPost("{id_Muestreo}/{liberacion_USA}/{liberacion_EU}/{sector}")]
        public ActionResult Post(int id_Muestreo, int liberacion_USA, int liberacion_EU, short sector, [FromBody] ProdAnalisis_Residuo model)
        {
            try
            {
                ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                int IdSector = 0;
                DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
                var muestreo = _context.ProdMuestreo.Find(id_Muestreo);

                if (muestreo != null)
                {
                    var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(m => m.Cod_Prod == muestreo.Cod_Prod && m.Cod_Campo == muestreo.Cod_Campo && m.Fecha_entrega == model.Fecha_entrega && m.Fecha_envio == model.Fecha_envio && m.Estatus == model.Estatus && m.Num_analisis == model.Num_analisis && m.Temporada == catSemanas.Temporada);

                    if (analisis == null)
                    {
                        var model_sector = _context.ProdMuestreoSector.FirstOrDefault(m => m.Cod_Prod == muestreo.Cod_Prod && m.Cod_Campo == muestreo.Cod_Campo && m.Sector == sector);
                        if (model_sector == null)
                        {
                            prodMuestreoSector.Cod_Prod = model.Cod_Prod;
                            prodMuestreoSector.Cod_Campo = model.Cod_Campo;
                            prodMuestreoSector.Sector = sector;
                            _context.ProdMuestreoSector.Add(prodMuestreoSector);
                            _context.SaveChanges();

                            IdSector = prodMuestreoSector.id;
                        }
                        else
                        {
                            IdSector = model_sector.id;
                        }

                        model.Cod_Empresa = 2;
                        model.Cod_Prod = muestreo.Cod_Prod;
                        model.Cod_Campo = muestreo.Cod_Campo;
                        model.Fecha = DateTime.Now;
                        model.Temporada = catSemanas.Temporada;
                        model.IdSector = IdSector;
                        model.Id_Muestreo = id_Muestreo;

                        if (model.Estatus == "F")
                        {
                            fechaLiberacionUSA = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_USA);
                            fechaLiberacionEU = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_EU);

                            model.LiberacionUSA = fechaLiberacionUSA;
                            model.LiberacionEU = fechaLiberacionEU;
                        }

                        _context.ProdAnalisis_Residuo.Add(model);
                        _context.SaveChanges();

                        string estatus = "";

                        if (model.Estatus == "R")
                        {
                            estatus = "CON RESIDUOS";
                        }
                        else if (model.Estatus == "P")
                        {
                            estatus = "EN PROCESO";
                        }
                        else if (model.Estatus == "F")
                        {
                            estatus = "FUERA DEL LIMITE";
                        }
                        else if (model.Estatus == "L")
                        {
                            estatus = "LIBERADO";
                        }

                        title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                        body = "Resultado del análisis: " + estatus;
                        notificaciones.SendNotificationJSON(title, body);

                        return Ok();
                    }

                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<AnalisisController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AnalisisController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
