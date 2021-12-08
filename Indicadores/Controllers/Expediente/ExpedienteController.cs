using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Expediente
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpedienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ExpedienteController(AppDbContext context)
        {
            this._context = context;
        }

        //[HttpGet]
        //public ActionResult Get()
        //{
        //    try
        //    {
        //        var visitas = _context.VisitasExpedienteClass.FromSqlRaw($"sp_GetExpediente 1, " + idAgen + "").ToList();
        //        var proyeccion = _context.ProyeccionExpedienteClass.FromSqlRaw($"sp_GetExpediente 2, " + idAgen + "").ToList();
        //        var rendimiento = _context.RendimientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 3, " + idAgen + "").ToList();
        //        var financiamiento = _context.FinanciamientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 4, " + idAgen + "").ToList();
               

        //        using (ExcelEngine excelEngine = new ExcelEngine())
        //        {
        //            IApplication application = excelEngine.Excel;
        //            application.DefaultVersion = ExcelVersion.Excel2016;

        //            //Create WorkBook
        //            IWorkbook workbook = application.Workbooks.Create(1);
        //            IWorksheet worksheet = workbook.Worksheets[0];
        //            IStyle style = workbook.Styles.Add("FillColor");

        //            //Disable gridLines
        //            worksheet.IsGridLinesVisible = false;

        //            //Header
        //            worksheet.Range["C1"].Text = "Fecha:";


        //            worksheet.Range["B3"].Text = "USUARIO:";
        //            worksheet.Range["B4"].Text = "COD PRODUCTOR:";
        //            worksheet.Range["B5"].Text = "NOM PRODUCTOR:";
        //            worksheet.Range["B6"].Text = "COD CAMPO(S):";
        //            worksheet.Range["B7"].Text = "NOMBRE CAMPO:";
        //            worksheet.Range["B8"].Text = "ZONA:";


        //            worksheet.Range["B1:C8"].CellStyle.Font.Bold = true;

        //            //Table
        //            worksheet.Range["A11"].Text = "N°";
        //            worksheet.Range["B11"].Text = "Puntos de Control";
        //            worksheet.Range["C11"].Text = "Criterios de Cumplimiento";
        //            worksheet.Range["D11"].Text = "Nivel";
        //            worksheet.Range["E11"].Text = "Si";
        //            worksheet.Range["F11"].Text = "No";
        //            worksheet.Range["G11"].Text = "N/A";
        //            worksheet.Range["H11"].Text = "Justificacion";
        //            worksheet.Range["A11:H11"].CellStyle.Font.Bold = true;
        //            worksheet.Range["A11:H11"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
        //            worksheet.Range["A11:H11"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

        //            //AF
        //            worksheet.Range["A12"].Text = "AF";
        //            worksheet.Range["B12:C12"].Merge();
        //            worksheet.Range["B12"].Text = "MÓDULO BASE PARA TODO TIPO DE FINCA";
        //            worksheet.Range["A12:C12"].CellStyle.Font.Bold = true;

        //            worksheet.Range["B13:C13"].Merge();
        //            worksheet.Range["B13"].Text = "Los puntos de control de este módulo son aplicables a todos los productores que solicitan la certificación, ya que cubren aspectos relevantes a toda actividad agropecuaria";
        //            worksheet.Range["B13:C13"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

        //            //worksheet.Range["A14"].Text = "AF 1";
        //            //worksheet.Range["B14:C14"].Merge();
        //            //worksheet.Range["B14"].Text = "HISTORIAL Y MANEJO DEL SITIO";
        //            //worksheet.Range["A14:C14"].CellStyle.Font.Bold = true;

        //            //worksheet.Range["B15"].Text = "Una de las características clave de la producción agropecuaria sostenible es que continuamente integra los conocimientos específicos al sitio y la experiencia práctica en la planificación del manejo y las prácticas para el futuro. El objetivo de esta sección es asegurar que el campo, los edificios y las otras instalaciones que constituyen el esqueleto de la granja, se gestionen adecuadamente con el fin de garantizar la producción segura de alimentos y la protección del medio ambiente.";
        //            //worksheet.Range["B15"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

        //            //worksheet.Range["A16"].Text = "AF 1.1";
        //            //worksheet.Range["B16:C16"].Merge();
        //            //worksheet.Range["B16"].Text = "Historial del Sitio";
        //            //worksheet.Range["A16:C16"].CellStyle.Font.Bold = true;

        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
        //            worksheet.Range["A11:H13"].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;


        //            int currRowCount = 14;
        //            foreach (var x in res)
        //            {
        //                worksheet.Range["A" + currRowCount].Text = x.NoPuntoDesc;
        //                worksheet.Range["A" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
        //                worksheet.Range["A" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

        //                worksheet.Range["B" + currRowCount].Text = x.PuntoControlDesc;
        //                worksheet.Range["B" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

        //                worksheet.Range["C" + currRowCount].Text = x.Criterio;
        //                worksheet.Range["C" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

        //                worksheet.Range["D" + currRowCount].Text = x.Nivel;
        //                worksheet.Range["D" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
        //                worksheet.Range["D" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

        //                worksheet.Range["F" + currRowCount].Text = "X";
        //                worksheet.Range["F" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
        //                worksheet.Range["F" + currRowCount].CellStyle.HorizontalAlignment = (ExcelHAlign)ExcelVAlign.VAlignCenter;

        //                worksheet.Range["H" + currRowCount].Text = x.Justificacion;
        //                worksheet.Range["H" + currRowCount].CellStyle.VerticalAlignment = ExcelVAlign.VAlignJustify;

        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;

        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeTop].Color = ExcelKnownColors.Black;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].Color = ExcelKnownColors.Black;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeLeft].Color = ExcelKnownColors.Black;
        //                worksheet.Range["A" + currRowCount + ":H" + currRowCount].CellStyle.Borders[ExcelBordersIndex.EdgeRight].Color = ExcelKnownColors.Black;

        //                currRowCount++;
        //            }

        //            //Save the Excel workbook to MemoryStream
        //            MemoryStream stream = new MemoryStream();
        //            workbook.SaveAs(stream);
        //            stream.Position = 0;

        //            Response.ContentType = new MediaTypeHeaderValue("application/octet-stream").ToString();// Content type
        //            return new FileStreamResult(stream, "application/excel") { FileDownloadName = "Indicadores Producción.xlsx" };
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // GET: api/<ExpedienteController>
        [HttpGet("{idAgen}")]
        public ActionResult Get(short idAgen)
        {
            try
            {
                var visitas = _context.VisitasExpedienteClass.FromSqlRaw($"sp_GetExpediente 1, " + idAgen + "").ToList();
                var proyeccion = _context.ProyeccionExpedienteClass.FromSqlRaw($"sp_GetExpediente 2, " + idAgen + "").ToList();
                var rendimiento = _context.RendimientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 3, " + idAgen + "").ToList();
                var financiamiento = _context.FinanciamientoExpedienteClass.FromSqlRaw($"sp_GetExpediente 4, " + idAgen + "").ToList();

                //if (item == "reporte")
                
                //{
                //    using (ExcelEngine excelEngine = new ExcelEngine())
                //    {
                //        IApplication application = excelEngine.Excel;
                //        application.DefaultVersion = ExcelVersion.Excel2016;

                //        //Create WorkBook
                //        IWorkbook workbook = application.Workbooks.Create(1);
                //        IStyle style = workbook.Styles.Add("FillColor");

                //        //Visitas
                //        IWorksheet worksheetVisitas = workbook.Worksheets[0];

                //        //Disable gridLines
                //        worksheetVisitas.IsGridLinesVisible = false;

                //        //Header
                //        worksheetVisitas.Range["A1"].Text = "Temporada";
                //        worksheetVisitas.Range["B1"].Text = "Region";
                //        worksheetVisitas.Range["C1"].Text = "Zona";
                //        worksheetVisitas.Range["D1"].Text = "Asesor";
                //        worksheetVisitas.Range["E1"].Text = "Eficiencia";
                //        worksheetVisitas.Range["F1"].Text = "Efectividad";

                //        worksheetVisitas.Range["A1:F1"].CellStyle.Font.Bold = true;
                //        worksheetVisitas.Range["A1:F1"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                //        worksheetVisitas.Range["A1:F1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

                //        int currRowCount = 2;
                //        foreach (var x in visitas)
                //        {
                //            worksheetVisitas.Range["A" + currRowCount].Text = x.Temporada;
                //            worksheetVisitas.Range["B" + currRowCount].Text = x.Region;
                //            worksheetVisitas.Range["C" + currRowCount].Text = x.Zona;
                //            worksheetVisitas.Range["D" + currRowCount].Text = x.Asesor;
                //            worksheetVisitas.Range["E" + currRowCount].Text = x.Eficiencia;
                //            worksheetVisitas.Range["F" + currRowCount].Text = x.Efectividad;
                //            currRowCount++;
                //        }

                //        //Productividad
                //        IWorksheet worksheetProductividad = workbook.Worksheets[1];
                //        worksheetProductividad.IsGridLinesVisible = false;

                //        //Header
                //        worksheetProductividad.Range["A1"].Text = "Temporada";
                //        worksheetProductividad.Range["B1"].Text = "Region";
                //        worksheetProductividad.Range["C1"].Text = "Zona";
                //        worksheetProductividad.Range["D1"].Text = "Asesor";
                //        worksheetProductividad.Range["E1"].Text = "Pronostico Total";
                //        worksheetProductividad.Range["F1"].Text = "Entregado Sin/Curva";
                //        worksheetProductividad.Range["G1"].Text = "Pronostico Acumulado";
                //        worksheetProductividad.Range["H1"].Text = "Entregado Con/Curva";
                //        worksheetProductividad.Range["I1"].Text = "Asertividad";


                //        worksheetProductividad.Range["A1:I1"].CellStyle.Font.Bold = true;
                //        worksheetProductividad.Range["A1:I1"].CellStyle.ColorIndex = ExcelKnownColors.Grey_25_percent;
                //        worksheetProductividad.Range["A1:I1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;


                //        int c2 = 2;
                //        foreach (var x in proyeccion)
                //        {
                //            worksheetProductividad.Range["A" + c2].Text = x.Temporada;
                //            worksheetProductividad.Range["B" + c2].Text = x.Region;
                //            worksheetProductividad.Range["C" + c2].Text = x.Zona;
                //            worksheetProductividad.Range["D" + c2].Text = x.Asesor;
                //            worksheetProductividad.Range["E" + c2].Number = x.Pronostico;
                //            worksheetProductividad.Range["F" + c2].Number = x.EntregadoSC;
                //            worksheetProductividad.Range["G" + c2].Number = x.PronosticoAA;
                //            worksheetProductividad.Range["H" + c2].Number = x.EntregadoCC;
                //            worksheetProductividad.Range["I" + c2].Number = x.Asertividad;
                //            c2++;
                //        }

                //        //Save the Excel workbook to MemoryStream
                //        MemoryStream stream = new MemoryStream();
                //        workbook.SaveAs(stream);
                //        stream.Position = 0;

                //        Response.ContentType = new MediaTypeHeaderValue("application/octet-stream").ToString();// Content type
                //        return new FileStreamResult(stream, "application/excel") { FileDownloadName = "Indicadores Producción.xlsx" };
                //    }
                //}

                //else
                //{
                    var res = Tuple.Create(visitas.ToList(), proyeccion.ToList(), rendimiento.ToList(), financiamiento.ToList());
                    return Ok(res);
               

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
