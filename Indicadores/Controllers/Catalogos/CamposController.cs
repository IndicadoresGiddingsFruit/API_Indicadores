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

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamposController : ControllerBase
    {

        private readonly AppDbContext _context;
        public CamposController(AppDbContext context)
        {
            this._context = context;
        }

        Correo correo = new Correo();
        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";

        //Campos
        [HttpGet("{Cod_Prod}/{Cod_Campo}")]
        public ActionResult GetData(string Cod_Prod, short Cod_Campo=0)
        {
            try
            {
                var item = _context.InfoCampoClass.FromSqlRaw($"sp_GetInfoCampos '" + Cod_Prod + "', "+ Cod_Campo + " ").ToList();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // PUT api/<CamposController>/5
        //Reasignar codigo
        [HttpPut("{idAgenOriginal}/{idAgen}/{tipo}")]
        public async Task<ActionResult<ProdCamposCat>> Put(short idAgenOriginal, short idAgen, string tipo, [FromBody] ProdMuestreo model)
        {
            try
            {
                var model_campo = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == model.Cod_Prod && x.Cod_Campo == model.Cod_Campo && x.Cod_Empresa == 2);
                if (model_campo != null)
                {
                    if (tipo == "P")
                    {
                        model_campo.IdAgen = idAgen;
                    }
                    else if (tipo == "C" || idAgen == 304)
                    {
                        model_campo.IdAgenC = idAgen;
                    }
                    else if (tipo == "I")
                    {
                        model_campo.IdAgenI = idAgen;
                    }

                    await _context.SaveChangesAsync();

                    title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    body = "Código reasignado";
                    notificaciones.SendNotificationJSON(title, body);

                    enviar(idAgenOriginal, model.Cod_Prod, model.Cod_Campo, tipo);

                    return Ok(model);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public void enviar(short idAgen_Session, string cod_Prod, short cod_Campo, string tipo)
        {
            try
            {
                string correo_p, correo_c, correo_i;
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen);
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC);
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI);

                correo_p = email_p.correo;

                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);

                var muestreo = _context.ProdMuestreo.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).FirstOrDefault();
                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);

                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).FirstOrDefault();
                    if (sesion.IdRegion == 3)
                    {
                        item.IdAgenC = 168;
                        correo_c = "juan.mares@giddingsfruit.mx";
                    }
                    if (sesion.IdRegion == 1)
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
                    if (sesion.IdRegion == 3)
                    {
                        correo_c = "hector.torres@giddingsfruit.mx";
                    }
                    if (sesion.IdRegion == 1 || sesion.IdRegion == 8) //Los Reyes
                    {
                        var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).FirstOrDefault();
                        item.IdAgenI = 205;
                        _context.SaveChanges();
                        correo_i = "jesus.palafox@giddingsfruit.mx";
                    }
                    else
                    {
                        correo_i = email_i.correo;
                    }
                }

                try
                {
                    MailMessage correo = new MailMessage();
                    correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                    var prod = _context.ProdProductoresCat.FirstOrDefault(x => x.Cod_Prod == campo.Cod_Prod);
                    var agente = (dynamic)null;

                    if (tipo == "P")
                    {
                        agente = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen);
                    }
                    if (tipo == "C")
                    {
                        agente = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC);
                    }
                    if (tipo == "I")
                    {
                        agente = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI);
                    }

                    if (cod_Prod == "99999")
                    {
                        correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    }

                    else
                    {
                        correo.To.Add(sesion.correo);
                        correo.CC.Add(agente.correo);
                        correo.CC.Add("oscar.castillo@giddingsfruit.mx");
                    }

                    correo.Subject = "Reasignación de código: " + cod_Prod;
                    correo.Body += "El productor: " + cod_Prod + " - " + prod.Nombre + " con campo: " + cod_Campo + " - " + campo.Descripcion +
                        " ha sido reasignado a " + agente.Completo + " por " + sesion.Completo + " <br/>";

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
