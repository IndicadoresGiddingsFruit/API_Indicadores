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

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitasController : ControllerBase
    {
        List<VisitasReport> reporte = null;
        Image img = null;
        List<VisitasTable> visitas = null;

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

        //GET api/<VisitasController>/5
        [HttpGet("{fecha}/{idAgen}", Name = "GetVisitas")]
        public Tuple<List<VisitasTable>, List<VisitasGraph>, List<VisitasReport>> Get(string fecha="", short idAgen=0)
        {
           
            List<VisitasGraph> grafica = null;

            if (fecha != "null")
            {
                reporte = _context.VisitasReport.FromSqlRaw($"Select cat.Nombre as Asesor,cab.IdVisita, sec.IdSector,cab.Cod_prod, prod.Nombre as Productor,cab.Cod_Campo, cam.Descripcion as Campo, rtrim(tpo.Descripcion) + ' - ' + rtrim(pto.Descripcion) as Tipo, pto.Descripcion as Producto, convert(VARCHAR(20), cab.Fecha, 103) as Fecha, " +
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
               
            }
            else
            {
                visitas = _context.VisitasTable.FromSqlRaw($"SET LANGUAGE Spanish; select V.Mes, V.TotalCampos, V.TotalCamposVisit, V.Eficiencia, cast(round(ISNULL(V.Efectividad*100,0),0) as varchar)+'%' as Efectividad, " +
             "round(V.Suma / (D1 + D2 + D3 + D4 + D5 + D6 + D7 + D8 + D9 + D10 + D11 + D12 + D13 + D14 + D15 + D16 + D17 + D18 + D19 + D20 + D21 + D22 + D23 + D24 + D25 + D26 + D27 + D28 + D29 + D30 + D31), 0) as Promedio, " +
             "V._1, V._2, V._3, V._4, V._5, V._6, V._7, V._8, V._9, V._10, V._11, V._12, V._13, V._14, V._15, V._16, V._17, V._18, V._19, V._20, V._21, V._22, V._23, V._24, V._25, V._26, V._27, V._28, V._29, V._30, V._31 " +
             "from(select V.IdAgen, V.Mes, V.TotalCampos, V.TotalCamposVisit, cast(round((V.Eficiencia * 100), 0) as varchar) + '%' as Eficiencia, (V.TotalCamposVisit / V.TotalCampos) as Efectividad, " +
             "(isnull(V.[1], 0) + isnull(V.[2], 0) + isnull(V.[3], 0) + isnull(V.[4], 0) + isnull(V.[5], 0) + isnull(V.[6], 0) + isnull(V.[7], 0) + isnull(V.[8], 0) + isnull(V.[9], 0) + isnull(V.[10], 0) + isnull(V.[11], 0) + isnull(V.[12], 0)+" +
             "isnull(V.[13], 0) + isnull(V.[14], 0) + isnull(V.[15], 0) + isnull(V.[16], 0) + isnull(V.[17], 0) + isnull(V.[18], 0) + isnull(V.[19], 0) + isnull(V.[20], 0) + isnull(V.[21], 0) + isnull(V.[22], 0) + isnull(V.[23], 0)+ isnull(V.[24], 0)+ " +
             "isnull(V.[25], 0) + isnull(V.[26], 0) + isnull(V.[27], 0) + isnull(V.[28], 0) + isnull(V.[29], 0) + isnull(V.[30], 0) + isnull(V.[31], 0)) AS Suma, " +
             "(case when V.[1] is not null then 1 else 0 END)AS D1,(case when V.[2] is not null then 1 else 0 END) AS D2,(case when V.[3] is not null then 1 else 0 END) AS D3,(case when V.[4] is not null then 1 else 0 END) AS D4, " +
             "(case when V.[5] is not null then 1 else 0 END) AS D5,(case when V.[6] is not null then 1 else 0 END) AS D6,(case when V.[7] is not null then 1 else 0 END) AS D7,(case when V.[8] is not null then 1 else 0 END) AS D8, " +
             "(case when V.[9] is not null then 1 else 0 END) AS D9,(case when V.[10] is not null then 1 else 0 END) AS D10,(case when V.[11] is not null then 1 else 0 END) AS D11,(case when V.[12] is not null then 1 else 0 END) AS D12, " +
             "(case when V.[13] is not null then 1 else 0 END) AS D13,(case when V.[14] is not null then 1 else 0 END) AS D14,(case when V.[15] is not null then 1 else 0 END) AS D15,(case when V.[16] is not null then 1 else 0 END)AS D16, " +
             "(case when V.[17] is not null then 1 else 0 END) AS D17, (case when V.[18] is not null then 1 else 0 END) AS D18, (case when V.[19] is not null then 1 else 0 END) AS D19,(case when V.[20] is not null then 1 else 0 END) AS D20, " +
             "(case when V.[21] is not null then 1 else 0 END) AS D21, (case when V.[22] is not null then 1 else 0 END) AS D22, (case when V.[23] is not null then 1 else 0 END) AS D23,(case when V.[24] is not null then 1 else 0 END) AS D24, " +
             "(case when V.[25] is not null then 1 else 0 END) AS D25,(case when V.[26] is not null then 1 else 0 END) AS D26, (case when V.[27] is not null then 1 else 0 END) AS D27, (case when V.[28] is not null then 1 else 0 END) AS D28, " +
             "(case when V.[29] is not null then 1 else 0 END) AS D29, (case when V.[30] is not null then 1 else 0 END) AS D30,(case when V.[31] is not null then 1 else 0 END) AS D31, " +
             "isnull(LTRIM(STR(V.[1], 3, 0)), '') as _1,isnull(LTRIM(STR(V.[2], 3, 0)), '') AS _2, isnull(LTRIM(STR(V.[3], 3, 0)), '') AS _3, isnull(LTRIM(STR(V.[4], 3, 0)), '') AS _4, isnull(LTRIM(STR(V.[5], 3, 0)), '') as _5,isnull(LTRIM(STR(V.[6], 3, 0)), '') AS _6, isnull(LTRIM(STR(V.[7], 3, 0)), '') as _7, " +
             "isnull(LTRIM(STR(V.[8], 3, 0)), '') AS _8, isnull(LTRIM(STR(V.[9], 3, 0)), '') AS _9, isnull(LTRIM(STR(V.[10], 3, 0)), '') AS _10, isnull(LTRIM(STR(V.[11], 3, 0)), '') AS _11, isnull(LTRIM(STR(V.[12], 3, 0)), '') AS _12, isnull(LTRIM(STR(V.[13], 3, 0)), '') AS _13, isnull(LTRIM(STR(V.[14], 3, 0)), '') AS _14, " +
             "isnull(LTRIM(STR(V.[15], 3, 0)), '') AS _15, isnull(LTRIM(STR(V.[16], 3, 0)), '') AS _16, isnull(LTRIM(STR(V.[17], 3, 0)), '') AS _17, isnull(LTRIM(STR(V.[18], 3, 0)), '') AS _18, isnull(LTRIM(STR(V.[19], 3, 0)), '') AS _19, isnull(LTRIM(STR(V.[20], 3, 0)), '') AS _20, isnull(LTRIM(STR(V.[21], 3, 0)), '') AS _21, " +
             "isnull(LTRIM(STR(V.[22], 3, 0)), '') AS _22, isnull(LTRIM(STR(V.[23], 3, 0)), '') AS _23, isnull(LTRIM(STR(V.[24], 3, 0)), '') AS _24, isnull(LTRIM(STR(V.[25], 3, 0)), '') AS _25, isnull(LTRIM(STR(V.[26], 3, 0)), '') AS _26, isnull(LTRIM(STR(V.[27], 3, 0)), '') AS _27, isnull(LTRIM(STR(V.[28], 3, 0)), '') AS _28, " +
             "isnull(LTRIM(STR(V.[29], 3, 0)), '') AS _29, isnull(LTRIM(STR(V.[30], 3, 0)), '') AS _30, isnull(LTRIM(STR(V.[31], 3, 0)), '') AS _31 " +
             "from(select * from(select V.IdAgen, cast(sum(V.Campos) as float) as Campos, V.Mes, V.Dia, T.TotalCampos, C.TotalCamposVisit, E.Eficiencia " +
             "FROM(select V.IdAgen, count(V.Cod_Campo) as Campos, V.Año, V.Mes, V.Semana, V.Dia, V.Fecha " +
             "from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(year, V.Fecha) as Año, DATENAME(month, V.Fecha) as Mes, S.Semana, DATENAME(day, V.Fecha) as Dia " +
             "from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
             "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = (select Temporada from Catsemanas where getdate() between Inicio and Fin) and C.Activo = 'S')V GROUP BY V.IdAgen, V.Año, V.Mes, V.Semana, V.Dia, V.Fecha)V " +
             "left join(select C.IdAgen, C.Mes, cast(SUM(C.TotalCamposVisit) as float) as TotalCamposVisit from(select V.IdAgen, V.Mes, COUNT(V.Cod_Prod) as TotalProductoresVisit, COUNT(V.Cod_Campo) as TotalCamposVisit " +
             "from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
             "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = (select Temporada from Catsemanas where getdate() between Inicio and Fin) and C.Activo = 'S')V group by V.IdAgen, V.Mes)C group by C.IdAgen, C.Mes)C ON  V.IdAgen = C.IdAgen AND V.Mes = C.Mes " +
             "LEFT JOIN(select C.IdAgen, SUM(C.Campos)as TotalCampos from(select C.IdAgen, count(C.Productores) AS Productores, count(C.Campos) AS Campos from(SELECT distinct IdAgen, Cod_Prod as Productores, Cod_Campo as Campos " +
             "from ProdCamposCat where Activo = 'S')C group by C.IdAgen)C group by C.IdAgen)T ON  V.IdAgen = T.IdAgen left join(select (CAST(sum(V.Dia) AS float)/ 26) as Eficiencia, V.Mes, V.IdAgen from(select V.IdAgen, V.Mes, count(V.Dia) as Dia " +
             "from(select distinct V.IdAgen, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes, DATENAME(day, V.Fecha) as Dia from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
             "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = (select temporada from catsemanas where getdate() between inicio and fin) and C.Activo = 'S' )V GROUP BY V.IdAgen, V.Fecha, V.Mes, V.Dia )V GROUP BY V.Mes, V.Dia, V.IdAgen )E ON  V.IdAgen = E.IdAgen AND V.Mes = E.Mes " +
             "WHERE V.IdAgen  = " + idAgen + " " +
             "GROUP BY V.IdAgen, V.Fecha, V.Año, V.Mes, V.Dia, T.TotalCampos, C.TotalCamposVisit, E.Eficiencia)V " +
             "pivot(sum(Campos) FOR Dia in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31])) as P)V " +
             ")V order by(case when V.Mes = 'Julio' then 1 when V.Mes = 'Agosto' then 2 when V.Mes = 'Septiembre' then 3 when V.Mes = 'Octubre' then 4 when V.Mes = 'Noviembre' then 5 when V.Mes = 'Diciembre' then 6 when V.Mes = 'Enero' then 7 when V.Mes = 'Febrero' then 8 when V.Mes = 'Marzo' then 9 when V.Mes = 'Abril' then 10 when V.Mes = 'Mayo' then 11 when V.Mes = 'Junio' then 12 else '0' end)").ToList();

                //visitas = _context.VisitasTable.FromSqlRaw($"SET LANGUAGE Spanish; select V.Mes, V.TotalCampos, V.TotalCamposVisit, V.Eficiencia, cast(round(ISNULL(V.Efectividad*100,0),0) as varchar)+'%' as Efectividad, " +
                //    $"round(V.Suma / (D1 + D2 + D3 + D4 + D5 + D6 + D7 + D8 + D9 + D10 + D11 + D12 + D13 + D14 + D15 + D16 + D17 + D18 + D19 + D20 + D21 + D22 + D23 + D24 + D25 + D26 + D27 + D28 + D29 + D30 + D31), 0) as Promedio," +
                //    $"V._1, V._2, V._3, V._4, V._5, V._6, V._7, V._8, V._9, V._10, V._11, V._12, V._13, V._14, V._15, V._16, V._17, V._18, V._19, V._20, V._21, V._22, V._23, V._24, V._25, V._26, V._27, V._28, V._29, V._30, V._31 " +
                //    $"from(select V.IdAgen, V.Mes, V.TotalCampos, V.TotalCamposVisit, cast(round((V.Eficiencia * 100), 0) as varchar) + '%' as Eficiencia, (V.TotalCamposVisit / V.TotalCampos) as Efectividad, " +
                //    $"(isnull(V.[1], 0) + isnull(V.[2], 0) + isnull(V.[3], 0) + isnull(V.[4], 0) + isnull(V.[5], 0) + isnull(V.[6], 0) + isnull(V.[7], 0) + isnull(V.[8], 0) + isnull(V.[9], 0) + isnull(V.[10], 0) + isnull(V.[11], 0) + isnull(V.[12], 0) +" +
                //    $"isnull(V.[13], 0) + isnull(V.[14], 0) + isnull(V.[15], 0) + isnull(V.[16], 0) + isnull(V.[17], 0) + isnull(V.[18], 0) + isnull(V.[19], 0) + isnull(V.[20], 0) + isnull(V.[21], 0) + isnull(V.[22], 0) + isnull(V.[23], 0) + isnull(V.[24], 0) +" +
                //    $"isnull(V.[25], 0) + isnull(V.[26], 0) + isnull(V.[27], 0) + isnull(V.[28], 0) + isnull(V.[29], 0) + isnull(V.[30], 0) + isnull(V.[31], 0)) AS Suma, " +
                //    $"(case when V.[1] is not null then 1 else 0 END)AS D1,(case when V.[2] is not null then 1 else 0 END) AS D2,(case when V.[3] is not null then 1 else 0 END) AS D3,(case when V.[4] is not null then 1 else 0 END) AS D4," +
                //    $"(case when V.[5] is not null then 1 else 0 END) AS D5,(case when V.[6] is not null then 1 else 0 END) AS D6,(case when V.[7] is not null then 1 else 0 END) AS D7,(case when V.[8] is not null then 1 else 0 END) AS D8," +
                //    $"(case when V.[9] is not null then 1 else 0 END) AS D9,(case when V.[10] is not null then 1 else 0 END) AS D10,(case when V.[11] is not null then 1 else 0 END) AS D11,(case when V.[12] is not null then 1 else 0 END) AS D12," +
                //    $"(case when V.[13] is not null then 1 else 0 END) AS D13,(case when V.[14] is not null then 1 else 0 END) AS D14,(case when V.[15] is not null then 1 else 0 END) AS D15,(case when V.[16] is not null then 1 else 0 END)AS D16," +
                //    $"(case when V.[17] is not null then 1 else 0 END) AS D17, (case when V.[18] is not null then 1 else 0 END) AS D18, (case when V.[19] is not null then 1 else 0 END) AS D19,(case when V.[20] is not null then 1 else 0 END) AS D20," +
                //    $"(case when V.[21] is not null then 1 else 0 END) AS D21, (case when V.[22] is not null then 1 else 0 END) AS D22, (case when V.[23] is not null then 1 else 0 END) AS D23,(case when V.[24] is not null then 1 else 0 END) AS D24," +
                //    $"(case when V.[25] is not null then 1 else 0 END) AS D25,(case when V.[26] is not null then 1 else 0 END) AS D26, (case when V.[27] is not null then 1 else 0 END) AS D27, (case when V.[28] is not null then 1 else 0 END) AS D28," +
                //    $"                    (case when V.[29] is not null then 1 else 0 END) AS D29, (case when V.[30] is not null then 1 else 0 END) AS D30,(case when V.[31] is not null then 1 else 0 END) AS D31," +
                //    $"                    isnull(LTRIM(STR(V.[1], 3, 0)), '') as _1,isnull(LTRIM(STR(V.[2], 3, 0)), '') AS _2, isnull(LTRIM(STR(V.[3], 3, 0)), '') AS _3, isnull(LTRIM(STR(V.[4], 3, 0)), '') AS _4, isnull(LTRIM(STR(V.[5], 3, 0)), '') as _5,isnull(LTRIM(STR(V.[6], 3, 0)), '') AS _6, isnull(LTRIM(STR(V.[7], 3, 0)), '') as _7, " +
                //    $"isnull(LTRIM(STR(V.[8], 3, 0)), '') AS _8, isnull(LTRIM(STR(V.[9], 3, 0)), '') AS _9, isnull(LTRIM(STR(V.[10], 3, 0)), '') AS _10, isnull(LTRIM(STR(V.[11], 3, 0)), '') AS _11, isnull(LTRIM(STR(V.[12], 3, 0)), '') AS _12, isnull(LTRIM(STR(V.[13], 3, 0)), '') AS _13, isnull(LTRIM(STR(V.[14], 3, 0)), '') AS _14," +
                //    $"isnull(LTRIM(STR(V.[15], 3, 0)), '') AS _15, isnull(LTRIM(STR(V.[16], 3, 0)), '') AS _16, isnull(LTRIM(STR(V.[17], 3, 0)), '') AS _17, isnull(LTRIM(STR(V.[18], 3, 0)), '') AS _18, isnull(LTRIM(STR(V.[19], 3, 0)), '') AS _19, isnull(LTRIM(STR(V.[20], 3, 0)), '') AS _20, isnull(LTRIM(STR(V.[21], 3, 0)), '') AS _21," +
                //    $"isnull(LTRIM(STR(V.[22], 3, 0)), '') AS _22, isnull(LTRIM(STR(V.[23], 3, 0)), '') AS _23, isnull(LTRIM(STR(V.[24], 3, 0)), '') AS _24, isnull(LTRIM(STR(V.[25], 3, 0)), '') AS _25, isnull(LTRIM(STR(V.[26], 3, 0)), '') AS _26, isnull(LTRIM(STR(V.[27], 3, 0)), '') AS _27, isnull(LTRIM(STR(V.[28], 3, 0)), '') AS _28," +
                //    $"isnull(LTRIM(STR(V.[29], 3, 0)), '') AS _29, isnull(LTRIM(STR(V.[30], 3, 0)), '') AS _30, isnull(LTRIM(STR(V.[31], 3, 0)), '') AS _31 " +
                //    $"from(select * from(select V.IdAgen, cast(sum(V.Campos) as float) as Campos, V.Mes, V.Dia, T.TotalCampos, C.TotalCamposVisit, E.Eficiencia " +
                //    $"FROM(select V.IdAgen, count(V.Cod_Campo) as Campos, V.Año, V.Mes, V.Semana, V.Dia, V.Fecha " +
                //    $"from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(year, V.Fecha) as Año, DATENAME(month, V.Fecha) as Mes, S.Semana, DATENAME(day, V.Fecha) as Dia " +
                //    $"from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                //    $"LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = 2021 and C.Activo = 'S')V GROUP BY V.IdAgen, V.Año, V.Mes, V.Semana, V.Dia, V.Fecha)V " +
                //    $"left join(select C.IdAgen, C.Mes, cast(SUM(C.TotalCamposVisit) as float) as TotalCamposVisit from(select V.IdAgen, V.Mes, COUNT(V.Cod_Prod) as TotalProductoresVisit, COUNT(V.Cod_Campo) as TotalCamposVisit " +
                //    $"from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                //    $"LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = 2021 and C.Activo = 'S')V group by V.IdAgen, V.Mes)C group by C.IdAgen, C.Mes)C ON  V.IdAgen = C.IdAgen AND V.Mes = C.Mes " +
                //    $"LEFT JOIN(select C.IdAgen, SUM(C.Campos) as TotalCampos from(select C.IdAgen, count(C.Productores) AS Productores, count(C.Campos) AS Campos from(SELECT distinct IdAgen, Cod_Prod as Productores, Cod_Campo as Campos " +
                //    $"from ProdCamposCat where Activo = 'S')C group by C.IdAgen)C group by C.IdAgen)T ON  V.IdAgen = T.IdAgen left join(select(CAST(sum(V.Dia) AS float) / 26) as Eficiencia, V.Mes, V.IdAgen from(select V.IdAgen, V.Mes, count(V.Dia) as Dia " +
                //    $"from(select distinct V.IdAgen, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes, DATENAME(day, V.Fecha) as Dia from ProdVisitasCab V left join ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                //    $"LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin where S.Temporada = 2021 and C.Activo = 'S')V GROUP BY V.IdAgen, V.Fecha, V.Mes, V.Dia)V GROUP BY V.Mes, V.Dia, V.IdAgen)E ON  V.IdAgen = E.IdAgen AND V.Mes = E.Mes " +
                //    $"WHERE V.IdAgen = "+idAgen+" " +
                //    $"GROUP BY V.IdAgen, V.Fecha, V.Año, V.Mes, V.Dia, T.TotalCampos, C.TotalCamposVisit, E.Eficiencia)V " +
                //    $"pivot(sum(Campos) FOR Dia in ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31])) as P)V " +
                //    $")V order by(case when V.Mes = 'Julio' then 1 when V.Mes = 'Agosto' then 2 when V.Mes = 'Septiembre' then 3 when V.Mes = 'Octubre' then 4 when V.Mes = 'Noviembre' then 5 when V.Mes = 'Diciembre' then 6 when V.Mes = 'Enero' then 7 when V.Mes = 'Febrero' then 8 when V.Mes = 'Marzo' then 9 when V.Mes = 'Abril' then 10 when V.Mes = 'Mayo' then 11 " +
                //    $"when V.Mes = 'Junio' then 12 else '0' end)").ToList();


                grafica = _context.VisitasGraph.FromSqlRaw($"SET LANGUAGE Spanish; select SUM(C.TotalCamposVisit) as Total, C.Mes from(select V.IdAgen, V.Mes, COUNT(V.Cod_Campo) as TotalCamposVisit " +
                    "from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes " +
                    "from ProdVisitasCab V LEFT JOIN ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                    "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin " +
                    "where S.Temporada = (select temporada from catsemanas where getdate() between inicio and fin) and C.Activo = 'S')V group by V.IdAgen, V.Mes)C " +
                    "where c.IdAgen = " + idAgen + " group by C.IdAgen, C.Mes " +
                    "order by(case when C.Mes = 'Julio' then 1 when C.Mes = 'Agosto' then 2 when C.Mes = 'Septiembre' then 3 when C.Mes = 'Octubre' then 4 when C.Mes = 'Noviembre' then 5 when C.Mes = 'Diciembre' then 6 when C.Mes = 'Enero' then 7 when C.Mes = 'Febrero' then 8 when C.Mes = 'Marzo' then 9 when C.Mes = 'Abril' then 10 when C.Mes = 'Mayo' then 11 when C.Mes = 'Junio' then 12 else '0' end) ").ToList();

                //grafica = _context.VisitasGraph.FromSqlRaw($"SET LANGUAGE Spanish; select SUM(C.TotalCamposVisit) as Total, C.Mes from(select V.IdAgen, V.Mes, COUNT(V.Cod_Campo) as TotalCamposVisit " +
                // "from(select distinct V.IdAgen, V.Cod_Prod, V.Cod_Campo, CONVERT(VARCHAR(10), V.Fecha, 23) AS Fecha, DATENAME(month, V.Fecha) as Mes " +
                // "from ProdVisitasCab V LEFT JOIN ProdCamposCat C on V.IdAgen = C.IdAgen and V.Cod_Prod = C.Cod_Prod and V.Cod_Campo = C.Cod_Campo " +
                // "LEFT join CatSemanas S ON V.Fecha between S.Inicio and S.Fin " +
                // "where S.Temporada = 2021 and C.Activo = 'S')V group by V.IdAgen, V.Mes)C " +
                // "where c.IdAgen = " + idAgen + " group by C.IdAgen, C.Mes " +
                // "order by(case when C.Mes = 'Julio' then 1 when C.Mes = 'Agosto' then 2 when C.Mes = 'Septiembre' then 3 when C.Mes = 'Octubre' then 4 when C.Mes = 'Noviembre' then 5 when C.Mes = 'Diciembre' then 6 when C.Mes = 'Enero' then 7 when C.Mes = 'Febrero' then 8 when C.Mes = 'Marzo' then 9 when C.Mes = 'Abril' then 10 when C.Mes = 'Mayo' then 11 when C.Mes = 'Junio' then 12 else '0' end) ").ToList();

            }
            return Tuple.Create(visitas, grafica, reporte);
        }

        //public void RptView(string fecha, string asesor)
        //{
        //    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 20, 20, 10, 20);//left, rigth, top, bottom           
        //    var output = new MemoryStream();
        //    iTextSharp.text.pdf.PdfWriter pw = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, output);
        //    //Image logo = Image.GetInstance("/Image/GIDDINGS_PRIMARY_STACKED_LOGO_DRIFT_RGB.png");
        //    //pw.PageEvent = new HeaderFooter();

        //    Font _standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);

        //    doc.AddTitle(fecha);
        //    doc.AddCreator(asesor);
        //    doc.Open();
        //    PdfPTable pdfTab = new PdfPTable(3);

        //    doc.Add(new Paragraph("Asesor: " + asesor));
        //    doc.Add(new Paragraph("Fecha: " + fecha));
        //    doc.Add(Chunk.NEWLINE);

        //    PdfPTable tbl = new PdfPTable(9);
        //    tbl.WidthPercentage = 100;
        //    tbl.HorizontalAlignment = Element.ALIGN_CENTER;

        //    float[] values = new float[9];
        //    values[0] = 80;
        //    values[1] = 200;
        //    values[2] = 150;
        //    values[3] = 75;
        //    values[4] = 200;
        //    values[5] = 150;
        //    values[6] = 150;
        //    values[7] = 200;
        //    values[8] = 300;
        //    tbl.SetWidths(values);

        //    // Configuramos el título de las columnas de la tabla  
        //    PdfPCell clCod_prod = new PdfPCell(new Phrase("Código", _standardFont));
        //    clCod_prod.BorderWidth = 0;
        //    clCod_prod.BorderWidthBottom = 0.75f;

        //    PdfPCell clProductor = new PdfPCell(new Phrase("Productor", _standardFont));
        //    clProductor.BorderWidth = 0;
        //    clProductor.BorderWidthBottom = 0.75f;

        //    PdfPCell clCampo = new PdfPCell(new Phrase("Campo", _standardFont));
        //    clCampo.BorderWidth = 0;
        //    clCampo.BorderWidthBottom = 0.75f;

        //    PdfPCell clSector = new PdfPCell(new Phrase("Sector", _standardFont));
        //    clSector.BorderWidth = 0;
        //    clSector.BorderWidthBottom = 0.75f;

        //    PdfPCell clProducto = new PdfPCell(new Phrase("Producto", _standardFont));
        //    clProducto.BorderWidth = 0;
        //    clProducto.BorderWidthBottom = 0.75f;

        //    PdfPCell clAtendio = new PdfPCell(new Phrase("Atendió", _standardFont));
        //    clAtendio.BorderWidth = 0;
        //    clAtendio.BorderWidthBottom = 0.75f;

        //    PdfPCell clEtapa = new PdfPCell(new Phrase("Etapa", _standardFont));
        //    clEtapa.BorderWidth = 0;
        //    clEtapa.BorderWidthBottom = 0.75f;

        //    PdfPCell clComentarios = new PdfPCell(new Phrase("Comentarios", _standardFont));
        //    clComentarios.BorderWidth = 0;
        //    clComentarios.BorderWidthBottom = 0.75f;

        //    PdfPCell X = new PdfPCell(new Phrase("Fotografía", _standardFont));
        //    X.BorderWidth = 0;
        //    X.BorderWidthBottom = 0.75f;

        //    // Añadimos las celdas a la tabla            
        //    tbl.AddCell(clCod_prod);
        //    tbl.AddCell(clProductor);
        //    tbl.AddCell(clCampo);
        //    tbl.AddCell(clSector);
        //    tbl.AddCell(clProducto);
        //    tbl.AddCell(clAtendio);
        //    tbl.AddCell(clEtapa);
        //    tbl.AddCell(clComentarios);
        //    tbl.AddCell(X);

        //    Get(fecha, idAgen);
        
        //    int x = 2;
        //    foreach (var item in visitas)
        //    {
        //        clCod_prod = new PdfPCell(new Phrase(item.Cod_Prod, _standardFont));
        //        clCod_prod.BorderWidth = 0;

        //        clProductor = new PdfPCell(new Phrase(item.Productor, _standardFont));
        //        clProductor.BorderWidth = 0;

        //        clCampo = new PdfPCell(new Phrase(Convert.ToString(item.Cod_Campo + " - " + item.Campo), _standardFont));
        //        clCampo.BorderWidth = 0;

        //        clSector = new PdfPCell(new Phrase(Convert.ToString(item.IdSector), _standardFont));
        //        clSector.BorderWidth = 0;

        //        clProducto = new PdfPCell(new Phrase(item.Tipo, _standardFont));
        //        clProducto.BorderWidth = 0;

        //        clAtendio = new PdfPCell(new Phrase(item.Atendio, _standardFont));
        //        clAtendio.BorderWidth = 0;

        //        clEtapa = new PdfPCell(new Phrase(item.Etapa, _standardFont));
        //        clEtapa.BorderWidth = 0;

        //        clComentarios = new PdfPCell(new Phrase(item.Comentarios, _standardFont));
        //        clComentarios.BorderWidth = 0;

        //        tbl.AddCell(clCod_prod);
        //        tbl.AddCell(clProductor);
        //        tbl.AddCell(clCampo);
        //        tbl.AddCell(clSector);
        //        tbl.AddCell(clProducto);
        //        tbl.AddCell(clAtendio);
        //        tbl.AddCell(clEtapa);
        //        tbl.AddCell(clComentarios);

        //        if (ImgExist(item.IdVisita))
        //        {
        //            img.BorderWidth = 0;
        //            img.Alignment = Element.ALIGN_CENTER;
        //            float percentage = 0.0f;
        //            percentage = 150 / img.Width;
        //            img.ScalePercent(percentage * 100);
        //        }
        //        tbl.AddCell(img);
        //        x++;
        //    }

        //    //Logo            

        //    //logo.SetAbsolutePosition(0f, 0f);
        //    //logo.ScaleAbsolute(50f, 50f);

        //    //doc.Add(logo);
        //    doc.Add(tbl);
        //    doc.Close();
        //    pw.Close();

        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename=" + fecha + ".pdf", "some string"));
        //    Response.BinaryWrite(output.ToArray());
        //    Response.End();
        //}

        //class HeaderFooter : PdfPageEventHelper
        //{
        //    public override void OnEndPage(PdfWriter writer, Document document)
        //    {
        //        //base.OnEndPage(writer, document);
        //        PdfPTable tbHeader = new PdfPTable(3);
        //        tbHeader.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
        //        tbHeader.DefaultCell.Border = 0;

        //        PdfPCell _cell = new PdfPCell(new Paragraph("Visitas diarias"));
        //        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        tbHeader.AddCell(_cell);
        //        tbHeader.AddCell(new Paragraph());

        //        PdfPTable tbFooter = new PdfPTable(3);
        //        tbFooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
        //        tbFooter.DefaultCell.Border = 0;

        //        _cell = new PdfPCell(new Paragraph("Fruits - Giddings S.A.de C.V"));
        //        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        tbFooter.AddCell(_cell);
        //        tbFooter.AddCell(new Paragraph());
        //    }
        //}

        //public bool ImgExist(int IdVisita)
        //{
        //    try
        //    {
        //        img = Image.GetInstance("//192.168.0.21/recursos season/VisitasProd/" + IdVisita + "/1.jpg");
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        e.ToString();
        //        return false;
        //    }
        //}
    }


}
