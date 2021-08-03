
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using ApiIndicadores.Models;
using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguimientoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeguimientoController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        Email email = new Email();
        string title = "", body = "";

        // GET: api/<SeguimientoController>
        [HttpGet]
        public async Task<ActionResult<Seguimiento_financ>> Get()
        {
            try
            {
                return Ok(await _context.Seguimiento_financ.ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // GET api/<SeguimientoController>/5 
        [HttpGet("{id}/{idAgen}", Name = "GetSeguimiento")]
        public Tuple<List<SeguimientoClass>, List<SeguimientoClass>> Get(int id, short idAgen)
        {
            var item = (dynamic)null;
            var item2 = (dynamic)null;
            if (id == 391)
            {
                item = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento null, -1").ToList();
                item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', -1").ToList();
            }
            else if (id == 394)
            {
                item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', -1, 'S'").ToList();
            }
            //zona LR-UR-ZM-IR
            else if (id == 188)
            {
                item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + "").ToList();
            }
            //zona JL
            else if (id == 44)
            {
                item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + "").ToList();
            }
            else
            {
                item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + "").ToList();
            }
            var tuple = Tuple.Create<List<SeguimientoClass>, List<SeguimientoClass>>(item, item2);
            return tuple;
        }

        // POST api/<SeguimientoController>
        [HttpPost]
        public async Task<ActionResult<Seguimiento_financ>> Post([FromBody] Seguimiento_financ model)
        {
            try
            {
                model.Cod_Empresa = 2;
                model.Fecha = DateTime.Now;
                _context.Seguimiento_financ.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // PUT api/<SeguimientoController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Seguimiento_financ>> Put(int id, [FromBody] Seguimiento_financ model)
        {
            try
            {
                var item = _context.Seguimiento_financ.Where(x => x.Id == id).First();
                if (model.Estatus.Length == 1)
                {
                    item.Estatus = model.Estatus;
                }
                item.Comentarios = model.Comentarios;
                item.Fecha_Up = DateTime.Now;
                await _context.SaveChangesAsync();

                title = "Código: " + item.Cod_Prod;
                body = "Estatus modificado";

                notificaciones.SendNotificationJSON(title, body);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{recipient}")]
        public async Task<ActionResult<Seguimiento_financ>> Patch([FromBody] List<SeguimientoClass> model, string recipient = "")
        {
            try
            {               

                foreach (var x in model)
                {
                    var item = _context.Seguimiento_financ.Where(i => i.Id == x.Id).First();
                    item.Enviado = "S";
                    await _context.SaveChangesAsync();
                }

                if (recipient != "Enviar sin correo")
                {
                    if (recipient == "AP")
                    {
                        foreach (var x in model)
                        {
                            var item = _context.Seguimiento_financ.Where(i => i.Id == x.Id).First();
                            item.AP = "S";
                            await _context.SaveChangesAsync();
                        }

                        email.sendmail("ademir.reyes@giddingsfruit.mx", 0, null);
                    }

                    else
                    {
                        var result = (from y in model
                                      group y by new { y.IdAgen } into g
                                      select new
                                      {
                                          IdAgen = g.Key.IdAgen
                                      }).ToList();

                        var q = result.AsQueryable();

                        foreach (var x in q)
                        {
                            var agente = _context.SIPGUsuarios.FirstOrDefault(a => a.IdAgen == x.IdAgen);

                            var lista = (from y in model
                                         where y.IdAgen == x.IdAgen
                                         group y by new
                                         {
                                             y.Cod_Prod,
                                             y.Productor,
                                             y.Estatus,
                                             y.DescEstatus,
                                             y.Comentarios,
                                             y.SaldoFinal,
                                             y.caja1,
                                             y.caja2
                                         } into g
                                         select new SeguimientoClass()
                                         {
                                             Cod_Prod = g.Key.Cod_Prod,
                                             Productor = g.Key.Productor,
                                             Estatus = g.Key.Estatus,
                                             DescEstatus = g.Key.DescEstatus,
                                             Comentarios = g.Key.Comentarios,
                                             SaldoFinal = g.Key.SaldoFinal,
                                             caja1 = g.Key.caja1,
                                             caja2 = g.Key.caja2
                                         }).ToList();


                            //email.sendmail(agente.correo, agente.IdRegion, lista);

                            title = "Asignar estatus de financiamientos";
                            body = "Usted tiene nuevos códigos a revisar";

                            notificaciones.SendNotificationJSON(title, body);
                        }
                    }
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        // DELETE api/<SeguimientoController>/5  
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Seguimiento_financ>> Delete(int id)
        {
            try
            {
                var model = _context.Seguimiento_financ.Find(id);

                if (model == null)
                {
                    return NotFound();
                }

                _context.Seguimiento_financ.Remove(model);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
