using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ApiIndicadores.Controllers.Auditoria
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdLogAuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdLogAuditoriaController(AppDbContext context)
        {
            _context = context;
        }

        //Puntos de control de acciones correctivas por id de auditoria
        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {             
                var item = _context.AccionesCorrectivasClass.FromSqlRaw($"GetLogAccionesCorrectivas " + IdProdAuditoria + "").ToList();

                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Puntos de control por id y id de auditoria
        [HttpGet("{IdCatAuditoria}/{IdProdAuditoria}")]
        public ActionResult Get(int IdCatAuditoria, int IdProdAuditoria)
        {
            try
            {
                var item = _context.ProdLogAuditoria.Where(x => x.IdCatAuditoria == IdCatAuditoria && x.IdProdAuditoria == IdProdAuditoria).Distinct();
                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Agregar respuesta por punto de control
        [HttpPost]
        public async Task<ActionResult<ProdLogAuditoria>> Post([FromBody] ProdLogAuditoria model)
        {
            try
            {
                var auditoriaExiste = _context.ProdLogAuditoria.FirstOrDefault(x =>
                x.IdProdAuditoria == model.IdProdAuditoria &&
                x.IdCatAuditoria == model.IdCatAuditoria);

                if (auditoriaExiste == null)
                {
                    model.Fecha = DateTime.Now;
                    _context.ProdLogAuditoria.Add(model);
                    await _context.SaveChangesAsync();

                    if (model.Opcion == "NO")
                    {
                        var accion_correctivaExiste = _context.ProdLogAccionesCorrectivas.FirstOrDefault(x =>
                    x.IdLogAuditoria == model.Id);

                        if (accion_correctivaExiste == null)
                        {
                            var prodLogAccionesCorrectivas = new ProdLogAccionesCorrectivas();

                            prodLogAccionesCorrectivas.IdLogAuditoria = model.Id;
                            prodLogAccionesCorrectivas.Justificacion = model.Justificacion;
                            prodLogAccionesCorrectivas.Fecha = DateTime.Now;
                            _context.ProdLogAccionesCorrectivas.Add(prodLogAccionesCorrectivas);
                            await _context.SaveChangesAsync();

                            return Ok(model);
                        }
                        else
                        {
                            return BadRequest("El registro ya existe");
                        }
                    }

                    else
                    {
                        return Ok(model);
                    }
                }
                else
                {
                    return BadRequest("El registro ya existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Gardar respuesta por varios punto de control.
        //Opcion es SI o NA 
        [HttpPost("{opcion}")]
        public async Task<ActionResult<ProdLogAuditoria>> Post(string opcion, [FromBody] List<ProdLogAuditoria> model)
        {
            try
            {
                if (model != null)
                {
                    foreach (var m in model)
                    {
                        if (m != null)
                        {
                            var auditoriaExiste = _context.ProdLogAuditoria.FirstOrDefault(x => x.IdProdAuditoria == m.IdProdAuditoria && x.IdCatAuditoria == m.IdCatAuditoria);

                            if (auditoriaExiste == null)
                            {
                                var justificacion = _context.ProdAudInocCat.FirstOrDefault(x => x.Id == m.IdCatAuditoria);

                                m.Fecha = DateTime.Now;
                                m.Opcion = opcion;
                                m.Justificacion = justificacion.Justificacion;
                                _context.ProdLogAuditoria.Add(m);
                                await _context.SaveChangesAsync();

                                if (opcion == "NO")
                                {
                                    var accion_correctivaExiste = _context.ProdLogAccionesCorrectivas.FirstOrDefault(x => x.IdLogAuditoria == m.Id);

                                    if (accion_correctivaExiste == null)
                                    {
                                        var prodLogAccionesCorrectivas = new ProdLogAccionesCorrectivas();

                                        prodLogAccionesCorrectivas.IdLogAuditoria = m.Id;
                                        prodLogAccionesCorrectivas.Justificacion = justificacion.Justificacion;
                                        prodLogAccionesCorrectivas.Fecha = DateTime.Now;
                                        _context.ProdLogAccionesCorrectivas.Add(prodLogAccionesCorrectivas);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                            //else
                            //{
                            //    return BadRequest("El registro ya existe");
                            //}
                        }
                    }
                    return Ok(model);
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

        //Actualizar acciones correctivas a SI y editar puntos de control
        [HttpPut("{IdLog}")]
        public async Task<ActionResult<ProdLogAuditoria>> Put(int IdLog, [FromBody] ProdLogAuditoria model)
        {
            try
            {
                var auditoriaExiste = (dynamic)null;
                if (IdLog == 0)
                {
                    auditoriaExiste = _context.ProdLogAuditoria.FirstOrDefault(x =>
                    x.IdCatAuditoria == model.IdCatAuditoria && x.IdProdAuditoria == model.IdProdAuditoria);
                }
                else
                {
                    auditoriaExiste = _context.ProdLogAuditoria.FirstOrDefault(x => x.Id == IdLog);
                }

                if (auditoriaExiste != null)
                {
                    auditoriaExiste.Fecha = DateTime.Now;
                    auditoriaExiste.Opcion = model.Opcion;
                    auditoriaExiste.Justificacion = model.Justificacion;
                    await _context.SaveChangesAsync();

                    if (model.Opcion == "NO")
                    {
                        var accion_correctivaExiste = _context.ProdLogAccionesCorrectivas.FirstOrDefault(x =>
                    x.IdLogAuditoria == IdLog);

                        if (accion_correctivaExiste == null)
                        {
                            var prodLogAccionesCorrectivas = new ProdLogAccionesCorrectivas();
                            prodLogAccionesCorrectivas.IdLogAuditoria = IdLog;
                            prodLogAccionesCorrectivas.Justificacion = model.Justificacion;
                            prodLogAccionesCorrectivas.Fecha = DateTime.Now;
                            _context.ProdLogAccionesCorrectivas.Add(prodLogAccionesCorrectivas);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            accion_correctivaExiste.IdLogAuditoria = IdLog;
                            accion_correctivaExiste.Justificacion = model.Justificacion;
                            accion_correctivaExiste.Fecha = DateTime.Now;
                            await _context.SaveChangesAsync();
                        }
                    }

                    else if (model.Opcion == "SI")
                    {
                        var accion_correctivaExiste = _context.ProdLogAccionesCorrectivas.FirstOrDefault(x => x.IdLogAuditoria == IdLog);
                        if (accion_correctivaExiste != null)
                        {
                            _context.ProdLogAccionesCorrectivas.Remove(accion_correctivaExiste);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            //Revisar cuantas AccionesCorrectivas faltan para dar por terminada la auditoria 
                            var accionesCorrectivasFaltantes = (from a in _context.ProdLogAccionesCorrectivas
                                                                join l in _context.ProdLogAuditoria on a.IdLogAuditoria equals l.Id
                                                                where l.IdProdAuditoria == model.IdProdAuditoria && l.Opcion == "NO"
                                                                group a by new
                                                                {
                                                                    Faltantes = a.Id
                                                                } into x
                                                                select new
                                                                {
                                                                    Faltantes = x.Key.Faltantes,
                                                                }).Count();

                            //Revisar cuantas fotos de las AccionesCorrectivas faltan para dar por terminada la auditoria 
                            var accionesCorrectivasFotos = (from a in _context.ProdLogAccionesCorrectivas
                                                            join l in _context.ProdLogAuditoria on a.IdLogAuditoria equals l.Id
                                                            join f in _context.ProdAuditoriaFoto on a.Id equals f.IdLogAC
                                                            where l.IdProdAuditoria == model.IdProdAuditoria && f.Ruta == null
                                                            group a by new
                                                            {
                                                                Faltantes = f.Id
                                                            } into x
                                                            select new
                                                            {
                                                                Faltantes = x.Key.Faltantes,
                                                            }).Count();

                            //Revisar que no haya acciones correctivas pendientes
                            if (accionesCorrectivasFaltantes == 0 && accionesCorrectivasFotos == 0)
                            {
                                var auditoria = _context.ProdAudInoc.Find(model.IdProdAuditoria);
                                //Revisar que se haya finalizado la auditoria
                                if (auditoria.Fecha_termino != null)
                                {
                                    auditoria.Finalizada = 1;
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    return Ok(model);

                }
                else
                {
                    return BadRequest("El registro no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
