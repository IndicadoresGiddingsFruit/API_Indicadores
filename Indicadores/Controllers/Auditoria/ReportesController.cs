using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("{idAgen}/{IdProdAuditoria}")]
        public ActionResult Get(int idAgen, int IdProdAuditoria)

        {
            try
            {
                var dataAuditoria = _context.AuditoriaClass.FromSqlRaw($"sp_GetAuditoria " + idAgen + "," + IdProdAuditoria + "").ToList();

                var puntosControlAF = (from l in _context.ProdLogAuditoria
                                     join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                                     join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                                     join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                                     where l.IdProdAuditoria == IdProdAuditoria && l.Opcion=="NO" && c.NoPunto=="AF"
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


                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    //Create WorkBook
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];
                    IStyle style = workbook.Styles.Add("FillColor");

                    //Disable gridLines
                    worksheet.IsGridLinesVisible = false;

                    //Header
                    worksheet.Range["C1"].Text = "Fecha:";


                    worksheet.Range["B3"].Text = "USUARIO:";
                    worksheet.Range["B4"].Text = "COD PRODUCTOR:";
                    worksheet.Range["B5"].Text = "NOM PRODUCTOR:";
                    worksheet.Range["B6"].Text = "COD CAMPO(S):";
                    worksheet.Range["B7"].Text = "NOMBRE CAMPO:";
                    worksheet.Range["B8"].Text = "ZONA:";

                    foreach (var x in dataAuditoria)
                    {
                        worksheet.Range["C3"].Text = x.Asesor;
                        worksheet.Range["C4"].Text = x.Cod_Prod;
                        worksheet.Range["C5"].Text = x.Productor;
                        worksheet.Range["C6"].Text = x.Cod_Campo;
                        worksheet.Range["C6"].Text = x.Campo;
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
                    worksheet.Range["A"+ currRowCount].Text = "CB";
                    worksheet.Range["B"+ currRowCount + ":C" + currRowCount].Merge();
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
