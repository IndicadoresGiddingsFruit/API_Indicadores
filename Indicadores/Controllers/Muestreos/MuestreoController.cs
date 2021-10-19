using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        //// GET api/<MuestreoController>/5
        //[HttpGet("{idMuestreo}")]       
        //public ActionResult Get(int idMuestreo)
        //{
        //    try
        //    {
        //        var muestreos = _context.MuestreosClass.FromSqlRaw($"sp_GetMuestreos,NULL,NULL,NULL, " + idMuestreo + "").Distinct();
        //        return Ok(muestreos);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);

        //    }
        //}

        // GET api/<MuestreoController>/5
        [HttpGet("{id}/{tipo}/{depto}")]
        //id es IdAgen o id de usuario        
        public async Task<ActionResult<MuestreosClass>> Get(short id, string tipo = null, string depto = null)
        {
            try
            {
                var muestreos = _context.MuestreosClass.FromSqlRaw($"sp_GetMuestreos " + id + "," + tipo + "," + depto + "").ToListAsync();
                //var muestreos = _context.MuestreosClass.FromSqlRaw($"MuestreosPruebas").ToListAsync();
                return Ok(await muestreos);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [HttpGet("{IdMuestreo}")]
        //imagen
        public ActionResult obtenerFile(int IdMuestreo = 0)
        {
            string ruta = "//192.168.0.21/recursos season/AutorizaTarjeta/" + IdMuestreo + ".jpg";
            string Imagen = getFile(ruta);
            return Ok(Imagen);
        }

        public string getFile(string ruta)
        {
            try
            {
                byte[] bytesImagen = System.IO.File.ReadAllBytes(ruta);
                string imagenBase64 = Convert.ToBase64String(bytesImagen);
                string tipoContenido;
                switch (Path.GetExtension(ruta))
                {
                    case ".jpg":
                        {
                            tipoContenido = "image/jpg";
                            break;
                        }
                    default:
                        {
                            return null;
                        }
                }
                return string.Format("data:{0};base64,{1}", tipoContenido, imagenBase64);
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }

        // Nuevo  
        [HttpPost]
        public ActionResult Post([FromBody] ProdMuestreo model)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now.Date >= m.Inicio && DateTime.Now.Date <= m.Fin);
                var agente = _context.SIPGUsuarios.FirstOrDefault(s => s.IdAgen == model.IdAgen);
                var campos = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo);

                //var modeloExistente = _context.ProdMuestreo.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Temporada == catSemanas.Temporada);
                //if (modeloExistente == null)
                //{
                var usuario = _context.SIPGUsuarios.Where(x => x.IdAgen == model.IdAgen).First();

                if (usuario.Depto == "P" || model.IdAgen == 216)
                {
                    model.Liberacion = "S";
                }
                model.Fecha_solicitud = DateTime.Now;
                model.Temporada = catSemanas.Temporada;
                if (agente.Depto != "P")
                {
                    model.IdAgen = (short)campos.IdAgen;
                }
                _context.ProdMuestreo.Add(model);
                _context.SaveChanges();

                title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                body = "Nuevo muestreo";
                notificaciones.SendNotificationJSON(title, body);

                enviar(model.IdAgen, model.Id, "nuevo");

                return Ok(model);
                //}
                //else
                //{
                //    return BadRequest("La solicitud ya existe");
                //}
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Editar muestreo  y añadir fecha de ejecución
        [HttpPut("{id}/{idAgen}/{sector}")]
        public ActionResult Put(int id, short idAgen, short sector, [FromBody] ProdMuestreo model)
        {
            try
            {
                ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                var item = _context.ProdMuestreo.Find(id);
                if (item.Id == id)
                {
                    //fecha_ejecucion  
                    if (sector != 0)
                    {
                        var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                        var model_sectorMuestreo = _context.ProdMuestreoSector.Where(x => x.Cod_Prod == item.Cod_Prod && x.Cod_Campo == item.Cod_Campo && x.Sector == sector).FirstOrDefault();

                        if (model_sectorMuestreo != null)
                        {
                            var valida_temporada = _context.ProdMuestreo.Where(x => x.Id == model_sectorMuestreo.IdMuestreo).FirstOrDefault();

                            if (valida_temporada.Temporada == catSemanas.Temporada)
                            {
                                return BadRequest("El sector ya fue muestreado");
                            }
                            else
                            {
                                prodMuestreoSector.Cod_Prod = item.Cod_Prod;
                                prodMuestreoSector.Cod_Campo = item.Cod_Campo;
                                prodMuestreoSector.Sector = sector;
                                prodMuestreoSector.IdMuestreo = id;
                                _context.ProdMuestreoSector.Add(prodMuestreoSector);
                                _context.SaveChanges();

                                item.IdAgenI = idAgen;
                                if (item.Fecha_ejecucion == null)
                                {
                                    item.Fecha_ejecucion = model.Fecha_ejecucion;

                                    var Fecha_ejecucion = String.Format("{0:d}", model.Fecha_ejecucion);

                                    title = "Código: " + item.Cod_Prod + " campo: " + item.Cod_Campo;
                                    body = "Fecha de ejecución: " + Fecha_ejecucion;
                                    notificaciones.SendNotificationJSON(title, body);

                                    enviar(idAgen, item.Id, "fecha_ejecucion");
                                }
                                _context.SaveChanges();

                                return Ok(model);
                            }
                        }
                        else
                        {
                            prodMuestreoSector.Cod_Prod = item.Cod_Prod;
                            prodMuestreoSector.Cod_Campo = item.Cod_Campo;
                            prodMuestreoSector.Sector = sector;
                            prodMuestreoSector.IdMuestreo = id;
                            _context.ProdMuestreoSector.Add(prodMuestreoSector);
                            _context.SaveChanges();

                            item.IdAgenI = idAgen;
                            if (item.Fecha_ejecucion == null)
                            {
                                item.Fecha_ejecucion = model.Fecha_ejecucion;

                                var Fecha_ejecucion = String.Format("{0:d}", model.Fecha_ejecucion);

                                title = "Código: " + item.Cod_Prod + " campo: " + item.Cod_Campo;
                                body = "Fecha de ejecución: " + Fecha_ejecucion;
                                notificaciones.SendNotificationJSON(title, body);

                                enviar(idAgen, item.Id, "fecha_ejecucion");
                            }
                            _context.SaveChanges();

                            return Ok(model);
                        }
                    }

                    //editar muestreo
                    else
                    {
                        item.Cod_Campo = model.Cod_Campo;
                        item.Inicio_cosecha = model.Inicio_cosecha;
                        item.Telefono = model.Telefono;
                        item.CajasEstimadas = model.CajasEstimadas;
                        _context.SaveChanges();
                        return Ok(model);
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Liberar solicitud - Autorizar tarjeta - Bloqueo Tarjeta
        [HttpPatch("{id}/{idAgen}/{opcion}")]
        public ActionResult Patch(int id, short idAgen, string opcion, [FromBody] ProdMuestreo model)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);

                if (muestreo.Id == id)
                {
                    if (opcion == "liberacion")
                    {
                        muestreo.Liberacion = "S";
                        muestreo.Fecha_solicitud = DateTime.Now;
                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Solicitud de muestreo liberada";

                        notificaciones.SendNotificationJSON(title, body);
                        _context.SaveChanges();

                        enviar(idAgen, muestreo.Id, "Muestreo Liberado");
                    }

                    else if (opcion == "tarjeta")
                    {
                        muestreo.Tarjeta = "S";
                        if (idAgen == 281 || idAgen == 204 || idAgen == 352 || idAgen == 1)
                        {
                            muestreo.IdAgen_Tarjeta = idAgen;
                        }
                        muestreo.Liberar_Tarjeta = model.Liberar_Tarjeta;

                        //guardar fecha de liberacion
                        _context.SaveChanges();

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Entrega de tarjeta autorizada";
                        notificaciones.SendNotificationJSON(title, body);

                        enviar(idAgen, muestreo.Id, "Tarjeta");
                    }

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

        [HttpPatch("{id}")]
        public ActionResult PatchT(int id, [FromForm] IFormFile file)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);
                if (muestreo.Id == id)
                {
                    if (file != null)
                    {
                        var extension = Path.GetExtension(file.FileName).Substring(1);
                        var path = "//192.168.0.21/recursos season/AutorizaTarjeta/" + id + ".jpg";

                        using (var stream = System.IO.File.Create(path))
                        {
                            file.CopyToAsync(stream);
                        }

                        muestreo.ImageAutoriza = path;
                    }
                    _context.SaveChanges();
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

        //Eliminar
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

        //correo
        public void enviar(short idAgen_Session, int idMuestreo, string tipo_correo)
        {
            try
            {

                var muestreo = _context.ProdMuestreo.Where(m => m.Id == idMuestreo).FirstOrDefault();
                var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == muestreo.Cod_Prod && m.Cod_Campo == muestreo.Cod_Campo);
                var localidad = _context.CatLocalidades.FirstOrDefault(m => m.CodLocalidad == campo.CodLocalidad);
                //var sectores = _context.ProdMuestreoSector.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo).ToList();
                var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen);
                var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC);
                var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI);

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

                var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
                var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);

                //Si agente de calidad es null
                if (email_c == null)
                {
                    var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == muestreo.Cod_Prod && x.Cod_Campo == muestreo.Cod_Campo && x.Cod_Empresa == 2);

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
                        var item = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == muestreo.Cod_Prod && x.Cod_Campo == muestreo.Cod_Campo && x.Cod_Empresa == 2);

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

                if (tipo_correo == "nuevo")
                {
                    var Inicio_cosecha = String.Format("{0:d}", muestreo.Inicio_cosecha);
                    if (muestreo.Cod_Prod == "99999")
                    {
                        correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    }

                    else
                    {
                        //producción
                        if (sesion.Depto == "P")
                        {
                            correo.To.Add(sesion.correo);

                            //Ziracuaretiro
                            if (sesion.IdAgen == 158 || sesion.IdAgen == 173)
                            {
                                if (correo_c != "maria.lopez@giddingsfruit.mx")
                                {
                                    correo.CC.Add("maria.lopez@giddingsfruit.mx");
                                }

                                else
                                {
                                    correo.CC.Add(correo_c);
                                }

                                if (correo_i != "maria.jimenez@giddingsfruit.mx")
                                {
                                    correo.CC.Add("maria.jimenez@giddingsfruit.mx");
                                }

                                else
                                {
                                    correo.CC.Add(correo_i);
                                }
                            }

                            //Todas
                            else
                            {
                                correo.CC.Add(correo_c);

                                if (email_c.IdAgen == 29)
                                {
                                    correo.CC.Add("judith.santiago@giddingsfruit.mx");
                                    correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                                }

                                correo.CC.Add(correo_i);

                                //Arandas
                                if (sesion.IdAgen == 197)
                                {
                                    correo.CC.Add("hector.torres@giddingsfruit.mx");
                                }
                            }
                        }

                        //inocuidad 
                        else if (sesion.Depto == "I")
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_c);
                            if (email_c.IdAgen == 29)
                            {
                                correo.CC.Add("judith.santiago@giddingsfruit.mx");
                                correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                            }
                            correo.CC.Add(correo_p);

                            //Arandas
                            //if (sesion.IdAgen == 211)
                            //{
                            //    correo.CC.Add("hector.torres@giddingsfruit.mx");
                            //}
                        }

                        //calidad
                        else if (sesion.Depto == "C")
                        {
                            correo.To.Add(sesion.correo);
                            if (email_c.IdAgen == 29)
                            {
                                correo.CC.Add("judith.santiago@giddingsfruit.mx");
                                correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                            }
                            correo.CC.Add(correo_i);
                            correo.CC.Add(correo_p);
                        }

                        //------------------------------------------------------------------------------------------------------------------------------
                        //zona Los Reyes copia a Mayra
                        if (email_p.IdRegion == 1 && email_p.IdAgen != 197)
                        {
                            correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                        }

                        //zona Jalisco copia a Daniel
                        if (email_p.IdRegion == 2)
                        {
                            correo.CC.Add("jose.partida@giddingsfruit.mx");
                        }

                        //zona Uruapan 
                        if (email_p.IdRegion == 3)
                        {
                            if (email_c.IdAgen != 168)
                            {
                                correo.CC.Add("juan.mares@giddingsfruit.mx");
                            }

                            if (sesion.correo == "angel.hernandez@giddingsfruit.mx")
                            {
                                correo.CC.Add("hector.torres@giddingsfruit.mx");
                            }
                            correo.CC.Add("genaro.morales@giddingsfruit.mx");
                        }

                        //zona Zamora 
                        if (email_p.IdRegion == 4)
                        {
                            correo.CC.Add("josefina.cervantes@giddingsfruit.mx");
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
                    correo.Body += " <br/>";
                    correo.Body += "Cajas estimadas: " + muestreo.CajasEstimadas + "<br/>";
                }

                else if (tipo_correo == "Muestreo Liberado")
                {
                    if (muestreo.Cod_Prod == "99999")
                    {
                        correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    }
                    else
                    {
                        correo.To.Add(sesion.correo);//correo_p
                        correo.CC.Add(correo_c);
                        if (email_c.IdAgen == 29)
                        {
                            correo.CC.Add("judith.santiago@giddingsfruit.mx");
                            correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                        }
                        correo.CC.Add(correo_i);
                    }

                    correo.Subject = "Muestreo Liberado: " + muestreo.Cod_Prod;
                    correo.Body = "Liberado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + muestreo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + muestreo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Ubicacion: " + campo.Ubicacion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                    correo.Body += " <br/>";
                }

                else if (tipo_correo == "fecha_ejecucion")
                {
                    var Fecha_ejecucion = String.Format("{0:d}", muestreo.Fecha_ejecucion);
                    if (muestreo.Cod_Prod == "99999")
                    {
                        correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    }

                    else
                    {
                        correo.To.Add(sesion.correo);//correo_i                          
                        correo.CC.Add(correo_c);
                        if (email_c.IdAgen == 29)
                        {
                            correo.CC.Add("judith.santiago@giddingsfruit.mx");
                            correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                        }
                        correo.CC.Add(correo_p);

                        if (sesion.correo != correo_i)
                        {
                            correo.CC.Add(correo_i);
                        }
                    }

                    correo.Subject = "Fecha ejecucion agregada: " + muestreo.Cod_Prod;
                    correo.Body = "Agregada por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + muestreo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + muestreo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Ubicacion: " + campo.Ubicacion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de ejecución: " + Fecha_ejecucion + "<br/>";
                }

                else if (tipo_correo == "Tarjeta")
                {
                    if (muestreo.Cod_Prod == "99999")
                    {
                        correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    }
                    else
                    {

                        if (sesion.Id == 352)
                        {
                            correo.To.Add(correo_p);
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_i);
                            correo.CC.Add("daniel.cervantes@giddingsfruit.mx");
                            correo.CC.Add("hector.torres@giddingsfruit.mx");
                            correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                            correo.CC.Add("godo.garcia@giddingsfruit.mx");
                            correo.CC.Add("angel.lopez@giddingsfruit.mx");
                        }

                        else
                        {
                            correo.To.Add(sesion.correo);
                            correo.CC.Add(correo_p);
                            correo.CC.Add(correo_c);
                            correo.CC.Add(correo_i);

                            if (email_c.IdAgen == 29)
                            {
                                correo.CC.Add("judith.santiago@giddingsfruit.mx");
                                correo.CC.Add("nelida.inocencio@giddingsfruit.mx");
                            }

                            //if (sesion.IdRegion == 1 && email_c.IdAgen != 167)
                            //{
                            //    if (correo_p != "aliberth.martinez@giddingsfruit.mx")
                            //    {
                            //        correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                            //    }
                            //}

                            if (sesion.IdAgen == 1)
                            {
                                correo.CC.Add("hector.torres@giddingsfruit.mx");
                                correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                                correo.CC.Add("godo.garcia@giddingsfruit.mx");
                            }

                            if (sesion.IdAgen == 153)
                            {
                                correo.CC.Add("hector.torres@giddingsfruit.mx");
                                correo.CC.Add("angel.lopez@giddingsfruit.mx");
                                correo.CC.Add("godo.garcia@giddingsfruit.mx");
                            }

                            if (sesion.IdAgen == 281)
                            {
                                correo.CC.Add("angel.lopez@giddingsfruit.mx");
                                correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                                correo.CC.Add("godo.garcia@giddingsfruit.mx");
                            }

                            if (sesion.IdAgen == 204)
                            {
                                correo.CC.Add("angel.lopez@giddingsfruit.mx");
                                correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                                correo.CC.Add("hector.torres@giddingsfruit.mx");
                            }
                        }
                    }

                    correo.Subject = "Entrega de tarjeta: " + muestreo.Cod_Prod;

                    if (sesion.Id != 352)
                    {
                        correo.Body = "Autorizado por: " + sesion.Completo + " <br/>";
                        correo.Body += " <br/>";
                    }

                    correo.Body += "Se ha autorizado la liberacion para entrega de tarjeta del productor: " + muestreo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "campo: " + muestreo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Ubicación: " + localidad.Descripcion + " <br/>";
                    correo.Body += " <br/>";

                    if (muestreo.Liberar_Tarjeta != "")
                    {
                        correo.Body += "Justificacion: " + muestreo.Liberar_Tarjeta + "<br/>";
                        correo.Body += " <br/>";
                    }

                    if (sesion.Id == 352)
                    {
                        correo.Body += "Considerar este productor como de trato delicado en su manejo <br/>";
                        correo.Body += " <br/>";
                    }


                    //if (sesion.IdAgen == 1)
                    //{
                    //    correo.Body += "Gerencia de Inocuidad y Calidad favor de proceder con la autorización <br/>";
                    //    correo.Body += " <br/>";
                    //}

                }

                else if (tipo_correo == "bloqueo")
                {
                    if (muestreo.Cod_Prod == "99999")
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

                        //if (sesion.IdRegion == 1 && email_c.IdAgen != 167)
                        //{
                        //    if (correo_p != "aliberth.martinez@giddingsfruit.mx")
                        //    {
                        //        correo.CC.Add("mayra.ramirez@giddingsfruit.mx");
                        //    }
                        //}

                        //if (sesion.IdAgen == 1)
                        //{
                        //    correo.CC.Add("hector.rodriguez@giddingsfruit.mx");
                        //    correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                        //    correo.CC.Add("godo.garcia@giddingsfruit.mx");
                        //}

                        //if (sesion.IdAgen == 153)
                        //{
                        //    correo.CC.Add("hector.rodriguez@giddingsfruit.mx");
                        //    correo.CC.Add("angel.lopez@giddingsfruit.mx");
                        //    correo.CC.Add("godo.garcia@giddingsfruit.mx");
                        //}

                        //if (sesion.IdAgen == 281)
                        //{
                        //    correo.CC.Add("angel.lopez@giddingsfruit.mx");
                        //    correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                        //    correo.CC.Add("godo.garcia@giddingsfruit.mx");
                        //}

                        //if (sesion.IdAgen == 204)
                        //{
                        //    correo.CC.Add("angel.lopez@giddingsfruit.mx");
                        //    correo.CC.Add("brenda.garibay@giddingsfruit.mx");
                        //    correo.CC.Add("hector.rodriguez@giddingsfruit.mx");
                        //}
                    }

                    correo.Subject = "Bloqueo de entrega de tarjeta: " + muestreo.Cod_Prod;
                    correo.Body = "Bloqueado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Se ha denegado la liberación para entrega de tarjeta del productor: " + muestreo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "campo: " + muestreo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Ubicación: " + localidad.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Justificacion: " + muestreo.Liberar_Tarjeta + "<br/>";
                    correo.Body += " <br/>";
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