using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";

        // GET: api/<MuestreoController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdCamposCat.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        // GET api/<CamposController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CamposController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CamposController>/5
        //Reasignar codigo
        [HttpPut("{idAgen}/{tipo}")]
        public ActionResult Put(short idAgen, string tipo, [FromBody] ProdMuestreo model)
        {
            try
            {
                var model_campo = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == model.Cod_Prod && x.Cod_Campo == model.Cod_Campo);
                if (tipo == "P")
                {
                    model_campo.IdAgen = idAgen;
                }
                else if (tipo == "C")
                {
                    model_campo.IdAgenC = idAgen;
                }
                else if (tipo == "I")
                {
                    model_campo.IdAgenI = idAgen;
                }

                title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                body = "Se le ha asignado un nuevo código";
                _context.SaveChanges();
                notificaciones.SendNotificationJSON(title, body);
                return CreatedAtRoute("GetMuestreo", new { id = 0, idAgen = idAgen }, model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<CamposController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
