using Indicadores.Classes;
using Indicadores.Context;
using Indicadores.Models;
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

        // GET api/<CalidadMuestreoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CalidadMuestreoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CalidadMuestreoController>/5
        [HttpPut("{id}/{idAgen}")]
        public ActionResult Put(int id, short idAgen, [FromBody] ProdCalidadMuestreo model)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);

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
                        }
                        else if (model.Estatus == "2")
                        {
                            muestreo.Tarjeta = "N";
                        }
                        else if (model.Estatus == "3")
                        {
                            muestreo.Tarjeta = "N";
                        }

                        _context.SaveChanges();

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Calidad evaluada: estatus " + model;
                    }

                    notificaciones.SendNotificationJSON(title, body);
                    _context.SaveChanges();
                    return Ok();
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

        // DELETE api/<CalidadMuestreoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
