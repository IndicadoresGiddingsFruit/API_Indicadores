
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using ApiIndicadores.Models;
using ApiIndicadores.Classes;
using ApiIndicadores.Context;

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

        // GET: Estatus de fiananciamiento
        [HttpGet]
        public async Task<ActionResult<EstatusFinanciamiento>> Get()
        {
            try
            {
                return Ok(await _context.EstatusFinanciamiento.ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // GET api/<SeguimientoController>/5 
        [HttpGet("{id}/{idAgen}")]
        public ActionResult<SeguimientoClass> Get(int id, short idAgen)
        {
            try
            {
                var item = (dynamic)null;
                var item2 = (dynamic)null;
                if (id == 352)
                {
                    item = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento null, -1, null").ToList();
                    item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', -1, null").ToList();
                }

                else if (id == 394)
                {
                    item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', -1, 'S'").ToList();
                }

                //zona LOS REYES - URUAPAN - ZAMORA - IRAPUATO - ARANDAS
                else if (id == 188)
                {
                    item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + ", null").ToList();
                }

                //zona JALISCO
                else if (id == 44)
                {
                    item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + ", null").ToList();
                }

                else
                {
                    item2 = _context.SeguimientoClass.FromSqlRaw($"sp_Seguimiento_Financiamiento 'S', " + idAgen + ", null").ToList();
                }

                var tuple = Tuple.Create((List<SeguimientoClass>)item, (List<SeguimientoClass>)item2);
                return Ok(tuple);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST Agregar nuevo código
        [HttpPost]
        public async Task<ActionResult<Seguimiento_financ>> Post([FromBody] Seguimiento_financ model)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now.Date >= m.Inicio && DateTime.Now.Date <= m.Fin);
                model.Cod_Empresa = 2;
                model.Fecha = DateTime.Now;
                model.Temporada = catSemanas.Temporada;

                _context.Seguimiento_financ.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // PUT Agregar Estatus y Comentarios
        [HttpPut("{id}")]
        public async Task<ActionResult<Seguimiento_financ>> Put(int id, [FromBody] Seguimiento_financ model)
        {
            try
            {
                var item = _context.Seguimiento_financ.Where(x => x.Id == id).First();
                if (model.Estatus > 0)
                {
                    item.Estatus = model.Estatus;
                }
                item.Comentarios = model.Comentarios;
                item.Fecha_Up = DateTime.Now;
                await _context.SaveChangesAsync();

                title = "Código: " + item.Cod_Prod;
                body = "Estatus: " + model.Estatus;

                notificaciones.SendNotificationJSON(title, body);
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Patch: Enviar correos
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

                //Atencion a productores
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

                else if(recipient== "Enviar a ingenieros")
                {
                    foreach (var x in model)
                    {
                        var item = _context.Seguimiento_financ.Where(i => i.Id == x.Id).First();
                        item.AP = "S";
                        await _context.SaveChangesAsync();
                    }

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


                        email.sendmail(agente.correo, agente.IdRegion, lista);

                        title = "Asignar estatus de financiamientos";
                        body = "Tiene nuevos códigos a revisar";

                        notificaciones.SendNotificationJSON(title, body);
                    }
                }

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: Eliminar registro
        [HttpDelete("{id}")]
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
