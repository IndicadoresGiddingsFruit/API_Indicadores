using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        //Puntos de control acciones correctivas
        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {
                var item = (from l in _context.ProdLogAccionesCorrectivas
                            join a in _context.ProdLogAuditoria on l.IdLogAuditoria equals a.Id
                            join c in _context.ProdAudInocCat on a.IdCatAuditoria equals c.Id
                            join p in _context.ProdAudInoc on a.IdProdAuditoria equals p.Id
                            join f in _context.ProdAuditoriaFoto on l.Id equals f.IdLogAC into F
                            from f in F.DefaultIfEmpty()
                            where a.IdProdAuditoria == IdProdAuditoria
                            group l by new
                            {
                                IdLogAC = l.Id,
                                IdLog = a.Id,
                                IdCatAuditoria = a.IdCatAuditoria,
                                NoPunto = c.NoPunto,
                                Nivel=c.Nivel,
                                NoPuntoDesc = c.NoPuntoDesc,
                                PuntoControl = c.PuntoControl,
                                PuntoControlDesc = c.PuntoControlDesc,
                                Justificacion = l.Justificacion,
                                Opcion = a.Opcion,
                                isOpen = false,
                                FotoAC = f.IdLogAC
                            } into x
                            select new
                            {
                                IdLogAC = x.Key.IdLogAC,
                                IdLog = x.Key.IdLog,
                                IdCatAuditoria = x.Key.IdCatAuditoria,
                                NoPunto = x.Key.NoPunto,
                                Nivel = x.Key.Nivel,
                                NoPuntoDesc = x.Key.NoPuntoDesc,
                                PuntoControl = x.Key.PuntoControl,
                                PuntoControlDesc = x.Key.PuntoControlDesc,
                                Justificacion = x.Key.Justificacion,
                                Opcion = x.Key.Opcion,
                                isOpen = x.Key.isOpen,
                                FotoAC = x.Key.FotoAC
                            }).Distinct();

                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Todos los puntos de control
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

        //Gardar varios
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

        //Actualizar acciones correctivas y puntos de control
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
                                                            where l.IdProdAuditoria == model.IdProdAuditoria && f.Ruta==null
                                                                group a by new
                                                                {
                                                                    Faltantes = f.Id
                                                                } into x
                                                                select new
                                                                {
                                                                    Faltantes = x.Key.Faltantes,
                                                                }).Count();

                            if (accionesCorrectivasFaltantes == 0 && accionesCorrectivasFotos==0)
                            {
                                var auditoria = _context.ProdAudInoc.Find(model.IdProdAuditoria);
                                if (auditoria != null)
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
