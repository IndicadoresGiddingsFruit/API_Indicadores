using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiIndicadores.Context;
using ApiIndicadores.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using ApiIndicadores.Classes.Visitas;

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitasController : ControllerBase
    {  
        private readonly AppDbContext _context;
        public VisitasController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<VisitasController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdVisitasCab.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //GET Visitas por día
        [HttpGet("{idAgen}/{fecha}")]
        public ActionResult Get(short idAgen = 0, string fecha="")
        {
            try
            {
                if (fecha != "null")
                {
                    List<VisitasReport> reporte = _context.VisitasReport.FromSqlRaw($"Select cat.Nombre as Asesor,cab.IdVisita, sec.IdSector,cab.Cod_prod, prod.Nombre as Productor,cab.Cod_Campo, cam.Descripcion as Campo, rtrim(tpo.Descripcion) + ' - ' + rtrim(pto.Descripcion) as Tipo, pto.Descripcion as Producto, convert(VARCHAR(20), cab.Fecha, 103) as Fecha, " +
                     "cab.Comentarios, cab.Atendio, isnull(icat.DescIncidencia,'') as DescIncidencia, isnull(etp.DescEtapa,'') as Etapa, isnull(det.Comentario,'') as Folio " +
                     "FROM ProdVisitasCab cab " +
                     "inner join ProdAgenteCat cat on cab.IdAgen = cat.IdAgen " +
                     "left join ProdProductoresCat prod on cab.Cod_prod = prod.Cod_Prod " +
                     "left join ProdCamposCat cam on cab.Cod_prod = cam.Cod_Prod and cab.Cod_Campo = cam.Cod_Campo " +
                     "left join CatTiposProd tpo on cam.Tipo = tpo.Tipo " +
                     "left join CatProductos pto on cam.Tipo = pto.Tipo and cam.Producto = pto.Producto " +
                     "left join ProdVisitasDet det on cab.IdVisita = det.IdVisita " +
                     "left join(select cab.IdVisita, i.IdIncidencia, STUFF((SELECT distinct ',' + i.DescIncidencia FROM ProdVisitasCab cab left join ProdVisitasDet det on cab.IdVisita = det.IdVisita left join ProdIncidenciasCat i on det.IdIncidencia = i.IdIncidencia " +
                     "where convert(varchar, cab.Fecha, 23) = '" + fecha + "' and cab.IdAgen = " + idAgen + " FOR XML PATH('')), 1, 1, '') as DescIncidencia " +
                     "FROM ProdVisitasCab cab left join ProdVisitasDet det on cab.IdVisita = det.IdVisita left join ProdIncidenciasCat i on det.IdIncidencia = i.IdIncidencia)icat on icat.IdVisita = cab.IdVisita and det.IdIncidencia = icat.IdIncidencia " +
                     "left join ProdVisitasSectores sec on cab.IdVisita = sec.IdVisita " +
                     "left join ProdEtapasCat etp on det.IdEtapa = etp.IdEtapa " +
                     "where convert(varchar, cab.Fecha, 23) = '" + fecha + "' and cat.IdAgen = " + idAgen + " " +
                     "group by cat.Nombre, cab.IdVisita, sec.IdSector, cab.Cod_prod, prod.Nombre, cab.Cod_Campo, cam.Descripcion, tpo.Descripcion, pto.Descripcion, cab.Fecha, cab.Comentarios, cab.Atendio, icat.DescIncidencia,etp.DescEtapa, det.Comentario, cab.IdSector " +
                     "order by cab.Fecha, cab.Cod_prod, cab.Cod_Campo, sec.IdSector").ToList();
                    return Ok(reporte);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e) {
                return BadRequest(e.ToString());
            }
        }

        [HttpGet("{idAgen}")]
        public ActionResult Get(short idAgen = 0)
        {
            try
            {
                //grafica = _context.VisitasGraph.FromSqlRaw($"SET LANGUAGE Spanish; select SUM(C.TotalCamposVisit) as Total, C.Mes from(select V.IdAgen, V.Mes, COUNT(V.Cod_Campo) as TotalCamposVisit " +
                //    "from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes " +
                //    "from ProdVisitasCab V LEFT JOIN ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                //    "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin " +
                //    "where S.Temporada = (select temporada from catsemanas where getdate() between inicio and fin) and C.Activo = 'S')V group by V.IdAgen, V.Mes)C " +
                //    "where c.IdAgen = " + idAgen + " group by C.IdAgen, C.Mes " +
                //    "order by(case when C.Mes = 'Julio' then 1 when C.Mes = 'Agosto' then 2 when C.Mes = 'Septiembre' then 3 when C.Mes = 'Octubre' then 4 when C.Mes = 'Noviembre' then 5 when C.Mes = 'Diciembre' then 6 when C.Mes = 'Enero' then 7 when C.Mes = 'Febrero' then 8 when C.Mes = 'Marzo' then 9 when C.Mes = 'Abril' then 10 when C.Mes = 'Mayo' then 11 when C.Mes = 'Junio' then 12 else '0' end) ").ToList();

                if (idAgen == 50 || idAgen == 1 || idAgen == 196 || idAgen == 2 || idAgen == 115 || idAgen == 5 || idAgen == 259)
                { 
                    var visitas = _context.VisitasTotal.FromSqlRaw($"sp_GetVisitas " + idAgen + "").ToList();
                    var visitasMes = _context.VisitasMes.FromSqlRaw($"sp_GetVisitasMes " + idAgen + "").ToList();

                    var res = Tuple.Create(visitas.ToList(), visitasMes.ToList());
                    return Ok(res); 
                }
                else
                {
                   var visitas = _context.VisitasTable.FromSqlRaw($"sp_GetVisitas " + idAgen + "").ToList();
                    return Ok(visitas);
                }            
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }            
        }
       
    }


}
