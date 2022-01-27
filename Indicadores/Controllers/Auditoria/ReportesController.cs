using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Excel = Microsoft.Office.Interop.Excel;
using Syncfusion.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Auditoria
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReportesController(AppDbContext context)
        {
            this._context = context;
        }

        //
        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {
                var model = _context.ProdAuditoriaFoto.Where(x=>x.IdProdAuditoria == IdProdAuditoria && x.extension.Equals("pdf")).ToList();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //
        [HttpGet("{idAgen}/{IdProdAuditoria}/{reporte}")]
        public ActionResult Get(int idAgen, int IdProdAuditoria, string reporte)
        {
            try
            {
                var dataAuditoria = _context.AuditoriaClass.FromSqlRaw($"sp_GetAuditoria " + idAgen + "," + IdProdAuditoria + "").ToList();
                if (reporte == "todo")
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        var puntosControlAF = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "AF"  
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion  
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        var puntosControlCB = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "CB"  
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion  
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        var puntosControlFV = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "FV"  
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion  
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        IApplication application = excelEngine.Excel;
                        application.DefaultVersion = ExcelVersion.Excel2016;

                        //Create WorkBook
                        IWorkbook workbook = application.Workbooks.Create(0);
                        IWorksheet politicas = workbook.Worksheets.Create("Declaración políticas");

                        //POLITICAS
                        //Disable gridLines
                        politicas.IsGridLinesVisible = false;

                        politicas.Range["A1:B1"].Merge();
                        politicas.Range["A1:B1"].Text = "DECLARACIÓN SOBRE POLÍTICAS DE INOCUIDAD ALIMENTARIA";
                        politicas.Range["A1:B1"].CellStyle.Font.Bold = true;
                        politicas.Range["A2:B2"].Merge();
                        politicas.Range["A2:B2"].Text = "Un productor podrá usar esta plantilla o cualquier otro formato para cumplir con AF 15.1";

                        politicas.Range["A3"].Text = "NOMBRE DE LA EMPRESA:";
                        politicas.Range["A4"].Text = "NOMBRE DEL ADMINISTRADOR/DUEÑO:";
                        politicas.Range["A5"].Text = "FECHA:";
                        politicas.Range["A6"].Text = "FIRMA:";
                        politicas.Range["A3:A6"].CellStyle.Font.Bold = true;

                        politicas.Range["A7:B7"].Merge();
                        politicas.Range["A7:B7"].Text = "Nos comprometemos a asegurar la implementación y el mantenimiento de la inocuidad alimentaria en todos nuestros procesos de producción." +
                            "Esto se logra de la siguiente manera:";

                        politicas.Range["A8:B8"].Merge();
                        politicas.Range["A8:B8"].Text = "1. CUMPLIMIENTO E IMPLEMENTACIÓN DE LA LEGISLACIÓN RELEVANTE";

                        politicas.Range["A9:B9"].Merge();
                        politicas.Range["A9:B9"].Text = "2. IMPLEMENTACIÓN DE LAS BUENAS PRÁCTICAS AGRÍCOLAS Y CERTIFICACIÓN BAJO LA NORMA GLOBALG.A.P. PARA ASEGURAMIENTO INTEGRADO DE FINCAS EN SU VERSIÓN ACTUAL";

                        politicas.Range["A10:B10"].Merge();
                        politicas.Range["A10:B10"].Text = "Todo nuestro personal recibió formación en temas de inocuidad alimentaria e higiene (véase AF 3). El personal es controlado estrictamente para asegurar de que se implementen las prácticas.";

                        politicas.Range["A11:B11"].Merge();
                        politicas.Range["A11:B11"].Text = "La(s) siguiente(s) persona(s) son responsables por la inocuidad alimentaria";
                        politicas.Range["A11:B11"].CellStyle.Font.Bold = true;

                        politicas.Range["A12"].Text = "DURANTE LA PRODUCCIÓN:";

                        politicas.Range["A13"].Text = "NOMBRE(S):";
                        politicas.Range["A13"].CellStyle.Font.Bold = true;

                        politicas.Range["A14"].Text = "DESIGNACIÓN:";
                        politicas.Range["A14"].CellStyle.Font.Bold = true;

                        politicas.Range["A15"].Text = "REEMPLAZO(S):";
                        politicas.Range["A15"].CellStyle.Font.Bold = true;

                        politicas.Range["A16:B16"].Merge();
                        politicas.Range["A16:B16"].Text = "En caso de ser otra la persona responsable durante la cosecha (cultivos) para asegurar que solo se cosechen productos inocuos de acuerdo a la norma:";

                        politicas.Range["A17"].Text = "NOMBRE(S):";
                        politicas.Range["A17"].CellStyle.Font.Bold = true;

                        politicas.Range["A18"].Text = "DESIGNACIÓN:";
                        politicas.Range["A18"].CellStyle.Font.Bold = true;

                        politicas.Range["A19"].Text = "REEMPLAZO(S):";
                        politicas.Range["A19"].CellStyle.Font.Bold = true;

                        politicas.Range["A20:B20"].Merge();
                        politicas.Range["A20:B20"].Text = "En caso de ser otra la persona responsable durante la manipulación del producto para asegurar que se cumplen los procedimientos de despacho de acuerdo a la norma:";

                        politicas.Range["A21"].Text = "NOMBRE(S):";
                        politicas.Range["A21"].CellStyle.Font.Bold = true;

                        politicas.Range["A22"].Text = "DESIGNACIÓN:";
                        politicas.Range["A22"].CellStyle.Font.Bold = true;

                        politicas.Range["A23"].Text = "REEMPLAZO(S):";
                        politicas.Range["A23"].CellStyle.Font.Bold = true;

                        politicas.Range["A24:B24"].Merge();
                        politicas.Range["A24:B24"].Text = "LA INFORMACIÓN DE CONTACTO LAS 24 HORAS EN EL EVENTO DE UNA EMERGENCIA CON RESPECTO A LA INOCUIDAD ALIMENTARIA ES LA SIGUIENTE:";

                        politicas.Range["A25"].Text = "TEL:";
                        politicas.Range["A25"].CellStyle.Font.Bold = true;

                        politicas.Range["A26:B26"].Merge();
                        politicas.Range["A26:B26"].Text = "La implementación de GLOBALG.A.P. se basa en la identificación de los riesgos y peligros. Se revisarán anualmente las actividades de mitigación de estos riesgos para asegurar que las mismas continúan siendo apropiadas, adecuadas y eficaces.";

                        politicas.Range["B3:B5"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B13:B15"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B17:B19"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B21:B23"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B25"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        //Notas de inspección
                        IWorksheet notas = workbook.Worksheets.Create("Notas de inspección");
                        notas.IsGridLinesVisible = false;

                        notas.Range["A1"].Text = "INDICACIONES PARA LAS INSPECCIONES (para toda inspección externa e interna)";
                        notas.Range["A1"].CellStyle.Font.Bold = true;
                        notas.Range["A2:E2"].Merge();
                        notas.Range["A2:E2"].Text = "1.  SE DEBEN INSPECCIONAR TODOS LOS PUNTOS DE CONTROL. POR DEFECTO TODOS SON APLICABLES SALVO QUE SE DECLARE LO CONTRARIO";
                        notas.Range["A3:E3"].Merge();
                        notas.Range["A3:E3"].Text = "2.  LOS PUNTOS DE CONTROL DEBERÁN JUSTIFICARSE PARA  ASEGURAR EL SEGUIMIENTO DE LOS PASOS DE LA AUDITORÍA.";
                        notas.Range["A4:E4"].Merge();
                        notas.Range["A4:E4"].Text = "3.  SIN EMBARGO, LA LISTA DE VERIFICACIÓN DE LA AUTOEVALUACIÓN (OPCIÓN 1) DEBERÁ CONTENER COMENTARIOS SOBRE LAS EVIDENCIAS OBSERVADAS CORRESPONDIENTES A TODOS LOS PUNTOS DE CONTROL NO CUMPLIDOS Y NO APLICABLES.";
                        notas.Range["A5:E5"].Merge();
                        notas.Range["A5:E5"].Text = "4.  PARA LAS INSPECCIONES INTERNAS (OPCIÓN 1 PRODUCTOR MULTISITIO Y OPCIÓN 2) Y LAS INSPECCIONES EXTERNAS, SE DEBERÁN APORTAR COMENTARIOS SOBRE TODAS LAS OBLIGACIONES MAYORES, TODOS LOS PUNTOS INCUMPLIDOS Y TODAS LAS OBLIGACIONES MENORES NO APLICABLES, SALVO QUE SE INDIQUE LO CONTRARIO EN LA GUÍA DE METODOLOGÍA DE INSPECCIÓN (CUANDO ESTÉ DISPONIBLE). LOS ORGANISMOS DE CERTIFICACIÓN DEBERÁN DOCUMENTAR LOS HALLAZGOS POSITIVOS CORRESPONDIENTES A LAS OBLIGACIONES MAYORES Y MENORES CUMPLIDAS, PARA PERMITIR EL SEGUIMIENTO DE LOS PASOS DE LA AUDITORÍA DESPUÉS DEL EVENTO.";
                        notas.Range["A6:E6"].Merge();
                        notas.Range["A6:E6"].Text = "Se han agregado los criterios de cumplimiento a la lista de verificación para tener la información completa y a modo de guía.";

                        notas.Range["A7"].Text = "Por favor, elija";
                        notas.Range["A7"].CellStyle.Font.Bold = true;

                        notas.Range["A8"].Text = "OPCIÓN 1";
                        notas.Range["A9"].Text = "OPCIÓN 1 PRODUCTOR MULTISITIO SIN SGC";
                        notas.Range["A10"].Text = "OPCIÓN 1 PRODUCTOR MULTISITIO CON SGC";
                        notas.Range["A11"].Text = "OPCIÓN 2";

                        notas.Range["B8:B11"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A12"].Text = "Tipo de inspección";
                        notas.Range["A12"].CellStyle.Font.Bold = true;

                        notas.Range["A13"].Text = "ANUNCIADA";
                        notas.Range["A14"].Text = "NO ANUNCIADA";
                        notas.Range["A15"].Text = "OTRO";

                        notas.Range["B13:B15"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["B16"].Text = "Si";
                        notas.Range["C16"].Text = "No";
                        notas.Range["D16"].Text = "N/A";
                        notas.Range["B16:D16"].CellStyle.Font.Bold = true;

                        notas.Range["B17:D17"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A17"].Text = "¿PARTICIPA EL PRODUCTOR EN EL PROGRAMA DE RECOMPENSAS NO-ANUNCIADAS?";
                        notas.Range["A18"].Text = "¿RECURRE EL PRODUCTOR A UN ASESOR?";
                        notas.Range["A19"].Text = "SI LA RESPUESTA ES SÍ, ¿ES EL ASESOR UN GLOBALG.A.P. LICENSED FARM ASSURER?";

                        notas.Range["B18:C19"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A20"].Text = "SI LA RESPUESTA ES SÍ, ¿CUÁL ES EL NOMBRE DEL FARM ASSURER?";
                        notas.Range["B20:E20"].Merge();
                        notas.Range["B20:E20"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A21"].Text = "¿ESTÁ EL PRODUCTOR REGISTRADO PARA LA PRODUCCIÓN PARALELA?";

                        notas.Range["B21:C21"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A22"].Text = "SI LA RESPUESTA ES SÍ, ¿PARA QUÉ PRODUCTOS?";
                        notas.Range["B22:E22"].Merge();
                        notas.Range["B22:E22"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A23"].Text = "¿ESTÁ EL PRODUCTOR REGISTRADO PARA LA PROPIEDAD PARALELA?";
                        notas.Range["B23:C23"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A24"].Text = "SI LA RESPUESTA ES SÍ, ¿PARA QUÉ PRODUCTOS?";
                        notas.Range["B24:E24"].Merge();
                        notas.Range["B24:E24"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A25"].Text = "¿EL PRODUCTOR COMPRA PRODUCTOS CERTIFICADOS DE FUENTES EXTERNAS?";
                        notas.Range["B25:C25"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A26"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B26:E26"].Merge();
                        notas.Range["B26:E26"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A27"].Text = "¿SE REALIZA LA INSPECCIÓN EN COMBINACIÓN CON ALGUNA OTRA NORMA O ADD-ON?";
                        notas.Range["B27:C27"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A28"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ NORMA O ADD-ON?";
                        notas.Range["B28:E28"].Merge();
                        notas.Range["B28:E28"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A29"].Text = "¿SE OBSERVÓ LA COSECHA DEL PRODUCTO DURANTE LA INSPECCIÓN?";
                        notas.Range["B29:C29"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A30"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B30:E30"].Merge();
                        notas.Range["B30:E30"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A31"].Text = "¿SE OBSERVÓ LA MANIPULACIÓN DEL PRODUCTO DURANTE LA INSPECCIÓN?";
                        notas.Range["B31:C31"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A32"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B32:E32"].Merge();
                        notas.Range["B32:E32"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A33"].Text = "LISTA DE TODOS LOS PRODUCTOS PRESENTADOS DURANTE LA INSPECCIÓN:";
                        notas.Range["B33:E33"].Merge();
                        notas.Range["B33:E33"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A34"].Text = "LUGAR(ES) VISITADOS:";
                        notas.Range["B34:E34"].Merge();
                        notas.Range["B34:E34"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A35"].Text = "DURACIÓN DE LA INSPECCIÓN:";
                        notas.Range["B35:E35"].Merge();
                        notas.Range["B35:E35"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A36"].Text = "CÁLCULO DEL 95 % DE CUMPLIMIENTO DE LAS OBLIGACIONES MENORES:";
                        notas.Range["B36:E36"].Merge();
                        notas.Range["B36:E36"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A38"].Text = "NOMBRE DEL PRODUCTOR:";
                        notas.Range["B38:E38"].Merge();
                        notas.Range["B38:E38"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A39"].Text = "FECHA:";
                        notas.Range["B39:E39"].Merge();
                        notas.Range["B39:E39"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A40"].Text = "FIRMA:";
                        notas.Range["B40:E40"].Merge();
                        notas.Range["B40:E40"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A42"].Text = "NOMBRE DEL INSPECTOR/AUDITOR:";
                        notas.Range["B42:E42"].Merge();
                        notas.Range["B42:E42"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A43"].Text = "FECHA:";
                        notas.Range["B43:E43"].Merge();
                        notas.Range["B43:E43"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A44"].Text = "FIRMA:";
                        notas.Range["B44:E44"].Merge();
                        notas.Range["B44:E44"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A38:A44"].CellStyle.Font.Bold = true;

                        notas.Range["A46:D46"].Merge();
                        notas.Range["A46:D46"].Text = "CON EL FIN DE MEJORAR NUESTROS SERVICIOS Y DAR LA " +
                            "OPORTUNIDAD DE APORTAR COMENTARIOS SOBRE EL ORGANISMO DE " +
                            "CERTIFICACIÓN, AUDITOR O INSPECTOR GLOBALG.A.P., LO INVITAMOS A " +
                            "PARTICIPAR EN UNA ENCUESTA DIRIGIDA A LOS PRODUCTORES.  " +
                            "INGRESE LA DIRECCIÓN www.globalgap.org/feedback EN SU BUSCADOR " +
                            "DE INTERNET O ESCANEE EL SIGUIENTE CÓDIGO QR.";
                        notas.Range["A46:D46"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignDistributed;

                        //PUNTOS CONTROL
                        IWorksheet worksheet = workbook.Worksheets.Create("AF_CB_FV");
                        //Disable gridLines
                        worksheet.IsGridLinesVisible = false;

                        //Header
                        worksheet.Range["C1"].Text = "Fecha:";
                        worksheet.Range["B3"].Text = "USUARIO:";
                        worksheet.Range["B4"].Text = "COD PRODUCTOR:";
                        worksheet.Range["B5"].Text = "NOM PRODUCTOR:";
                        worksheet.Range["B6"].Text = "COD CAMPO(S):";
                        worksheet.Range["B7"].Text = "NOMBRE CAMPO(S):";
                        worksheet.Range["B8"].Text = "ZONA:";

                        foreach (var x in dataAuditoria)
                        {
                            worksheet.Range["C3"].Text = x.Asesor;
                            worksheet.Range["C4"].Text = x.Cod_Prod;
                            worksheet.Range["C5"].Text = x.Productor;
                            worksheet.Range["C6"].Text = x.Cod_Campo;
                            worksheet.Range["C7"].Text = x.Campo;
                            worksheet.Range["C8"].Text = x.Zona;
                            worksheet.Range["D1"].Value = x.Fecha.ToString();
                        }

                        worksheet.Range["B1:C8"].CellStyle.Font.Bold = true;

                        //Table
                        worksheet.Range["A11"].Text = "N°";
                        worksheet.Range["B11"].Text = "Puntos de Control";
                        worksheet.Range["C11"].Text = "Criterios de Cumplimiento";
                        worksheet.Range["D11"].Text = "Nivel";
                        worksheet.Range["E11"].Text = "Si";
                        worksheet.Range["F11"].Text = "No";
                        worksheet.Range["G11"].Text = "N/A";
                        worksheet.Range["H11"].Text = "Justificacion";
                        worksheet.Range["A11:H11"].CellStyle.Font.Bold = true;
                        worksheet.Range["A11:H11"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        worksheet.Range["A11:H11"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                        //AF
                        worksheet.Range["A12"].Text = "AF";
                        worksheet.Range["B12:C12"].Merge();
                        worksheet.Range["B12"].Text = "MÓDULO BASE PARA TODO TIPO DE FINCA";
                        worksheet.Range["A12:C12"].CellStyle.Font.Bold = true;

                        worksheet.Range["B13:C13"].Merge();
                        worksheet.Range["B13"].Text = "Los puntos de control de este módulo son aplicables a todos los productores que solicitan la certificación, ya que cubren aspectos relevantes a toda actividad agropecuaria";
                        worksheet.Range["B13:C13"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        //worksheet.Range["A14"].Text = "AF 1";
                        //worksheet.Range["B14:C14"].Merge();
                        //worksheet.Range["B14"].Text = "HISTORIAL Y MANEJO DEL SITIO";
                        //worksheet.Range["A14:C14"].CellStyle.Font.Bold = true;

                        //worksheet.Range["B15"].Text = "Una de las características clave de la producción agropecuaria sostenible es que continuamente integra los conocimientos específicos al sitio y la experiencia práctica en la planificación del manejo y las prácticas para el futuro. El objetivo de esta sección es asegurar que el campo, los edificios y las otras instalaciones que constituyen el esqueleto de la granja, se gestionen adecuadamente con el fin de garantizar la producción segura de alimentos y la protección del medio ambiente.";
                        //worksheet.Range["B15"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                        //worksheet.Range["A16"].Text = "AF 1.1";
                        //worksheet.Range["B16:C16"].Merge();
                        //worksheet.Range["B16"].Text = "Historial del Sitio";
                        //worksheet.Range["A16:C16"].CellStyle.Font.Bold = true;

                        int currRowCount = 14;
                        foreach (var x in puntosControlAF)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            if (x.Opcion == "SI")
                            {
                                worksheet.Range["E" + currRowCount].Text = "X";
                                worksheet.Range["E" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["E" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NO")
                            {
                                worksheet.Range["F" + currRowCount].Text = "X";
                                worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NA")
                            {
                                worksheet.Range["G" + currRowCount].Text = "X";
                                worksheet.Range["G" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["G" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                          
                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //CB
                        worksheet.Range["A" + currRowCount].Text = "CB";
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Merge();
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Text = "MÓDULO BASE PARA CULTIVOS";
                        worksheet.Range["A" + currRowCount + ":C" + currRowCount].CellStyle.Font.Bold = true;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        currRowCount = currRowCount + 1;
                        foreach (var x in puntosControlCB)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            if (x.Opcion == "SI")
                            {
                                worksheet.Range["E" + currRowCount].Text = "X";
                                worksheet.Range["E" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["E" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NO")
                            {
                                worksheet.Range["F" + currRowCount].Text = "X";
                                worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NA")
                            {
                                worksheet.Range["G" + currRowCount].Text = "X";
                                worksheet.Range["G" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["G" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }


                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //FV
                        worksheet.Range["A" + currRowCount].Text = "FV";
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Merge();
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Text = "FRUTAS Y HORTALIZAS";
                        worksheet.Range["A" + currRowCount + ":C" + currRowCount].CellStyle.Font.Bold = true;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        currRowCount = currRowCount + 1;
                        foreach (var x in puntosControlFV)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            if (x.Opcion == "SI")
                            {
                                worksheet.Range["E" + currRowCount].Text = "X";
                                worksheet.Range["E" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["E" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NO")
                            {
                                worksheet.Range["F" + currRowCount].Text = "X";
                                worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }
                            else if (x.Opcion == "NA")
                            {
                                worksheet.Range["G" + currRowCount].Text = "X";
                                worksheet.Range["G" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                worksheet.Range["G" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;
                            }

                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //Save the Excel workbook to MemoryStream
                        MemoryStream stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        Response.ContentType = new MediaTypeHeaderValue("application/octet-stream").ToString();// Content type
                        return new FileStreamResult(stream, "application/excel") { FileDownloadName = "Reporte de Acciones Correctivas " + IdProdAuditoria + ".xlsx" };
                    }
                }

                else if (reporte == "excel")
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        var puntosControlAF = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "AF"  && l.Opcion == "NO" 
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion //c.Justificacion + ' ' + l.Justificacion
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        var puntosControlCB = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "CB" && l.Opcion == "NO" 
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion //c.Justificacion + ' ' + l.Justificacion
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        var puntosControlFV = (from l in _context.ProdLogAuditoria
                                               join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                               join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                               join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                               where l.IdProdAuditoria == IdProdAuditoria && c.NoPunto == "FV" && l.Opcion == "NO" 
                                               group a by new
                                               {
                                                   IdLog = l.Id,
                                                   NoPunto = c.NoPunto,
                                                   NoPuntoDesc = c.NoPuntoDesc,
                                                   PuntoControl = c.PuntoControl,
                                                   PuntoControlDesc = c.PuntoControlDesc,
                                                   Criterio = c.Criterio,
                                                   Nivel = c.Nivel,
                                                   Opcion = l.Opcion,
                                                   Justificacion = l.Justificacion //c.Justificacion + ' ' + l.Justificacion
                                               } into x
                                               select new
                                               {
                                                   IdLog = x.Key.IdLog,
                                                   NoPunto = x.Key.NoPunto,
                                                   NoPuntoDesc = x.Key.NoPuntoDesc,
                                                   PuntoControl = x.Key.PuntoControl,
                                                   PuntoControlDesc = x.Key.PuntoControlDesc,
                                                   Criterio = x.Key.Criterio,
                                                   Nivel = x.Key.Nivel,
                                                   Opcion = x.Key.Opcion,
                                                   Justificacion = x.Key.Justificacion
                                               }).Distinct();

                        IApplication application = excelEngine.Excel;
                        application.DefaultVersion = ExcelVersion.Excel2016;

                        //Create WorkBook
                        IWorkbook workbook = application.Workbooks.Create(0);
                        IWorksheet politicas = workbook.Worksheets.Create("Declaración políticas");

                        //POLITICAS
                        //Disable gridLines
                        politicas.IsGridLinesVisible = false;

                        politicas.Range["A1:B1"].Merge();
                        politicas.Range["A1:B1"].Text = "DECLARACIÓN SOBRE POLÍTICAS DE INOCUIDAD ALIMENTARIA";
                        politicas.Range["A1:B1"].CellStyle.Font.Bold = true;
                        politicas.Range["A2:B2"].Merge();
                        politicas.Range["A2:B2"].Text = "Un productor podrá usar esta plantilla o cualquier otro formato para cumplir con AF 15.1";

                        politicas.Range["A3"].Text = "NOMBRE DE LA EMPRESA:";
                        politicas.Range["A4"].Text = "NOMBRE DEL ADMINISTRADOR/DUEÑO:";
                        politicas.Range["A5"].Text = "FECHA:";
                        politicas.Range["A6"].Text = "FIRMA:";
                        politicas.Range["A3:A6"].CellStyle.Font.Bold = true;

                        politicas.Range["A7:B7"].Merge();
                        politicas.Range["A7:B7"].Text = "Nos comprometemos a asegurar la implementación y el mantenimiento de la inocuidad alimentaria en todos nuestros procesos de producción." +
                            "Esto se logra de la siguiente manera:";

                        politicas.Range["A8:B8"].Merge();
                        politicas.Range["A8:B8"].Text = "1. CUMPLIMIENTO E IMPLEMENTACIÓN DE LA LEGISLACIÓN RELEVANTE";

                        politicas.Range["A9:B9"].Merge();
                        politicas.Range["A9:B9"].Text = "2. IMPLEMENTACIÓN DE LAS BUENAS PRÁCTICAS AGRÍCOLAS Y CERTIFICACIÓN BAJO LA NORMA GLOBALG.A.P. PARA ASEGURAMIENTO INTEGRADO DE FINCAS EN SU VERSIÓN ACTUAL";

                        politicas.Range["A10:B10"].Merge();
                        politicas.Range["A10:B10"].Text = "Todo nuestro personal recibió formación en temas de inocuidad alimentaria e higiene (véase AF 3). El personal es controlado estrictamente para asegurar de que se implementen las prácticas.";

                        politicas.Range["A11:B11"].Merge();
                        politicas.Range["A11:B11"].Text = "La(s) siguiente(s) persona(s) son responsables por la inocuidad alimentaria";
                        politicas.Range["A11:B11"].CellStyle.Font.Bold = true;

                        politicas.Range["A12"].Text = "DURANTE LA PRODUCCIÓN:";

                        politicas.Range["A13"].Text = "NOMBRE(S):";
                        politicas.Range["A13"].CellStyle.Font.Bold = true;

                        politicas.Range["A14"].Text = "DESIGNACIÓN:";
                        politicas.Range["A14"].CellStyle.Font.Bold = true;

                        politicas.Range["A15"].Text = "REEMPLAZO(S):";
                        politicas.Range["A15"].CellStyle.Font.Bold = true;

                        politicas.Range["A16:B16"].Merge();
                        politicas.Range["A16:B16"].Text = "En caso de ser otra la persona responsable durante la cosecha (cultivos) para asegurar que solo se cosechen productos inocuos de acuerdo a la norma:";

                        politicas.Range["A17"].Text = "NOMBRE(S):";
                        politicas.Range["A17"].CellStyle.Font.Bold = true;

                        politicas.Range["A18"].Text = "DESIGNACIÓN:";
                        politicas.Range["A18"].CellStyle.Font.Bold = true;

                        politicas.Range["A19"].Text = "REEMPLAZO(S):";
                        politicas.Range["A19"].CellStyle.Font.Bold = true;

                        politicas.Range["A20:B20"].Merge();
                        politicas.Range["A20:B20"].Text = "En caso de ser otra la persona responsable durante la manipulación del producto para asegurar que se cumplen los procedimientos de despacho de acuerdo a la norma:";

                        politicas.Range["A21"].Text = "NOMBRE(S):";
                        politicas.Range["A21"].CellStyle.Font.Bold = true;

                        politicas.Range["A22"].Text = "DESIGNACIÓN:";
                        politicas.Range["A22"].CellStyle.Font.Bold = true;

                        politicas.Range["A23"].Text = "REEMPLAZO(S):";
                        politicas.Range["A23"].CellStyle.Font.Bold = true;

                        politicas.Range["A24:B24"].Merge();
                        politicas.Range["A24:B24"].Text = "LA INFORMACIÓN DE CONTACTO LAS 24 HORAS EN EL EVENTO DE UNA EMERGENCIA CON RESPECTO A LA INOCUIDAD ALIMENTARIA ES LA SIGUIENTE:";

                        politicas.Range["A25"].Text = "TEL:";
                        politicas.Range["A25"].CellStyle.Font.Bold = true;

                        politicas.Range["A26:B26"].Merge();
                        politicas.Range["A26:B26"].Text = "La implementación de GLOBALG.A.P. se basa en la identificación de los riesgos y peligros. Se revisarán anualmente las actividades de mitigación de estos riesgos para asegurar que las mismas continúan siendo apropiadas, adecuadas y eficaces.";

                        politicas.Range["B3:B5"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B3:B5"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B13:B15"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B17:B19"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B17:B19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B21:B23"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        politicas.Range["B21:B23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        politicas.Range["B25"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        //Notas de inspección
                        IWorksheet notas = workbook.Worksheets.Create("Notas de inspección");
                        notas.IsGridLinesVisible = false;

                        notas.Range["A1"].Text = "INDICACIONES PARA LAS INSPECCIONES (para toda inspección externa e interna)";
                        notas.Range["A1"].CellStyle.Font.Bold = true;
                        notas.Range["A2:E2"].Merge();
                        notas.Range["A2:E2"].Text = "1.  SE DEBEN INSPECCIONAR TODOS LOS PUNTOS DE CONTROL. POR DEFECTO TODOS SON APLICABLES SALVO QUE SE DECLARE LO CONTRARIO";
                        notas.Range["A3:E3"].Merge();
                        notas.Range["A3:E3"].Text = "2.  LOS PUNTOS DE CONTROL DEBERÁN JUSTIFICARSE PARA  ASEGURAR EL SEGUIMIENTO DE LOS PASOS DE LA AUDITORÍA.";
                        notas.Range["A4:E4"].Merge();
                        notas.Range["A4:E4"].Text = "3.  SIN EMBARGO, LA LISTA DE VERIFICACIÓN DE LA AUTOEVALUACIÓN (OPCIÓN 1) DEBERÁ CONTENER COMENTARIOS SOBRE LAS EVIDENCIAS OBSERVADAS CORRESPONDIENTES A TODOS LOS PUNTOS DE CONTROL NO CUMPLIDOS Y NO APLICABLES.";
                        notas.Range["A5:E5"].Merge();
                        notas.Range["A5:E5"].Text = "4.  PARA LAS INSPECCIONES INTERNAS (OPCIÓN 1 PRODUCTOR MULTISITIO Y OPCIÓN 2) Y LAS INSPECCIONES EXTERNAS, SE DEBERÁN APORTAR COMENTARIOS SOBRE TODAS LAS OBLIGACIONES MAYORES, TODOS LOS PUNTOS INCUMPLIDOS Y TODAS LAS OBLIGACIONES MENORES NO APLICABLES, SALVO QUE SE INDIQUE LO CONTRARIO EN LA GUÍA DE METODOLOGÍA DE INSPECCIÓN (CUANDO ESTÉ DISPONIBLE). LOS ORGANISMOS DE CERTIFICACIÓN DEBERÁN DOCUMENTAR LOS HALLAZGOS POSITIVOS CORRESPONDIENTES A LAS OBLIGACIONES MAYORES Y MENORES CUMPLIDAS, PARA PERMITIR EL SEGUIMIENTO DE LOS PASOS DE LA AUDITORÍA DESPUÉS DEL EVENTO.";
                        notas.Range["A6:E6"].Merge();
                        notas.Range["A6:E6"].Text = "Se han agregado los criterios de cumplimiento a la lista de verificación para tener la información completa y a modo de guía.";

                        notas.Range["A7"].Text = "Por favor, elija";
                        notas.Range["A7"].CellStyle.Font.Bold = true;

                        notas.Range["A8"].Text = "OPCIÓN 1";
                        notas.Range["A9"].Text = "OPCIÓN 1 PRODUCTOR MULTISITIO SIN SGC";
                        notas.Range["A10"].Text = "OPCIÓN 1 PRODUCTOR MULTISITIO CON SGC";
                        notas.Range["A11"].Text = "OPCIÓN 2";

                        notas.Range["B8:B11"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B8:B11"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A12"].Text = "Tipo de inspección";
                        notas.Range["A12"].CellStyle.Font.Bold = true;

                        notas.Range["A13"].Text = "ANUNCIADA";
                        notas.Range["A14"].Text = "NO ANUNCIADA";
                        notas.Range["A15"].Text = "OTRO";

                        notas.Range["B13:B15"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B13:B15"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["B16"].Text = "Si";
                        notas.Range["C16"].Text = "No";
                        notas.Range["D16"].Text = "N/A";
                        notas.Range["B16:D16"].CellStyle.Font.Bold = true;

                        notas.Range["B17:D17"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B17:D17"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A17"].Text = "¿PARTICIPA EL PRODUCTOR EN EL PROGRAMA DE RECOMPENSAS NO-ANUNCIADAS?";
                        notas.Range["A18"].Text = "¿RECURRE EL PRODUCTOR A UN ASESOR?";
                        notas.Range["A19"].Text = "SI LA RESPUESTA ES SÍ, ¿ES EL ASESOR UN GLOBALG.A.P. LICENSED FARM ASSURER?";

                        notas.Range["B18:C19"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B18:C19"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A20"].Text = "SI LA RESPUESTA ES SÍ, ¿CUÁL ES EL NOMBRE DEL FARM ASSURER?";
                        notas.Range["B20:E20"].Merge();
                        notas.Range["B20:E20"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A21"].Text = "¿ESTÁ EL PRODUCTOR REGISTRADO PARA LA PRODUCCIÓN PARALELA?";

                        notas.Range["B21:C21"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B21:C21"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A22"].Text = "SI LA RESPUESTA ES SÍ, ¿PARA QUÉ PRODUCTOS?";
                        notas.Range["B22:E22"].Merge();
                        notas.Range["B22:E22"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A23"].Text = "¿ESTÁ EL PRODUCTOR REGISTRADO PARA LA PROPIEDAD PARALELA?";
                        notas.Range["B23:C23"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B23:C23"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A24"].Text = "SI LA RESPUESTA ES SÍ, ¿PARA QUÉ PRODUCTOS?";
                        notas.Range["B24:E24"].Merge();
                        notas.Range["B24:E24"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A25"].Text = "¿EL PRODUCTOR COMPRA PRODUCTOS CERTIFICADOS DE FUENTES EXTERNAS?";
                        notas.Range["B25:C25"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B25:C25"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A26"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B26:E26"].Merge();
                        notas.Range["B26:E26"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A27"].Text = "¿SE REALIZA LA INSPECCIÓN EN COMBINACIÓN CON ALGUNA OTRA NORMA O ADD-ON?";
                        notas.Range["B27:C27"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B27:C27"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A28"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ NORMA O ADD-ON?";
                        notas.Range["B28:E28"].Merge();
                        notas.Range["B28:E28"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A29"].Text = "¿SE OBSERVÓ LA COSECHA DEL PRODUCTO DURANTE LA INSPECCIÓN?";
                        notas.Range["B29:C29"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B29:C29"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A30"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B30:E30"].Merge();
                        notas.Range["B30:E30"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;

                        notas.Range["A31"].Text = "¿SE OBSERVÓ LA MANIPULACIÓN DEL PRODUCTO DURANTE LA INSPECCIÓN?";
                        notas.Range["B31:C31"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;
                        notas.Range["B31:C31"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.White;

                        notas.Range["A32"].Text = "SI LA RESPUESTA ES SÍ, ¿QUÉ PRODUCTOS?";
                        notas.Range["B32:E32"].Merge();
                        notas.Range["B32:E32"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B32:E32"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A33"].Text = "LISTA DE TODOS LOS PRODUCTOS PRESENTADOS DURANTE LA INSPECCIÓN:";
                        notas.Range["B33:E33"].Merge();
                        notas.Range["B33:E33"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B33:E33"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A34"].Text = "LUGAR(ES) VISITADOS:";
                        notas.Range["B34:E34"].Merge();
                        notas.Range["B34:E34"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B34:E34"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A35"].Text = "DURACIÓN DE LA INSPECCIÓN:";
                        notas.Range["B35:E35"].Merge();
                        notas.Range["B35:E35"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B35:E35"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A36"].Text = "CÁLCULO DEL 95 % DE CUMPLIMIENTO DE LAS OBLIGACIONES MENORES:";
                        notas.Range["B36:E36"].Merge();
                        notas.Range["B36:E36"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B36:E36"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A38"].Text = "NOMBRE DEL PRODUCTOR:";
                        notas.Range["B38:E38"].Merge();
                        notas.Range["B38:E38"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B38:E38"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A39"].Text = "FECHA:";
                        notas.Range["B39:E39"].Merge();
                        notas.Range["B39:E39"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B39:E39"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A40"].Text = "FIRMA:";
                        notas.Range["B40:E40"].Merge();
                        notas.Range["B40:E40"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B40:E40"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A42"].Text = "NOMBRE DEL INSPECTOR/AUDITOR:";
                        notas.Range["B42:E42"].Merge();
                        notas.Range["B42:E42"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B42:E42"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A43"].Text = "FECHA:";
                        notas.Range["B43:E43"].Merge();
                        notas.Range["B43:E43"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B43:E43"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A44"].Text = "FIRMA:";
                        notas.Range["B44:E44"].Merge();
                        notas.Range["B44:E44"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.White;
                        notas.Range["B44:E44"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.White;

                        notas.Range["A38:A44"].CellStyle.Font.Bold = true;

                        notas.Range["A46:D46"].Merge();
                        notas.Range["A46:D46"].Text = "CON EL FIN DE MEJORAR NUESTROS SERVICIOS Y DAR LA " +
                            "OPORTUNIDAD DE APORTAR COMENTARIOS SOBRE EL ORGANISMO DE " +
                            "CERTIFICACIÓN, AUDITOR O INSPECTOR GLOBALG.A.P., LO INVITAMOS A " +
                            "PARTICIPAR EN UNA ENCUESTA DIRIGIDA A LOS PRODUCTORES.  " +
                            "INGRESE LA DIRECCIÓN www.globalgap.org/feedback EN SU BUSCADOR " +
                            "DE INTERNET O ESCANEE EL SIGUIENTE CÓDIGO QR.";
                        notas.Range["A46:D46"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignDistributed;

                        //PUNTOS CONTROL
                        IWorksheet worksheet = workbook.Worksheets.Create("AF_CB_FV");
                        //Disable gridLines
                        worksheet.IsGridLinesVisible = false;

                        //Header
                        worksheet.Range["C1"].Text = "Fecha:";
                        worksheet.Range["B3"].Text = "USUARIO:";
                        worksheet.Range["B4"].Text = "COD PRODUCTOR:";
                        worksheet.Range["B5"].Text = "NOM PRODUCTOR:";
                        worksheet.Range["B6"].Text = "COD CAMPO(S):";
                        worksheet.Range["B7"].Text = "NOMBRE CAMPO(S):";
                        worksheet.Range["B8"].Text = "ZONA:";

                        foreach (var x in dataAuditoria)
                        {
                            worksheet.Range["C3"].Text = x.Asesor;
                            worksheet.Range["C4"].Text = x.Cod_Prod;
                            worksheet.Range["C5"].Text = x.Productor;
                            worksheet.Range["C6"].Text = x.Cod_Campo;
                            worksheet.Range["C7"].Text = x.Campo;
                            worksheet.Range["C8"].Text = x.Zona;
                            worksheet.Range["D1"].Value = x.Fecha.ToString();
                        }

                        worksheet.Range["B1:C8"].CellStyle.Font.Bold = true;

                        //Table
                        worksheet.Range["A11"].Text = "N°";
                        worksheet.Range["B11"].Text = "Puntos de Control";
                        worksheet.Range["C11"].Text = "Criterios de Cumplimiento";
                        worksheet.Range["D11"].Text = "Nivel";
                        worksheet.Range["E11"].Text = "Si";
                        worksheet.Range["F11"].Text = "No";
                        worksheet.Range["G11"].Text = "N/A";
                        worksheet.Range["H11"].Text = "Justificacion";
                        worksheet.Range["A11:H11"].CellStyle.Font.Bold = true;
                        worksheet.Range["A11:H11"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                        worksheet.Range["A11:H11"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                        //AF
                        worksheet.Range["A12"].Text = "AF";
                        worksheet.Range["B12:C12"].Merge();
                        worksheet.Range["B12"].Text = "MÓDULO BASE PARA TODO TIPO DE FINCA";
                        worksheet.Range["A12:C12"].CellStyle.Font.Bold = true;

                        worksheet.Range["B13:C13"].Merge();
                        worksheet.Range["B13"].Text = "Los puntos de control de este módulo son aplicables a todos los productores que solicitan la certificación, ya que cubren aspectos relevantes a toda actividad agropecuaria";
                        worksheet.Range["B13:C13"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                        //worksheet.Range["A14"].Text = "AF 1";
                        //worksheet.Range["B14:C14"].Merge();
                        //worksheet.Range["B14"].Text = "HISTORIAL Y MANEJO DEL SITIO";
                        //worksheet.Range["A14:C14"].CellStyle.Font.Bold = true;

                        //worksheet.Range["B15"].Text = "Una de las características clave de la producción agropecuaria sostenible es que continuamente integra los conocimientos específicos al sitio y la experiencia práctica en la planificación del manejo y las prácticas para el futuro. El objetivo de esta sección es asegurar que el campo, los edificios y las otras instalaciones que constituyen el esqueleto de la granja, se gestionen adecuadamente con el fin de garantizar la producción segura de alimentos y la protección del medio ambiente.";
                        //worksheet.Range["B15"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                        //worksheet.Range["A16"].Text = "AF 1.1";
                        //worksheet.Range["B16:C16"].Merge();
                        //worksheet.Range["B16"].Text = "Historial del Sitio";
                        //worksheet.Range["A16:C16"].CellStyle.Font.Bold = true;

                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        int currRowCount = 14;
                        foreach (var x in puntosControlAF)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["F" + currRowCount].Text = "X";
                            worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //CB
                        worksheet.Range["A" + currRowCount].Text = "CB";
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Merge();
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Text = "MÓDULO BASE PARA CULTIVOS";
                        worksheet.Range["A" + currRowCount + ":C" + currRowCount].CellStyle.Font.Bold = true;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        currRowCount = currRowCount + 1;
                        foreach (var x in puntosControlCB)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["F" + currRowCount].Text = "X";
                            worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //FV
                        worksheet.Range["A" + currRowCount].Text = "FV";
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Merge();
                        worksheet.Range["B" + currRowCount + ":C" + currRowCount].Text = "FRUTAS Y HORTALIZAS";
                        worksheet.Range["A" + currRowCount + ":C" + currRowCount].CellStyle.Font.Bold = true;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                        worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                        currRowCount = currRowCount + 1;
                        foreach (var x in puntosControlFV)
                        {
                            worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
                            worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
                            worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["C" + currRowCount].Text = x.Criterio;
                            worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["D" + currRowCount].Text = x.Nivel;
                            worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["F" + currRowCount].Text = "X";
                            worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

                            worksheet.Range["H" + currRowCount].Text = x.Justificacion;
                            worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
                            worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

                            currRowCount++;
                        }

                        //Save the Excel workbook to MemoryStream
                        MemoryStream stream = new MemoryStream();
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        Response.ContentType = new MediaTypeHeaderValue("application/octet-stream").ToString();// Content type
                        return new FileStreamResult(stream, "application/excel") { FileDownloadName = "Reporte de Acciones Correctivas " + IdProdAuditoria + ".xlsx" };
                    }
                }
                
                else
                {
                    var puntosControlAF = (from l in _context.ProdLogAuditoria
                                           join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                           join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                           join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                           where l.IdProdAuditoria == IdProdAuditoria && l.Opcion == "NO" && c.NoPunto == "AF"
                                           group a by new
                                           {
                                               IdLog = l.Id,
                                               NoPunto = c.NoPunto,
                                               NoPuntoDesc = c.NoPuntoDesc,
                                               PuntoControl = c.PuntoControl,
                                               PuntoControlDesc = c.PuntoControlDesc,
                                               Criterio = c.Criterio,
                                               Nivel = c.Nivel,
                                               Opcion = l.Opcion,
                                               Justificacion = l.Justificacion 
                                           } into x
                                           select new
                                           {
                                               IdLog = x.Key.IdLog,
                                               NoPunto = x.Key.NoPunto,
                                               NoPuntoDesc = x.Key.NoPuntoDesc,
                                               PuntoControl = x.Key.PuntoControl,
                                               PuntoControlDesc = x.Key.PuntoControlDesc,
                                               Criterio = x.Key.Criterio,
                                               Nivel = x.Key.Nivel,
                                               Opcion = x.Key.Opcion,
                                               Justificacion = x.Key.Justificacion
                                           }).Distinct();

                    var puntosControlCB = (from l in _context.ProdLogAuditoria
                                           join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                           join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                           join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                           where l.IdProdAuditoria == IdProdAuditoria && l.Opcion == "NO" && c.NoPunto == "CB"
                                           group a by new
                                           {
                                               IdLog = l.Id,
                                               NoPunto = c.NoPunto,
                                               NoPuntoDesc = c.NoPuntoDesc,
                                               PuntoControl = c.PuntoControl,
                                               PuntoControlDesc = c.PuntoControlDesc,
                                               Criterio = c.Criterio,
                                               Nivel = c.Nivel,
                                               Opcion = l.Opcion,
                                               Justificacion = l.Justificacion //c.Justificacion + ' ' + l.Justificacion
                                           } into x
                                           select new
                                           {
                                               IdLog = x.Key.IdLog,
                                               NoPunto = x.Key.NoPunto,
                                               NoPuntoDesc = x.Key.NoPuntoDesc,
                                               PuntoControl = x.Key.PuntoControl,
                                               PuntoControlDesc = x.Key.PuntoControlDesc,
                                               Criterio = x.Key.Criterio,
                                               Nivel = x.Key.Nivel,
                                               Opcion = x.Key.Opcion,
                                               Justificacion = x.Key.Justificacion
                                           }).Distinct();

                    var puntosControlFV = (from l in _context.ProdLogAuditoria
                                           join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                           join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                           join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                           where l.IdProdAuditoria == IdProdAuditoria && l.Opcion == "NO" && c.NoPunto == "FV"
                                           group a by new
                                           {
                                               IdLog = l.Id,
                                               NoPunto = c.NoPunto,
                                               NoPuntoDesc = c.NoPuntoDesc,
                                               PuntoControl = c.PuntoControl,
                                               PuntoControlDesc = c.PuntoControlDesc,
                                               Criterio = c.Criterio,
                                               Nivel = c.Nivel,
                                               Opcion = l.Opcion,
                                               Justificacion = l.Justificacion //c.Justificacion + ' ' + l.Justificacion
                                           } into x
                                           select new
                                           {
                                               IdLog = x.Key.IdLog,
                                               NoPunto = x.Key.NoPunto,
                                               NoPuntoDesc = x.Key.NoPuntoDesc,
                                               PuntoControl = x.Key.PuntoControl,
                                               PuntoControlDesc = x.Key.PuntoControlDesc,
                                               Criterio = x.Key.Criterio,
                                               Nivel = x.Key.Nivel,
                                               Opcion = x.Key.Opcion,
                                               Justificacion = x.Key.Justificacion
                                           }).Distinct();

                    using (MemoryStream mem = new MemoryStream())
                    {
                        using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                        {
                            wordDoc.AddMainDocumentPart();
                            // siga a ordem
                            Document doc = new Document();
                            Body body = new Body();

                            // 1 paragrafo
                            Paragraph titulo = new Paragraph();


                            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
                            ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Normal" };
                            Justification justification1 = new Justification() { Val = JustificationValues.Center };
                            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();

                            paragraphProperties1.Append(paragraphStyleId1);
                            paragraphProperties1.Append(justification1);
                            paragraphProperties1.Append(paragraphMarkRunProperties1);

                            Run run = new Run();
                            RunProperties runProperties1 = new RunProperties();

                            Text text = new Text() { Text = "REPORTE DE ACCIONES CORRECTIVAS" };

                            // siga a ordem 
                            run.Append(runProperties1);
                            run.Append(text);
                            titulo.Append(paragraphProperties1);
                            titulo.Append(run);

                            Table table = new Table();

                            TableProperties props = new TableProperties(
                                new TableBorders(
                                new TopBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                },
                                new BottomBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                },
                                new LeftBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                },
                                new RightBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                },
                                new InsideHorizontalBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                },
                                new InsideVerticalBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Single),
                                    Size = 12
                                }));

                            table.AppendChild<TableProperties>(props);

                            foreach (var x in dataAuditoria)
                            {
                                var tr = new TableRow();

                                var tcFecha = new TableCell();
                                tcFecha.Append(new Paragraph(new Run(new Text("FECHA DE AUDITORIA: " + x.Fecha.ToShortDateString()))));
                                tcFecha.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr.Append(tcFecha);

                                var tcTitulo = new TableCell();
                                tcTitulo.Append(new Paragraph(new Run(new Text("REPORTE DE ACCIONES CORRECTIVAS"))));
                                tcTitulo.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcTitulo.TableCellProperties = new TableCellProperties();
                                tcTitulo.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Restart };
                                tr.Append(tcTitulo);

                                var blank1 = new TableCell();
                                blank1.Append(new Paragraph(new Run(new Text(""))));
                                blank1.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                blank1.TableCellProperties = new TableCellProperties();
                                blank1.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr.Append(blank1);

                                var tcFechaC = new TableCell();
                                tcFechaC.Append(new Paragraph(new Run(new Text("FECHA LIMITE DE CUMPLIMIENTO: "))));
                                tcFechaC.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcFechaC.TableCellProperties = new TableCellProperties();
                                tcFechaC.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Restart };
                                tr.Append(tcFechaC);

                                var blank2 = new TableCell();
                                blank2.Append(new Paragraph(new Run(new Text(""))));
                                blank2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                blank2.TableCellProperties = new TableCellProperties();
                                blank2.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr.Append(blank2);

                                table.Append(tr);

                                ///////////////////////////////////////

                                var tr2 = new TableRow();
                                var tcAuditor = new TableCell();
                                tcAuditor.Append(new Paragraph(new Run(new Text("AUDITOR: " + x.Asesor))));
                                tcAuditor.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcAuditor.TableCellProperties = new TableCellProperties();
                                tcAuditor.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Restart };
                                tr2.Append(tcAuditor);

                                var tcBlank1 = new TableCell();
                                tcBlank1.Append(new Paragraph(new Run(new Text(""))));
                                tcBlank1.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcBlank1.TableCellProperties = new TableCellProperties();
                                tcBlank1.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr2.Append(tcBlank1);

                                var tcBlank2 = new TableCell();
                                tcBlank2.Append(new Paragraph(new Run(new Text(""))));
                                tcBlank2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcBlank2.TableCellProperties = new TableCellProperties();
                                tcBlank2.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr2.Append(tcBlank2);

                                var tcBlank3 = new TableCell();
                                tcBlank3.Append(new Paragraph(new Run(new Text(""))));
                                tcBlank3.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcBlank3.TableCellProperties = new TableCellProperties();
                                tcBlank3.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr2.Append(tcBlank3);

                                var tcBlank4 = new TableCell();
                                tcBlank4.Append(new Paragraph(new Run(new Text(""))));
                                tcBlank4.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tcBlank4.TableCellProperties = new TableCellProperties();
                                tcBlank4.TableCellProperties.HorizontalMerge = new HorizontalMerge { Val = MergedCellValues.Continue };
                                tr2.Append(tcBlank4);

                                table.Append(tr2);

                                ///////////////////////////////////////
                                var tr3 = new TableRow();
                                var tcNPregunta = new TableCell();
                                tcNPregunta.Append(new Paragraph(new Run(new Text("Nº DE PREGUNTA"))));
                                tcNPregunta.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr3.Append(tcNPregunta);

                                var tcJustificacion = new TableCell();
                                tcJustificacion.Append(new Paragraph(new Run(new Text("DESCRIPCIÓN DE LA FALLA"))));
                                tcJustificacion.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr3.Append(tcJustificacion);

                                var tcAcciones = new TableCell();
                                tcAcciones.Append(new Paragraph(new Run(new Text("ACCIONES CORRECTIVAS"))));
                                tcAcciones.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr3.Append(tcAcciones);

                                var tcFechaC2 = new TableCell();
                                tcFechaC2.Append(new Paragraph(new Run(new Text("FECHA DE CUMPLIMIENTO"))));
                                tcFechaC2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr3.Append(tcFechaC2);

                                var tcReviso = new TableCell();
                                tcReviso.Append(new Paragraph(new Run(new Text("REVISÓ"))));
                                tcReviso.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                tr3.Append(tcReviso);

                                table.Append(tr3);

                                ///////////////////////////////////////
                                foreach (var af in puntosControlAF)
                                {
                                    var tr4 = new TableRow();
                                    var tcNPunto = new TableCell();
                                    tcNPunto.Append(new Paragraph(new Run(new Text(af.NoPuntoDesc))));
                                    tcNPunto.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr4.Append(tcNPunto);

                                    var tcJustificacion2 = new TableCell();
                                    tcJustificacion2.Append(new Paragraph(new Run(new Text(af.Justificacion))));
                                    tcJustificacion2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr4.Append(tcJustificacion2);

                                    var tcAcciones2 = new TableCell();
                                    tcAcciones2.Append(new Paragraph(new Run(new Text(""))));
                                    tcAcciones2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr4.Append(tcAcciones2);

                                    var tcFechaC22 = new TableCell();
                                    tcFechaC22.Append(new Paragraph(new Run(new Text(""))));
                                    tcFechaC22.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr4.Append(tcFechaC22);

                                    var tcReviso2 = new TableCell();
                                    tcReviso2.Append(new Paragraph(new Run(new Text(""))));
                                    tcReviso2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr4.Append(tcReviso2);

                                    table.Append(tr4);
                                }

                                ///////////////////////////////////////
                                foreach (var af in puntosControlCB)
                                {
                                    var tr5 = new TableRow();
                                    var tcNPunto = new TableCell();
                                    tcNPunto.Append(new Paragraph(new Run(new Text(af.NoPuntoDesc))));
                                    tcNPunto.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr5.Append(tcNPunto);

                                    var tcJustificacion2 = new TableCell();
                                    tcJustificacion2.Append(new Paragraph(new Run(new Text(af.Justificacion))));
                                    tcJustificacion2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr5.Append(tcJustificacion2);

                                    var tcAcciones2 = new TableCell();
                                    tcAcciones2.Append(new Paragraph(new Run(new Text(""))));
                                    tcAcciones2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr5.Append(tcAcciones2);

                                    var tcFechaC22 = new TableCell();
                                    tcFechaC22.Append(new Paragraph(new Run(new Text(""))));
                                    tcFechaC22.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr5.Append(tcFechaC22);

                                    var tcReviso2 = new TableCell();
                                    tcReviso2.Append(new Paragraph(new Run(new Text(""))));
                                    tcReviso2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));

                                    tr5.Append(tcReviso2);
                                    table.Append(tr5);
                                }


                                ///////////////////////////////////////                                
                                foreach (var af in puntosControlFV)
                                {
                                    var tr6 = new TableRow();
                                    var tcNPunto = new TableCell();
                                    tcNPunto.Append(new Paragraph(new Run(new Text(af.NoPuntoDesc))));
                                    tcNPunto.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr6.Append(tcNPunto);

                                    var tcJustificacion2 = new TableCell();
                                    tcJustificacion2.Append(new Paragraph(new Run(new Text(af.Justificacion))));
                                    tcJustificacion2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr6.Append(tcJustificacion2);

                                    var tcAcciones2 = new TableCell();
                                    tcAcciones2.Append(new Paragraph(new Run(new Text(""))));
                                    tcAcciones2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr6.Append(tcAcciones2);

                                    var tcFechaC22 = new TableCell();
                                    tcFechaC22.Append(new Paragraph(new Run(new Text(""))));
                                    tcFechaC22.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));
                                    tr6.Append(tcFechaC22);

                                    var tcReviso2 = new TableCell();
                                    tcReviso2.Append(new Paragraph(new Run(new Text(""))));
                                    tcReviso2.Append(new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Auto }));

                                    tr6.Append(tcReviso2);
                                    table.Append(tr6);
                                }
                            }
                            body.Append(titulo);
                            body.Append(table);

                            doc.Append(body);
                            wordDoc.MainDocumentPart.Document = doc;
                            wordDoc.Close();
                        }
                        return File(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "REPORTE DE ACCIONES CORRECTIVAS " + IdProdAuditoria + ".docx");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
