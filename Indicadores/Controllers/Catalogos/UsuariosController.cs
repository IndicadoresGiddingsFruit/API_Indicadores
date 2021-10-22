using ApiIndicadores.Context; 
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context; 

        public UsuariosController(AppDbContext context) {
            _context = context; 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SIPGUsuarios>>> GetUsuarios() 
        {
            return await _context.SIPGUsuarios.OrderBy(u=>u.Completo).ToListAsync();    
        }

        //Cambiar contraseña
        [HttpPut]
        public async Task<ActionResult<SIPGUsuarios>> Put(SIPGUsuarios usuarios) 
        {           

            try
            {
                var model = _context.SIPGUsuarios.Find(usuarios.Id);
                if (model != null)
                {
                    model.Clave = usuarios.Clave;
                    await _context.SaveChangesAsync();
                    enviar(usuarios.Id);
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e) 
            {
               return BadRequest(e.ToString());              
            }
        }

        //Guardar token movil
        [HttpPatch]
        public async Task<ActionResult<SIPGUsuarios>> Patch(SIPGUsuarios usuarios)
        {
            try
            {
                var model = _context.SIPGUsuarios.Find(usuarios.Id);
                if (model != null)
                {
                    model.token_movil = usuarios.token_movil;
                    await _context.SaveChangesAsync();
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult<SIPGUsuarios>> Post(SIPGUsuarios usuarios) 
        {
            try
            {
                var catUsuarios = _context.CatUsuariosA.Where(u => u.Nombre == usuarios.Nombre).FirstOrDefault();
                if (catUsuarios != null)
                {
                    //var empleado = _contextRH.Empleado.ToList();//.Where(e => (e.nombre + " " + e.apellido_materno + " " + e.apellido_paterno).Contains(catUsuarios.Completo)).FirstOrDefault();
                    
                    //if (empleado != null)
                    //{
                        var sipgUsuarios = _context.SIPGUsuarios.Where(u => u.Nombre == usuarios.Nombre).FirstOrDefault();
                        if (sipgUsuarios == null)
                        {
                            //usuarios.id_empleado = empleado.id_empleado;
                            usuarios.Completo = catUsuarios.Completo;
                            _context.SIPGUsuarios.Add(usuarios);
                            await _context.SaveChangesAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("El usuario ya existe");
                        }
                    //}
                    //else
                    //{
                    //    return BadRequest("Usuario incorrecto");
                    //}
                }
                else
                {
                    return BadRequest("Usuario incorrecto");
                }
            }
            catch (Exception e) {
                return BadRequest(e.ToString());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<SIPGUsuarios>> DeleteUsuarios(int id) {
            var usuarios = await _context.SIPGUsuarios.FindAsync(id);
            if (usuarios == null) {
                return NotFound();
            }
            _context.SIPGUsuarios.Remove(usuarios);
            await _context.SaveChangesAsync();
            return usuarios;
        }

        [HttpGet("{username}/{password}")]
        public async Task<ActionResult<SIPGUsuarios>> Get(string username, string password)
        {
            try
            {
                var usuarios = _context.SIPGUsuarios.Where(u => u.Nombre.Equals(username) && u.Clave.Equals(password)).Distinct();
                if (usuarios == null)
                {
                    return NotFound();
                }
                return Ok(await usuarios.ToListAsync());
            }
            catch (Exception e) {
                return BadRequest(e.ToString());
            }
        }
        private bool UsuariosExist(int id) {
            return _context.SIPGUsuarios.Any(e=>e.Id==id);
        }

        public void enviar(int idUser)
        {
            try
            {  
                var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.Id == idUser); 
                try
                {
                    MailMessage correo = new MailMessage();
                    correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                  
                    correo.To.Add(sesion.correo);
                    correo.Subject = "Cambio de contraseña";
                    correo.Body += "Su contraseña nueva es: " + sesion.Clave + " <br/>";
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
