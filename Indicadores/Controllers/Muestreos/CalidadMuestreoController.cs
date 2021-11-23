using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalidadMuestreoController : ControllerBase
    {
       
        private readonly AppDbContext _context;
        public CalidadMuestreoController(AppDbContext context)
        {
            this._context = context;
        }
        
        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";        

        // GET: api/<CalidadMuestreoController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdCalidadMuestreo.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<CalidadMuestreoController>/5
        [HttpPut("{id}/{idAgen}")]
        public ActionResult Put(int id, short idAgen, [FromBody] ProdCalidadMuestreo model)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);
                string estatus_calidad = "";

                if (muestreo.Id == id)
                {
                    if (model.Estatus != null)
                    {
                        var item_calidad = _context.ProdCalidadMuestreo.Where(x => x.Id_Muestreo == id).FirstOrDefault();
                        if (item_calidad == null)
                        {
                            ProdCalidadMuestreo prodCalidadMuestreo = new ProdCalidadMuestreo();
                            prodCalidadMuestreo.Estatus = model.Estatus;
                            prodCalidadMuestreo.Fecha = DateTime.Now;
                            prodCalidadMuestreo.Incidencia = model.Incidencia;
                            prodCalidadMuestreo.Propuesta = model.Propuesta;
                            prodCalidadMuestreo.IdAgen = idAgen;
                            prodCalidadMuestreo.Id_Muestreo = id;
                            _context.ProdCalidadMuestreo.Add(prodCalidadMuestreo);
                        }
                        else
                        {
                            item_calidad.Estatus = model.Estatus;
                            item_calidad.Fecha = DateTime.Now;
                            item_calidad.Incidencia = model.Incidencia;
                            item_calidad.Propuesta = model.Propuesta;
                            item_calidad.IdAgen = idAgen;
                        }

                        _context.SaveChanges();

                        if (model.Estatus == "1")
                        {
                            muestreo.Tarjeta = "S";
                            estatus_calidad = "APTA";
                        }
                        else if (model.Estatus == "2")
                        {
                            muestreo.Tarjeta = "S";
                            estatus_calidad = "APTA CON CONDICIONES";
                        }
                        else if (model.Estatus == "3")
                        {
                            muestreo.Tarjeta = "N";
                            estatus_calidad = "PENDIENTE";
                        }

                        _context.SaveChanges();

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Estatus de calidad: " + estatus_calidad;
                    }

                    notificaciones.SendNotificationJSON(title, body);
                    enviar(idAgen, muestreo.Cod_Prod, muestreo.Cod_Campo, "Calidad fruta");

                    _context.SaveChanges();
                    return Ok(model);
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

        public void enviar(short? idAgen_Session, string cod_Prod, short? cod_Campo, string tipo_correo)
        {
            try
            {
                string correo_p, correo_c="", correo_i="";
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Depto != null);
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Depto != null);
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Depto != null);

                correo_p = email_p.correo;

                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);

                var muestreo = _context.ProdMuestreo.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).FirstOrDefault();
                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);

                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).First();
                    
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
                     if (tipo_correo == "Calidad fruta")
                     {
                        if (cod_Prod != "99999")
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_p);
                            correo.CC.Add(correo_i);                             

                            //jalisco
                            if (sesion.IdRegion == 5 && sesion.IdAgen == 250)
                            {
                                correo.CC.Add("moises.juarez@giddingsfruit.mx");
                            }

                            //uruapan
                            if (sesion.IdRegion == 3 && sesion.IdAgen != 168)
                            {
                                correo.CC.Add("juan.mares@giddingsfruit.mx");
                            }

                            //los reyes
                            if (sesion.IdRegion == 1 && sesion.IdAgen != 167 && sesion.IdAgen != 144)
                            {
                                //arandas
                                if (correo_p != "aliberth.martinez@giddingsfruit.mx")
                                {
                                    correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                                }
                            }                           
                        }

                        else {
                            correo.To.Add("marholy.martinez@giddingsfruit.mx");
                        }

                        correo.Subject = "Calidad de fruta evaluada: " + cod_Prod;
                        correo.Body += "Evaluado por: " + sesion.Completo + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                        correo.Body += " <br/>";
                        if (calidad_muestreo.Estatus == "1")
                        {
                            correo.Body += "Estatus: APTA <br/>";
                            correo.Body += " <br/>";
                            if (analisis != null)
                            {
                                if (analisis.Estatus == "L")
                                {
                                    correo.Body += "Entregar tarjeta <br/>";
                                    correo.Body += " <br/>";
                                }
                            }
                        }
                        else if (calidad_muestreo.Estatus == "2")
                        {
                            correo.Body += "Estatus: APTA CON CONDICIONES<br/>";
                            correo.Body += " <br/>";
                            if (analisis != null)
                            {
                                if (analisis.Estatus == "L")
                                {
                                    correo.Body += "Entregar tarjeta <br/>";
                                    correo.Body += " <br/>";
                                }
                            }
                        }
                        else if (calidad_muestreo.Estatus == "3")
                        {
                            correo.Body += "Estatus: PENDIENTE <br/>";
                            correo.Body += " <br/>";
                            correo.Body += "No entregar tarjeta <br/>";
                            correo.Body += " <br/>";
                        }
                        if (calidad_muestreo.Incidencia != "")
                        {
                            correo.Body += "Incidencia: " + calidad_muestreo.Incidencia + " <br/>";
                            correo.Body += " <br/>";
                            if (calidad_muestreo.Propuesta != "")
                            {
                                correo.Body += "Propuesta: " + calidad_muestreo.Propuesta + " <br/>";
                                correo.Body += " <br/>";
                            }
                        }
                    }

                    correo.IsBodyHtml = true;
                    correo.BodyEncoding = System.Text.Encoding.UTF8;
                    correo.Priority = MailPriority.Normal;

                    string sSmtpServer = "";
                    sSmtpServer = "smtp.gmail.com";

                    SmtpClient a = new SmtpClient();
                    a.Host = sSmtpServer;
                    a.Port = 587;//25
                    a.EnableSsl = true;
                    a.UseDefaultCredentials = true;
                    a.Credentials = new System.Net.NetworkCredential
                       ("indicadores.giddingsfruit@gmail.com", "kwfgnrflueomsrok");
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
