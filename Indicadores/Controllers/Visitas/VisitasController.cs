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
        Image img = null; 

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


                if (idAgen == 1 || idAgen == 5 || idAgen == 50)
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
