using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using ApiIndicadores.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisisController : ControllerBase
    {
        Correo correo = new Correo();
        private readonly AppDbContext _context;
        GetMimeTypes getMimeTypes = new GetMimeTypes();
        public AnalisisController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";
        int num_analisis = 0;

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
        [HttpGet("{id}/{tipo}/{depto}/{idMuestreo}/{estatus}")]
        //id es idagen o idusuario
        public ActionResult Get(short id = 0, string tipo = null, string depto = null, int idMuestreo=0, string estatus = null)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
                var analisis = (dynamic) null;

                if (idMuestreo != 0)
                {
                    buscarnum_analisis(idMuestreo);
                    return Ok(num_analisis);
                }
                else
                {
                    analisis = _context.AnalisisClass.FromSqlRaw($"sp_GetAnalisis " + id + "," + tipo + "," + depto + "," + estatus).ToList();
                    return Ok(analisis);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{idAnalisis}")]
        //imagen
        public async Task<ActionResult<ProdAnalisis_Residuo>> Get(int idAnalisis)
        {
            try
            {
                string path = "//192.168.0.21/recursos season/AnalisisResiduosPDF/" + idAnalisis+".pdf";
                
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;               
                return File(memory, "application/pdf", Path.GetFileName(path));
            }
            catch (Exception e) {
                return BadRequest(e.ToString());
            }
        }

        public void buscarnum_analisis(int idMuestreo)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
                //var item = _context.ProdAnalisis_Residuo.FirstOrDefault(a => a.Temporada == catSemanas.Temporada && a.Cod_Prod == cod_Prod && a.Cod_Campo == cod_Campo && a.Fecha == (from c in _context.ProdAnalisis_Residuo where c.Cod_Prod == a.Cod_Prod select c).Max(c => c.Fecha));
                var item = _context.ProdAnalisis_Residuo.FirstOrDefault(a => a.Id_Muestreo == idMuestreo);
                num_analisis = (int)((item == null ? 0 : item.Num_analisis) + 1);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        // POST api/<AnalisisController>
        [HttpPost("{idMuestreo}/{liberacion_USA}/{liberacion_EU}")] //int idAnalisis, short sector
        public ActionResult Post(int idMuestreo, int liberacion_USA, int liberacion_EU, [FromBody] ProdAnalisis_Residuo model)
        {
            try
            {
                //ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                //int IdSector = 0;
                //DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;
                //var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                //string cod_Prod = "";
                //short cod_Campo;
                //if (idMuestreo != 0)
                //{
                //    var item = _context.ProdMuestreo.Find(idMuestreo);
                //    cod_Prod = item.Cod_Prod;
                //    cod_Campo = (short)item.Cod_Campo;
                //}
                //else if (idAnalisis != 0)
                //{
                //    var item = _context.ProdAnalisis_Residuo.Find(idAnalisis);
                //    cod_Prod = item.Cod_Prod;
                //    cod_Campo = (short)item.Cod_Campo;
                //}
                //else
                //{
                //    cod_Prod = model.Cod_Prod;
                //    cod_Campo = (short)model.Cod_Campo;
                //}
                if (model.IdAgen != 0)
                {
                    var muestreo = _context.ProdMuestreo.Where(m => m.Id == idMuestreo).FirstOrDefault();

                    buscarnum_analisis(idMuestreo);


                    var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                    DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;

                    var analisis = _context.ProdAnalisis_Residuo.Where(m =>
                    m.Cod_Prod == muestreo.Cod_Prod &&
                    m.Cod_Campo == muestreo.Cod_Campo &&
                    m.Fecha_entrega == model.Fecha_entrega &&
                    m.Fecha_envio == model.Fecha_envio &&
                    m.Estatus == model.Estatus &&
                    m.Num_analisis == num_analisis &&
                    m.Temporada == "2122").FirstOrDefault(); //catSemanas.Temporada

                    if (analisis == null)
                    {
                        //var model_sector = _context.ProdMuestreoSector.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo && m.Sector == sector);
                        //if (model_sector == null)
                        //{
                        //    prodMuestreoSector.Cod_Prod = cod_Prod;
                        //    prodMuestreoSector.Cod_Campo = cod_Campo;
                        //    prodMuestreoSector.Sector = sector;
                        //    _context.ProdMuestreoSector.Add(prodMuestreoSector);
                        //    _context.SaveChanges();

                        //    IdSector = prodMuestreoSector.id;
                        //}
                        //else
                        //{
                        //    IdSector = model_sector.id;
                        //}

                        model.Cod_Empresa = 2;
                        model.Cod_Prod = muestreo.Cod_Prod;
                        model.Cod_Campo = muestreo.Cod_Campo;
                        model.Fecha = DateTime.Now;
                        model.Temporada = "2122";// catSemanas.Temporada;
                        model.Num_analisis = num_analisis;
                        if (idMuestreo != 0)
                        {
                            model.Id_Muestreo = idMuestreo;
                        }

                        if (model.Estatus == "F")
                        {
                            fechaLiberacionUSA = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_USA);
                            fechaLiberacionEU = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_EU);

                            model.LiberacionUSA = fechaLiberacionUSA;
                            model.LiberacionEU = fechaLiberacionEU;
                        }
                        if (model.Traza == "on")
                        {
                            model.Traza = "1";
                        }
                        if (model.Organico == "on")
                        {
                            model.Organico = "1";
                        }
                        if (model.Estatus == "L")
                        {
                            model.Comentarios = null;
                        }
                        else
                        {
                            model.Comentarios = model.Comentarios;
                        }

                        if (model.LiberaDocumento == "on")
                        {
                            model.LiberaDocumento = "S";
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

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Resultado del análisis: " + estatus;
                        notificaciones.SendNotificationJSON(title, body);

                        enviar(model.IdAgen, model.Id, "nuevo");

                        return Ok(model);
                    }

                    else
                    {
                        return BadRequest();//información duplicada
                    }
                }
                else
                {
                    return BadRequest("Usuario incorrecto");//información duplicada
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Editar análisis
        [HttpPut("{id}/{liberacion_USA}/{liberacion_EU}")]
        public async Task<ActionResult<ProdAnalisis_Residuo>> Put(int id, int liberacion_USA, int liberacion_EU, [FromBody] ProdAnalisis_Residuo model)
        {
            try
            {
                var analisis = _context.ProdAnalisis_Residuo.Find(id);
                if (analisis != null)
                {
                    DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;

                    if (model.Fecha_envio != null) 
                    {
                        analisis.Fecha_envio = model.Fecha_envio;
                    }

                    if (model.Fecha_entrega != null)
                    {
                        analisis.Fecha_entrega = model.Fecha_entrega;
                    }

                    if (model.Laboratorio != null || model.Laboratorio !="")
                    {
                        analisis.Laboratorio = model.Laboratorio;
                    }

                    if (model.Comentarios != null || model.Comentarios != "") 
                    {
                        analisis.Comentarios = model.Comentarios;
                    }                    

                    if (model.CodZona != null || model.CodZona != "")
                    {
                        analisis.CodZona = model.CodZona;
                    }

                    if (model.Estatus != null || model.Estatus != "")
                    {
                        analisis.Estatus = model.Estatus;

                        if (model.Estatus == "F")
                        {
                            fechaLiberacionUSA = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_USA);
                            fechaLiberacionEU = Convert.ToDateTime(model.Fecha_envio).AddDays(liberacion_EU);

                            analisis.LiberacionUSA = fechaLiberacionUSA;
                            analisis.LiberacionEU = fechaLiberacionEU;
                        }
                        else
                        {
                            analisis.LiberacionUSA = null;
                            analisis.LiberacionEU = null;
                        }
                    }
                    if (model.Folio != null || model.Folio != "")
                    {
                        analisis.Folio = model.Folio;
                    }                                      

                    if (model.Traza == "on")
                    {
                        analisis.Traza = "1";
                    }
                    else
                    {
                        analisis.Traza = null;
                    }

                    if (model.Organico == "on")
                    {
                        analisis.Organico = "1";
                    }
                    else
                    {
                        analisis.Organico = null;
                    }

                    if (model.ParteMuestreada != null || model.ParteMuestreada !="")
                    {
                        analisis.ParteMuestreada = model.ParteMuestreada;
                    } 

                    analisis.Fecha = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(analisis);
                }
                else
                {
                    return NotFound();
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Subir PDF del Laboratorio 
        [HttpPatch("{idAnalisis}")]
        public async Task<ActionResult<ProdAnalisis_Residuo>> Patch(int idAnalisis, [FromForm] IFormFile file)
        {
            try
            {
                var analisis = _context.ProdAnalisis_Residuo.Find(idAnalisis);
                if (analisis != null)
                {
                    if (file != null)
                    {
                        var extension = Path.GetExtension(file.FileName).Substring(1);
                        var path = "//192.168.0.21/recursos season/AnalisisResiduosPDF/" + idAnalisis + "."+ extension;
                        
                        using (var stream = System.IO.File.Create(path))
                        {
                           await file.CopyToAsync(stream);
                        }

                        analisis.PDFLaboratorio = path;
                    }
                    _context.SaveChanges();
                    return Ok(analisis);
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

       // Liberar Fuera de Limite
        [HttpPatch("{id}/{idAgen}")]
        public async Task<ActionResult<ProdAnalisis_Residuo>> Patch(int id, short idAgen)
        {
            try
            {
                var analisis = _context.ProdAnalisis_Residuo.Find(id);
                if (analisis != null)
                {
                    analisis.Estatus = "L";
                    analisis.Fecha = DateTime.Now;
                    await _context.SaveChangesAsync();

                    enviar(idAgen, analisis.Id, "liberar");
                    return Ok(analisis);
                }
                else
                {
                    return NotFound();
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        ////Delete  
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdAnalisis_Residuo>> Delete(int id)
        {
            try
            {
                var model = _context.ProdAnalisis_Residuo.Find(id);
                if (model != null)
                {
                    _context.ProdAnalisis_Residuo.Remove(model);
                    await _context.SaveChangesAsync();
                    return Ok(id);
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

        //correo
        public void enviar(short idAgen_Session, int idAnalisis, string tipo_correo)
        {
            try
            {
                string correo_p, correo_c = "", correo_i = "";
                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id == idAnalisis);
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == analisis.Cod_Prod && m.Cod_Campo == analisis.Cod_Campo);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Depto == "P");
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Depto == "C");
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Depto == "I");

                correo_p = email_p.correo;
                var LiberacionUSA = String.Format("{0:d}", analisis.LiberacionUSA);
                var LiberacionEU = String.Format("{0:d}", analisis.LiberacionEU);

                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);

                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == analisis.Cod_Prod && x.Cod_Campo == analisis.Cod_Campo).FirstOrDefault();

                    //Uruapan
                    if (sesion.IdRegion == 3)
                    {
                        item.IdAgenC = 168;
                        correo_c = "juan.mares@giddingsfruit.mx";
                    }

                    //Los Reyes
                    if (sesion.IdRegion == 1 || sesion.IdRegion == 8)
                    {
                        item.IdAgenC = 167;
                        correo_c = "mayra.ramirez@giddingsfruit.mx";
                    }
                    _context.SaveChanges();
                }
                else
                {
                    correo_c = email_c.correo;
                }

                if (email_i == null)
                {
                    //Los Reyes
                    if (sesion.IdRegion == 1 || sesion.IdRegion == 8)
                    {
                        var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == analisis.Cod_Prod && x.Cod_Campo == analisis.Cod_Campo).First();
                        item.IdAgenI = 205;
                        _context.SaveChanges();
                        correo_i = "jesus.palafox@giddingsfruit.mx";
                    }
                    else
                    {
                        correo_i = "hector.torres@giddingsfruit.mx";
                    }
                }
                else
                {
                    correo_i = email_i.correo;
                }

                var muestreo = _context.ProdMuestreo.Where(m => m.Id==analisis.Id_Muestreo).FirstOrDefault();
                var calidad_muestreo = _context.ProdCalidadMuestreo.Where(x => x.Id_Muestreo == muestreo.Id).FirstOrDefault();

                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                var prod = _context.ProdProductoresCat.FirstOrDefault(x => x.Cod_Prod == campo.Cod_Prod);
                var Fecha_envio = String.Format("{0:d}", analisis.Fecha_envio);
                var Fecha_entrega = String.Format("{0:d}", analisis.Fecha_entrega);

                string res_analisis = "";
                if (analisis.Estatus == "R")
                {
                    res_analisis = "CON RESIDUOS";
                }
                else if (analisis.Estatus == "P")
                {
                    res_analisis = "EN PROCESO";
                }
                else if (analisis.Estatus == "F")
                {
                    res_analisis = "FUERA DEL LIMITE";
                }
                else if (analisis.Estatus == "L")
                {
                    res_analisis = "LIBERADO";
                }

                if (campo.Cod_Prod == "99999")
                {
                    correo.To.Add("marholy.martinez@giddingsfruit.mx");
                }
                else
                {
                    correo.To.Add(sesion.correo);
                    if (email_p.IdAgen == 112)
                    {
                        correo.CC.Add("alejandro.alvarado@giddingsfruit.mx");
                    }
                    else
                    {
                        correo.CC.Add(correo_p);
                    }
                     
                    correo.CC.Add(correo_c);                    

                    if (email_c.IdAgen == 29)
                    {
                        correo.CC.Add("judith.santiago@giddingsfruit.mx");
                        correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                    }

                    if (correo_i != "jesus.palafox@giddingsfruit.mx")
                    {
                        correo.CC.Add(correo_i);
                    }
                    correo.CC.Add("residente.inocuidad@giddingsfruit.mx");

                    //los reyes
                    if (email_p.IdRegion == 1 && email_c.IdAgen != 167 && email_c.IdAgen != 144)
                    {
                        //arandas
                        if (email_p.IdAgen != 197)
                        {
                            correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                        }
                    }
                }

                if (tipo_correo == "nuevo")
                {
                    correo.Subject = "Analisis de residuos: " + campo.Cod_Prod + " - " + res_analisis;
                    correo.Body += "Realizado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + campo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + campo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de envio: " + Fecha_envio + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de entrega: " + Fecha_entrega + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Estatus: " + res_analisis + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Número de análisis: " + num_analisis + "<br/>";
                    correo.Body += " <br/>";

                    if (muestreo != null)
                    {
                        if (muestreo.Tarjeta != null)
                        {
                            if (analisis.Estatus == "L" && muestreo.Tarjeta == "S")
                            {
                                correo.Body += "Entregar tarjeta <br/>";
                                correo.Body += " <br/>";
                            }
                        }
                    }

                    if (analisis.Estatus == "F")
                    {                      

                        if (analisis.LiberacionUSA != null)
                        {
                            correo.Body += "Fecha de liberacion para USA: " + LiberacionUSA + "<br/>";
                            correo.Body += " <br/>";
                        }
                        if (analisis.LiberacionEU != null)
                        {
                            correo.Body += "Fecha de liberacion para EUROPA: " + LiberacionEU + "<br/>";
                            correo.Body += " <br/>";
                        }
                    }
                    //correo.Body += "Numero de analisis: " + analisis.Num_analisis + "<br/>";
                    //correo.Body += " <br/>";
                    correo.Body += "Laboratorio: " + analisis.Laboratorio + "<br/>";
                    correo.Body += " <br/>";

                    if (analisis.Estatus != "L")
                    {
                        if (analisis.Comentarios != null)
                        {
                            correo.Body += "Comentarios: " + analisis.Comentarios + "<br/>";
                            correo.Body += " <br/>";
                        }
                    }
                }
                else if (tipo_correo == "liberar")
                {
                    correo.Subject = "Analisis Liberado: " + campo.Cod_Prod + " - " + res_analisis;
                    correo.Body += "Realizado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "El resultado del análisis ha cambiado de Fuera de Límite a Liberado <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + campo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + campo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de Liberación USA: " + LiberacionUSA + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de Liberación EU: " + LiberacionEU + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += analisis.Comentarios + "<br/>";
                    correo.Body += " <br/>";

                }

                correo.IsBodyHtml = true;
                correo.BodyEncoding = System.Text.Encoding.UTF8;
                correo.Priority = MailPriority.High;

                string sSmtpServer = "";
                sSmtpServer = "smtp.gmail.com";

                SmtpClient a = new SmtpClient();
                a.Host = sSmtpServer;
                a.Port = 587;//25
                a.EnableSsl = true;
                a.UseDefaultCredentials = true;
                a.Credentials = new System.Net.NetworkCredential
                   ("indicadores.giddingsfruit@gmail.com", "indicadores2019");
                a.Send(correo);
            }
            catch (Exception e)
            {
                e.ToString();
            }

        }

    }
}
