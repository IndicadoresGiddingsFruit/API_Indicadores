using ApiIndicadores.Context;
using Indicadores.Context;
using Indicadores.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AppDBContextRH _contextRH;

        public UsuariosController(AppDbContext context, AppDBContextRH contextRH) {
            _context = context;
            _contextRH = contextRH;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SIPGUsuarios>>> GetUsuarios() 
        {
            return await _context.SIPGUsuarios.OrderBy(u=>u.Completo).ToListAsync();    
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SIPGUsuarios>> GetUsuarios(int id) {
            var usuarios = await _context.SIPGUsuarios.FindAsync(id);
            if (usuarios == null) {
                return NotFound();
            }
            return usuarios;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(int id, SIPGUsuarios usuarios) {
            if (id != usuarios.Id) {
                return BadRequest();
            }
            _context.Entry(usuarios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!UsuariosExist(id))
                {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            return NoContent();
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
        public ActionResult<List<SIPGUsuarios>> GetIniciasSesion(string username, string password)
        {
            var usuarios =  _context.SIPGUsuarios.Where(u=>u.Nombre.Equals(username) && u.Clave.Equals(password)).ToList();
            if (usuarios == null)
            {
                return NotFound();
            }
            return usuarios;
        }

        private bool UsuariosExist(int id) {
            return _context.SIPGUsuarios.Any(e=>e.Id==id);
        }
    }

}
