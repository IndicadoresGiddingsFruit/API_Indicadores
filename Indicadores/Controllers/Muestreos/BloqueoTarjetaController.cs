using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Muestreos
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloqueoTarjetaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BloqueoTarjetaController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";
        string correo_p, correo_c, correo_i;

        // GET: api/<BloqueoTarjetaController>
        [HttpGet]
        public async Task<ActionResult<ProdBloqueoTarjeta>> Get()
        {
            var sectores = _context.ProdBloqueoTarjeta.Distinct();
            return Ok(await sectores.ToListAsync());
        }
        // GET api/<BloqueoTarjetaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BloqueoTarjetaController>
        [HttpPost]
        public async Task<ActionResult<ProdBloqueoTarjeta>> Post([FromBody] ProdBloqueoTarjeta model)
        {
            try
            {
                var item = _context.ProdBloqueoTarjeta.Where(x => x.Cod_Prod == model.Cod_Prod && x.Cod_Campo == model.Cod_Campo).FirstOrDefault();
                if (item == null)
                {
                    model.Fecha = DateTime.Now;
                    _context.ProdBloqueoTarjeta.Add(model);
                    await _context.SaveChangesAsync();

                    title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    body = "Entrega de tarjeta denegada";

                    notificaciones.SendNotificationJSON(title, body);
                    _context.SaveChanges();

                    enviar(model.IdAgen, model.Cod_Prod, model.Cod_Campo);

                    return Ok(model);
                }
                else
                {
                    return BadRequest("Esté código ya fué bloqueado anteriormente");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<BloqueoTarjetaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BloqueoTarjetaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //correo
        public void enviar(short idAgen_Session, string cod_Prod, short cod_Campo)
        {
            try
            {
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
                var localidad = _context.CatLocalidades.FirstOrDefault(m => m.CodLocalidad == campo.CodLocalidad);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Depto == "P" && m.Depto != null);
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Depto == "C" && m.Depto != null);
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Depto == "I" && m.Depto != null);

                var sesion = _context.SIPGUsuarios.FirstOrDefault();

                correo_p = email_p.correo;

                //Administrador
                if (idAgen_Session == 352)
                {
                    sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.Id == idAgen_Session);
                }
                //Agentes
                else
                {
                    sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);
                }

                var model = _context.ProdBloqueoTarjeta.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).FirstOrDefault();
             
                //Si agente de calidad es null
                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo && x.Cod_Empresa == 2);

                    //uruapan
                    if (email_p.IdRegion == 3)
                    {
                        item.IdAgenC = 168;
                        correo_c = "juan.mares@giddingsfruit.mx";
                    }

                    //los reyes 
                    if (email_p.IdRegion == 1 && email_p.IdAgen != 197)
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

                //si agente de inocuidad es null
                if (email_i == null)
                {
                    //Los Reyes
                    if (sesion.IdRegion == 1 && email_p.IdAgen != 197)
                    {
                        var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo && x.Cod_Empresa == 2);

                        if (sesion.IdAgen == 216)
                        {
                            item.IdAgenI = 216;
                            correo_i = "angel.heredia@giddingsfruit.mx";
                        }

                        else
                        {
                            item.IdAgenI = 205;
                            correo_i = "jesus.palafox@giddingsfruit.mx";
                        }
                        _context.SaveChanges();
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


                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                var prod = _context.ProdProductoresCat.FirstOrDefault(x => x.Cod_Prod == campo.Cod_Prod);


                if (cod_Prod == "99999")
                {
                    correo.To.Add("marholy.martinez@giddingsfruit.mx");
                }
                else
                {
                    correo.To.Add(sesion.correo);
                    correo.CC.Add(correo_p);
                    correo.CC.Add(correo_c);

                    if (sesion.correo != correo_i)
                    {
                        correo.CC.Add(correo_i);
                    }

                    if (email_c.IdAgen == 29)
                    {
                        correo.CC.Add("judith.santiago@giddingsfruit.mx");
                        correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                    }
                }

                correo.Subject = "Bloqueo de entrega de tarjeta: " + cod_Prod;
                correo.Body = "Bloqueado por: " + sesion.Completo + " <br/>";
                correo.Body += " <br/>";
                correo.Body += "Se ha denegado la liberación para entrega de tarjeta del productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                correo.Body += " <br/>";
                correo.Body += "campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                correo.Body += " <br/>";
                correo.Body += "Ubicación: " + localidad.Descripcion + " <br/>";
                correo.Body += " <br/>";
                correo.Body += "Justificacion: " + model.Justificacion + "<br/>";
                correo.Body += " <br/>";


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
                a.Credentials = new System.Net.NetworkCredential("indicadores.giddingsfruit@gmail.com", "indicadores2019");
                a.Send(correo);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }

    }
} 