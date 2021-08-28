using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using ApiIndicadores.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalisisController : ControllerBase
    {
        Correo correo = new Correo();
        private readonly AppDbContext _context;
        public AnalisisController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";
        int num_analisis=0;

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
        [HttpGet("{idAgen}/{tipo}/{cod_Prod}/{cod_Campo}")]
        public ActionResult Get(short idAgen, string tipo, string cod_Prod = "", short cod_Campo = 0)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
                var analisis = (dynamic)null;

                if (cod_Prod != "" && cod_Campo != 0)
                {
                    buscarnum_analisis(cod_Prod, cod_Campo);
                    return Ok(num_analisis);
                }
                else
                {
                    if (idAgen == 205 || idAgen == 281)
                    {
                        analisis = _context.AnalisisClass.FromSqlRaw($"sp_GetAnalisis " + 205 + "," + tipo + "").ToList();
                    }
                    else
                    {
                        analisis = _context.AnalisisClass.FromSqlRaw($"sp_GetAnalisis " + idAgen + "," + tipo + "").ToList();

                    }
                    return Ok(analisis);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public void buscarnum_analisis(string cod_Prod, short? cod_Campo)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);
                var item = _context.ProdAnalisis_Residuo.FirstOrDefault(a => a.Temporada == catSemanas.Temporada && a.Cod_Prod == cod_Prod && a.Cod_Campo == cod_Campo && a.Fecha == (from c in _context.ProdAnalisis_Residuo where c.Cod_Prod == a.Cod_Prod select c).Max(c => c.Fecha));
                num_analisis = (int)((item == null ? 0 : item.Num_analisis) + 1);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

        // POST api/<AnalisisController>
        [HttpPost("{idMuestreo}/{idAnalisis}/{liberacion_USA}/{liberacion_EU}/{sector}")]
        public ActionResult Post(int idMuestreo, int idAnalisis, int liberacion_USA, int liberacion_EU, short sector, [FromBody] ProdAnalisis_Residuo model)
        {
            try
            {
                ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                int IdSector = 0;
                DateTime fechaLiberacionUSA = DateTime.Now, fechaLiberacionEU = DateTime.Now;
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                string cod_Prod = "";
                short cod_Campo;
                if (idMuestreo != 0)
                {
                    var item = _context.ProdMuestreo.Find(idMuestreo);
                    cod_Prod = item.Cod_Prod;
                    cod_Campo = (short)item.Cod_Campo;
                }
                else if (idAnalisis != 0)
                {
                    var item = _context.ProdAnalisis_Residuo.Find(idAnalisis);
                    cod_Prod = item.Cod_Prod;
                    cod_Campo = (short)item.Cod_Campo;
                }
                else
                {
                    cod_Prod = model.Cod_Prod;
                    cod_Campo = (short)model.Cod_Campo;
                }

                buscarnum_analisis(cod_Prod, cod_Campo);

                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo && m.Fecha_entrega == model.Fecha_entrega && m.Fecha_envio == model.Fecha_envio && m.Estatus == model.Estatus && m.Num_analisis == model.Num_analisis && m.Temporada == catSemanas.Temporada);

                if (analisis == null)
                {
                    var model_sector = _context.ProdMuestreoSector.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo && m.Sector == sector);
                    if (model_sector == null)
                    {
                        prodMuestreoSector.Cod_Prod = cod_Prod;
                        prodMuestreoSector.Cod_Campo = cod_Campo;
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
                    model.Cod_Prod = cod_Prod;
                    model.Cod_Campo = cod_Campo;
                    model.Fecha = DateTime.Now;
                    model.Temporada = catSemanas.Temporada;
                    model.IdSector = IdSector;
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
                    if (model.Estatus != "L")
                    {
                        model.Comentarios = model.Comentarios;
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

                    title = "Código: " + cod_Prod + " campo: " + cod_Campo;
                    body = "Resultado del análisis: " + estatus;
                    notificaciones.SendNotificationJSON(title, body);
                                       
                    enviar(model.IdAgen, cod_Prod, cod_Campo);

                    return Ok(model);
                }

                else
                {
                    return BadRequest();//información duplicada
                }
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public void enviar(short? idAgen_Session, string cod_Prod, short? cod_Campo)
        {
            try
            {
                string correo_p, correo_c = "", correo_i = "";
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Tipo == "P");
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Tipo == "C");
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Tipo == "I");

                correo_p = email_p.correo;

                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);
                var analisis = (dynamic)null;
                var muestreo = _context.ProdMuestreo.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).FirstOrDefault();
                if (muestreo != null)
                {
                    analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                    var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                }
                else
                {
                    analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo);
                }

                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).FirstOrDefault();

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
                        var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).First();
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

                try
                {
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
                        correo.CC.Add(correo_p);
                        correo.CC.Add(correo_c);

                        if (correo_c == "crystyan.torres@giddingsfruit.mx")
                        {
                            correo.CC.Add("judith.santiago@giddingsfruit.mx");
                        }

                        if (correo_i != "jesus.palafox@giddingsfruit.mx")
                        {
                            correo.CC.Add(correo_i);
                        }
                        correo.CC.Add("residente.inocuidad@giddingsfruit.mx");

                        //los reyes
                        if (email_p.IdRegion == 1 || email_p.IdRegion == 8 && correo_c != "mayra.ramirez@giddingsfruit.mx")
                        {
                            //arandas
                            if (correo_p != "aliberth.martinez@giddingsfruit.mx")
                            {
                                correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                            }
                        }
                         
                    }
                    correo.Subject = "Analisis de residuos: " + campo.Cod_Prod + " - " + res_analisis;
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
                        var LiberacionUSA = String.Format("{0:d}", analisis.LiberacionUSA);
                        var LiberacionEU = String.Format("{0:d}", analisis.LiberacionEU);

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
                    if (analisis.Comentarios != null)
                    {
                        correo.Body += "Comentarios: " + analisis.Comentarios + "<br/>";
                        correo.Body += " <br/>";
                    }


                    correo.IsBodyHtml = true;
                    correo.Priority = MailPriority.Normal;

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
            catch (Exception e)
            {
                e.ToString();
            }
        }

    }
}
