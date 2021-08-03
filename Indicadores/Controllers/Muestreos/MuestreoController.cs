﻿using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuestreoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MuestreoController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";
        string correo_p, correo_c, correo_i;

        // GET: api/<MuestreoController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdMuestreo.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<MuestreoController>/5
        [HttpGet("{idAgen}/{tipo}", Name = "GetMuestreo")]
        public async Task<ActionResult<MuestreosClass>> Get(short idAgen, string tipo)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                IQueryable<SectoresClass> listaSectores = (from m in _context.ProdMuestreo
                                                           join s in _context.ProdMuestreoSector on new { m.Cod_Prod, m.Cod_Campo } equals new { s.Cod_Prod, s.Cod_Campo } into Sectores
                                                           from s in Sectores.DefaultIfEmpty()
                                                           group m by new
                                                           {
                                                               IdMuestreo = m.Id,
                                                               Cod_Prod = m.Cod_Prod,
                                                               Cod_Campo = m.Cod_Campo,
                                                               IdSector = s.id,
                                                               Sector = s.Sector
                                                           } into x
                                                           select new SectoresClass()
                                                           {
                                                               IdMuestreo = x.Key.IdMuestreo,
                                                               Cod_Prod = x.Key.Cod_Prod,
                                                               Cod_Campo = x.Key.Cod_Campo,
                                                               IdSector = x.Key.IdSector,
                                                               Sector = x.Key.Sector,
                                                           }).Distinct();

                var muestreos =  _context.MuestreosClass.FromSqlRaw($"sp_GetMuestreos " + idAgen + "," + tipo + "").ToListAsync();
                return Ok(await muestreos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<MuestreoController>
        [HttpPost]
        public ActionResult Post([FromBody] ProdMuestreo model)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now.Date >= m.Inicio && DateTime.Now.Date <= m.Fin);
                var agente = _context.SIPGUsuarios.FirstOrDefault(s => s.IdAgen == model.IdAgen);
                var campos = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo);

                var modeloExistente = _context.ProdMuestreo.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Temporada == catSemanas.Temporada);
                if (modeloExistente == null)
                {
                    var usuario = _context.SIPGUsuarios.Where(x => x.IdAgen == model.IdAgen).First();

                    if (usuario.Tipo == "P")
                    {
                        model.Liberacion = "S";
                    }
                    model.Fecha_solicitud = DateTime.Now;
                    model.Temporada = catSemanas.Temporada;
                    if (agente.Tipo != "P")
                    {
                        model.IdAgen = campos.IdAgen;
                    }
                    _context.ProdMuestreo.Add(model);
                    _context.SaveChanges();

                    title = "Nuevo muestreo solicitado";
                    body = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    notificaciones.SendNotificationJSON(title, body);

                    enviar(agente.IdAgen, model.Cod_Prod, model.Cod_Campo, "nuevo");

                    return Ok(model);
                }
                else
                {
                    return BadRequest("La solicitud ya existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<MuestreoController>/5
        //fecha_ejecucion  
        [HttpPut("{id}/{idAgen}/{sector}")]
        public ActionResult Put(int id, short idAgen, short sector, [FromBody] ProdMuestreo model)
        {
            try
            {
                ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                int IdMuestreoSector = 0;

                var item = _context.ProdMuestreo.Find(id);
                if (item.Id == id)
                {
                    var model_sector = _context.ProdMuestreoSector.Where(x => x.Cod_Prod == item.Cod_Prod && x.Cod_Campo == item.Cod_Campo && x.Sector == sector).FirstOrDefault();
                    if (model_sector == null)
                    {
                        prodMuestreoSector.Cod_Prod = item.Cod_Prod;
                        prodMuestreoSector.Cod_Campo = item.Cod_Campo;
                        prodMuestreoSector.Sector = sector;
                        _context.ProdMuestreoSector.Add(prodMuestreoSector);
                        _context.SaveChanges();

                        IdMuestreoSector = prodMuestreoSector.id;
                    }
                    else
                    {
                        IdMuestreoSector = model_sector.id;
                    }

                    if (IdMuestreoSector != 0)
                    {
                        item.IdSector = IdMuestreoSector;
                        item.IdAgenI = idAgen;
                        if (model.Fecha_ejecucion != null)
                        {
                            item.Fecha_ejecucion = model.Fecha_ejecucion;
                        }
                        _context.SaveChanges();

                        title = "Código: " + item.Cod_Prod + " campo: " + item.Cod_Campo;
                        body = "Fecha de muestreo agregada: " + model.Fecha_ejecucion;
                        notificaciones.SendNotificationJSON(title, body);

                        enviar(idAgen, item.Cod_Prod, item.Cod_Campo, "fecha_ejecucion");
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Liberar solicitud 
        [HttpPatch("{id}/{idAgen}")]
        public ActionResult Patch(int id, short idAgen)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);

                if (muestreo.Id == id)
                {
                    muestreo.Liberacion = "S";
                    title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                    body = "Solicitud de muestreo liberada por producción ";

                    notificaciones.SendNotificationJSON(title, body);
                    _context.SaveChanges();

                    enviar(idAgen, muestreo.Cod_Prod, muestreo.Cod_Campo, "Muestreo Liberado");

                    return Ok(muestreo);
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

        // DELETE api/<MuestreoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.FirstOrDefault(m => m.Id == id);
                if (muestreo != null)
                {
                    _context.ProdMuestreo.Remove(muestreo);
                    _context.SaveChanges();
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

        public void enviar(short? idAgen_Session, string cod_Prod, short? cod_Campo, string tipo_correo)
        {
            try
            {
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Tipo == "P");
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Tipo == "C");
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Tipo == "I");

                correo_p = email_p.correo;

                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);

                var muestreo = _context.ProdMuestreo.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).FirstOrDefault();
                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);

                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo && x.Cod_Empresa == 2);

                    if (sesion.IdRegion == 3)
                    {
                        item.IdAgenC = 168;
                        correo_c = "juan.mares@giddingsfruit.mx";
                    }
                    if (sesion.IdRegion == 1 || sesion.IdRegion==8)
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
                        var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo && x.Cod_Empresa == 2);
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

                    if (tipo_correo == "nuevo")
                    {
                        var Inicio_cosecha = String.Format("{0:d}", muestreo.Inicio_cosecha);
                        if (cod_Prod=="99999") {
                            correo.To.Add("marholy.martinez@giddingsfruit.mx");
                        }
                        else { 
                        if (sesion.Tipo == "P")
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_c);
                            if (sesion.IdAgen == 158 || sesion.IdAgen == 173)
                            {
                                if (correo_i != "maria.lopez@giddingsfruit.mx")
                                {
                                    correo.CC.Add("maria.lopez@giddingsfruit.mx");
                                }
                                if (correo_i != "maria.jimenez@giddingsfruit.mx")
                                {
                                    correo.CC.Add("maria.jimenez@giddingsfruit.mx");
                                }
                            }
                            else
                            {
                                correo.CC.Add(correo_i);
                            }
                        }
                        else if (sesion.Tipo == "I")
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_p);
                        }
                        else if (sesion.Tipo == "C")
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_i);
                            correo.CC.Add(correo_p);

                            if (sesion.IdRegion == 1)
                            {
                                correo.CC.Add("marco.velazquez@giddingsfruit.mx");
                            }

                            if (sesion.IdRegion == 3)
                            {
                                if (correo_c != "juan.mares@giddingsfruit.mx")
                                {
                                    correo.CC.Add("juan.mares@giddingsfruit.mx");
                                }
                                correo.CC.Add("genaro.morales@giddingsfruit.mx");
                            }
                        }

                        correo.CC.Add("oscar.castillo@giddingsfruit.mx");
                    }

                        correo.Subject = "Nuevo Muestreo: " + campo.Cod_Prod;
                        correo.Body = "Solicitado por: " + sesion.Completo + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Productor: " + campo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Campo: " + campo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Inicio de cosecha: " + Inicio_cosecha + "<br/>";

                    }

                    else if (tipo_correo == "Muestreo Liberado")
                    {
                        if (cod_Prod == "99999")
                        {
                            correo.To.Add("marholy.martinez@giddingsfruit.mx");
                        }
                        else {
                            correo.To.Add(sesion.correo);//correo_p
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_i);
                        }
                       
                        correo.Subject = "Muestreo Liberado: " + cod_Prod;
                        correo.Body = "Liberado por: " + sesion.Completo + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Ubicacion: " + campo.Ubicacion + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                        correo.Body += " <br/>";
                    }

                    else if (tipo_correo == "fecha_ejecucion")
                    {
                        if (cod_Prod == "99999")
                        {
                            correo.To.Add("marholy.martinez@giddingsfruit.mx");
                        }
                        else
                        {
                            correo.To.Add(sesion.correo);//correo_p
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_p);
                        }

                        correo.Subject = "Fecha ejecucion agregada: " + cod_Prod;
                        correo.Body = "Agregada por: " + sesion.Completo + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Ubicacion: " + campo.Ubicacion + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Fecha: " + muestreo.Fecha_ejecucion + "<br/>";
                    }

                    else if (tipo_correo == "Liberar tarjeta ")
                    {
                        if (cod_Prod == "99999")
                        {
                            correo.To.Add("marholy.martinez@giddingsfruit.mx");
                        }
                        else
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_p);
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_i);
                        }

                        correo.Subject = "Entrega de tarjeta: " + cod_Prod;
                        correo.Body = "Autorizado por: " + sesion.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "Se ha autorizado la liberación para entrega de tarjeta del productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "campo: " + cod_Campo + " - " + campo.Descripcion + " ubicado en " + campo.Ubicacion + " <br/>";
                        correo.Body += " <br/>";
                        if (calidad_muestreo.Estatus == "1")
                        {
                            correo.Body += "Evaluacion de calidad: APTA <br/>";
                        }
                        else if (calidad_muestreo.Estatus == "2")
                        {
                            correo.Body += "Evaluacion de calidad: APTA CON CONDICIONES<br/>";
                        }
                        else if (calidad_muestreo.Estatus == "3")
                        {
                            correo.Body += "Evaluacion de calidad: PENDIENTE <br/>";
                        }
                        correo.Body += " <br/>";
                        if (calidad_muestreo != null)
                        {
                            correo.Body += "Incidencias encontradas: " + calidad_muestreo.Incidencia + " <br/>";
                            correo.Body += " <br/>";
                            if (calidad_muestreo.Propuesta != "")
                            {
                                correo.Body += "Propuesta: " + calidad_muestreo.Propuesta + " <br/>";
                                correo.Body += " <br/>";
                            }
                        }

                        if (muestreo.Liberar_Tarjeta != "")
                        {
                            correo.Body += "Justificación: " + muestreo.Liberar_Tarjeta + "<br/>";
                            correo.Body += " <br/>";
                        }
                        if (analisis != null)
                        {
                            if (analisis.Estatus == "R")
                            {
                                correo.Body += "Resultado del analisis: CON RESIDUOS <br/>";
                            }
                            else if (analisis.Estatus == "P")
                            {
                                correo.Body += "Resultado del analisis: EN PROCESO <br/>";
                            }
                            else if (analisis.Estatus == "F")
                            {
                                correo.Body += "Resultado del analisis: FUERA DE LIMITE <br/>";
                            }
                            else if (analisis.Estatus == "L")
                            {
                                correo.Body += "Resultado del analisis: LIBERADO <br/>";
                            }
                            correo.Body += " <br/>";
                        }
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