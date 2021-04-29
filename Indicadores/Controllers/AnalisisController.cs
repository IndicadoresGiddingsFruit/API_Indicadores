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
                //var data = _context.VisitasTable.FromSqlRaw($"select R.Cod_Prod, R.Cod_Campo, ISNULL(S.Sector,0) as Sector, P.Nombre as Productor, T.Descripcion as Tipo, V.Descripcion as Producto, isnull(L.DescZona,'') as Zona, R.Fecha_envio, R.Fecha_entrega, (case when R.Estatus = 'R' THEN 'CON RESIDUOS' else case when R.Estatus = 'P' THEN 'EN PROCESO' else case when R.Estatus = 'F' THEN 'FUERA DE LIMITE' else case when R.Estatus = 'L' THEN 'LIBERADO' end end end end) as Estatus, R.Num_analisis, R.Laboratorio, isnull(convert(varchar, R.LiberacionUSA, 103), '') as LiberacionUSA, isnull(convert(varchar, R.LiberacionEU, 103), '') as LiberacionEU, UPPER(isnull(R.Comentarios, '')) as Comentarios " +
                //    "FROM ProdAnalisis_Residuo R LEFT JOIN ProdMuestreoSector S ON R.IdSector = S.id Left Join ProdProductoresCat P on R.Cod_Prod = P.Cod_Prod Left Join ProdCamposCat C on R.Cod_Prod = C.Cod_Prod and R.Cod_Campo = C.Cod_Campo Left Join CatTiposProd T on C.Tipo = T.Tipo Left Join CatProductos V on C.Tipo = V.Tipo AND C.Producto = V.Producto Left Join ProdZonasRastreoCat L on R.CodZona = L.Codigo " +
                //    "order by R.Fecha,  R.Cod_Prod, R.Cod_Campo, S.Sector, R.Num_analisis desc").Distinct();

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
                var item = (from an in _context.ProdAnalisis_Residuo

                            join s in _context.ProdMuestreoSector on an.IdSector equals s.id into Sectores
                            from ms in Sectores.DefaultIfEmpty()

                            join c in _context.ProdCamposCat on new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into Campos
                            from mcam in Campos.DefaultIfEmpty()

                            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into Productores
                            from p in Productores.DefaultIfEmpty()

                            join t in _context.CatTiposProd on mcam.Tipo equals t.Tipo into Tipos
                            from t in Tipos.DefaultIfEmpty()

                            join prod in _context.CatProductos on new { mcam.Tipo, mcam.Producto } equals new { prod.Tipo, prod.Producto } into Productos
                            from prod in Productos.DefaultIfEmpty()

                            join z in _context.ProdZonasRastreoCat on an.CodZona equals z.Codigo into Zonas
                            from z in Zonas.DefaultIfEmpty()

                            group an by new
                            {
                                IdAnalisis_Residuo = an.Id,
                                Cod_Prod = an.Cod_Prod,
                                Cod_Campo = an.Cod_Campo,
                                Sector = (short)ms.Sector,
                                Productor = p.Nombre,
                                Tipo = t.Descripcion,
                                Producto = prod.Descripcion,
                                Zona = z.DescZona,
                                Fecha_envio = an.Fecha_envio,
                                Fecha_entrega = an.Fecha_entrega,
                                Estatus = an.Estatus,
                                Num_analisis = an.Num_analisis,
                                Laboratorio = an.Laboratorio,
                                LiberacionUSA = an.LiberacionUSA,
                                LiberacionEU = an.LiberacionEU,
                                Comentarios = an.Comentarios,
                                IdAgen = mcam.IdAgen,
                                IdAgenC = mcam.IdAgenC,
                                IdAgenI = mcam.IdAgenI,
                                Fecha = an.Fecha
                            } into x
                            select new AnalisisClass()
                            {
                                IdAnalisis_Residuo = x.Key.IdAnalisis_Residuo,
                                Cod_Prod = x.Key.Cod_Prod,
                                Cod_Campo = x.Key.Cod_Campo,
                                Sector = (short)x.Key.Sector,
                                Productor = x.Key.Productor,
                                Tipo = x.Key.Tipo,
                                Producto = x.Key.Producto,
                                Zona = x.Key.Zona,
                                Fecha_envio = (DateTime)x.Key.Fecha_envio,
                                Fecha_entrega = (DateTime)x.Key.Fecha_entrega,
                                Estatus =
                                (
                                    x.Key.Estatus == "R" ? "CON RESIDUOS" :
                                    x.Key.Estatus == "P" ? "EN PROCESO" :
                                    x.Key.Estatus == "F" ? "FUERA DEL LIMITE" : 
                                    x.Key.Estatus == "L" ? "LIBERADO" : ""
                                ),
                                Num_analisis = x.Key.Num_analisis,
                                Laboratorio = x.Key.Laboratorio,
                                LiberacionUSA = x.Key.LiberacionUSA,
                                LiberacionEU = x.Key.LiberacionEU,
                                Comentarios = x.Key.Comentarios,
                                IdAgen = x.Key.IdAgen,
                                IdAgenC = (short?)x.Key.IdAgenC,
                                IdAgenI = x.Key.IdAgenI,
                                Fecha = x.Key.Fecha
                            });

                //item = _context.AnalisisClass.FromSqlRaw($"select R.Cod_Prod, R.Cod_Campo, ISNULL(S.Sector,0) as Sector, P.Nombre as Productor, T.Descripcion as Tipo, V.Descripcion as Producto, isnull(L.DescZona,'') as Zona, R.Fecha_envio, R.Fecha_entrega, (case when R.Estatus = 'R' THEN 'CON RESIDUOS' else case when R.Estatus = 'P' THEN 'EN PROCESO' else case when R.Estatus = 'F' THEN 'FUERA DE LIMITE' else case when R.Estatus = 'L' THEN 'LIBERADO' end end end end) as Estatus, R.Num_analisis, R.Laboratorio, isnull(convert(varchar, R.LiberacionUSA, 103), '') as LiberacionUSA, isnull(convert(varchar, R.LiberacionEU, 103), '') as LiberacionEU, UPPER(isnull(R.Comentarios, '')) as Comentarios " +
                //     "FROM ProdAnalisis_Residuo R LEFT JOIN ProdMuestreoSector S ON R.IdSector = S.id Left Join ProdProductoresCat P on R.Cod_Prod = P.Cod_Prod Left Join ProdCamposCat C on R.Cod_Prod = C.Cod_Prod and R.Cod_Campo = C.Cod_Campo Left Join CatTiposProd T on C.Tipo = T.Tipo Left Join CatProductos V on C.Tipo = V.Tipo AND C.Producto = V.Producto Left Join ProdZonasRastreoCat L on R.CodZona = L.Codigo " +
                //     "where C.IdAgen=" + idAgen + " or C.IdAgenC=" + idAgen + " or C.IdAgenI=" + idAgen + " " +
                //     "order by R.Fecha,  R.Cod_Prod, R.Cod_Campo, S.Sector, R.Num_analisis desc").Distinct();
                if (id != 0)
                {
                    item=item.Where(x => x.IdAnalisis_Residuo == id).Distinct();
                }
                else if (idAgen == 205)
                {
                    item=item.Distinct();
                }
                else
                {
                    item=item.Where(x => x.Estatus != "" && (x.IdAgen == idAgen || x.IdAgenC == idAgen || x.IdAgenI == idAgen)).Distinct();
                }

                return Ok(item.OrderByDescending(x => x.Fecha).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<AnalisisController>
        [HttpPost("{sector}/{liberacionUSA}/{liberacionEU}")]
        public ActionResult Post(short sector,int liberacionUSA, int liberacionEU,[FromBody] ProdAnalisis_Residuo model)
        {
            try
            {
                ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                int IdSector = 0;
                DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                var modeloExistente = _context.ProdAnalisis_Residuo.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Fecha_entrega == model.Fecha_entrega && m.Fecha_envio == model.Fecha_envio && m.Estatus == model.Estatus && m.Num_analisis == model.Num_analisis && m.Temporada==catSemanas.Temporada);

                if (modeloExistente == null)
                {
                    var model_sector = _context.ProdMuestreoSector.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Sector==sector);
                    if (model_sector == null) {
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
                    model.Fecha = DateTime.Now;
                    model.Temporada = catSemanas.Temporada;
                    model.IdSector = IdSector;
                    if (model.Estatus == "F")
                    {
                        fechaLiberacionUSA = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacionUSA);
                        fechaLiberacionEU = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacionEU);

                        model.LiberacionUSA = fechaLiberacionUSA;
                        model.LiberacionEU = fechaLiberacionEU;
                    }

                    _context.ProdAnalisis_Residuo.Add(model);
                    _context.SaveChanges();

                    string analisis = "";

                    if (model.Estatus == "R")
                    {
                        analisis = "CON RESIDUOS";
                    }
                    else if (model.Estatus == "P")
                    {
                        analisis = "EN PROCESO";
                    }
                    else if (model.Estatus == "F")
                    {
                        analisis = "FUERA DEL LIMITE";
                    }
                    else if (model.Estatus == "L")
                    {
                        analisis = "LIBERADO";
                    }

                    title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    body = "Resultado del análisis: " + analisis;
                    notificaciones.SendNotificationJSON(title, body);

                    return CreatedAtRoute("GetAnalisis", new { id = model.Id, model.IdAgen }, model);
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
