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

        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {
                var item = (from l in _context.ProdLogAuditoria
                            join c in _context.ProdAudInocCat on l.IdCatAuditoria equals c.Id
                            join a in _context.AuditoriaCat on c.IdNorma equals a.Id
                            join p in _context.ProdAudInoc on l.IdProdAuditoria equals p.Id
                            join pr in _context.ProdProductoresCat on p.Cod_Prod equals pr.Cod_Prod
                            join ca in _context.ProdCamposCat on new { p.Cod_Prod, p.Cod_Campo } equals new { ca.Cod_Prod, ca.Cod_Campo } into ProdCamposCat
                            from ca in ProdCamposCat.DefaultIfEmpty()
                            join z in _context.ProdZonasRastreoCat on p.IdZona equals z.IdZona
                            join ag in _context.ProdAgenteCat on p.IdAgen equals ag.IdAgen
                            where l.IdProdAuditoria == IdProdAuditoria 
                            group l by new
                            {
                                IdLog = l.Id,
                                IdAgen = p.IdAgen,
                                Asesor = ag.Nombre,
                                Cod_Prod = p.Cod_Prod,
                                Productor=pr.Nombre,
                                Cod_Campo = p.Cod_Campo,
                                Campo = ca.Descripcion,
                                IdZona = p.IdZona,
                                Zona = z.DescZona,
                                IdNorma = a.Id,
                                Norma = a.Norma,
                                NoPunto = c.NoPunto,
                                NoPuntoDesc = c.NoPuntoDesc,
                                PuntoControl = c.PuntoControl,
                                PuntoControlDesc = c.PuntoControlDesc,
                                Criterio = c.Criterio,
                                Nivel = c.Nivel,
                                Justificacion = c.Justificacion,
                                Opcion = l.Opcion,
                                JustificacionLog = l.Justificacion
                            } into x
                            select new
                            {
                                IdLog = x.Key.IdLog,
                                IdAgen = x.Key.IdAgen,
                                Asesor = x.Key.Asesor,
                                Cod_Prod = x.Key.Cod_Prod,
                                Productor=x.Key.Productor,
                                Cod_Campo = x.Key.Cod_Campo,
                                Campo = x.Key.Campo,
                                IdZona = x.Key.IdZona,
                                Zona = x.Key.Zona,
                                IdNorma = x.Key.IdNorma,
                                Norma = x.Key.Norma,
                                NoPunto = x.Key.NoPunto,
                                NoPuntoDesc = x.Key.NoPuntoDesc,
                                PuntoControl = x.Key.PuntoControl,
                                PuntoControlDesc = x.Key.PuntoControlDesc,
                                Criterio = x.Key.Criterio,
                                Nivel = x.Key.Nivel,
                                Justificacion = x.Key.Justificacion,
                                Opcion = x.Key.Opcion,
                                JustificacionLog = x.Key.JustificacionLog,
                            }).Distinct();

                return Ok(item.ToList());

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("{IdCatAuditoria}/{IdProdAuditoria}")]
        public ActionResult Get(int IdCatAuditoria, int IdProdAuditoria)
        {
            try
            {
                //var item = (from c in _context.ProdAudInocCat
                //            join l in _context.ProdLogAuditoria on c.Id equals l.IdCatAuditoria
                //            where l.IdCatAuditoria == IdCatAuditoria && l.IdProdAuditoria==IdProdAuditoria
                //            group l by new
                //            {
                //                IdLog=l.Id,
                //                IdNorma = c.IdNorma,
                //                Consecutivo=c.Consecutivo,
                //                IdAuditoriaCat = l.IdCatAuditoria,
                //                IdAuditoriaProd=l.IdProdAuditoria
                //            } into x
                //            select new
                //            {
                //                IdLog = x.Key.IdLog,
                //                IdNorma = x.Key.IdNorma,
                //                Consecutivo = x.Key.Consecutivo,
                //                IdAuditoriaCat = x.Key.IdAuditoriaCat,
                //                IdAuditoriaProd = x.Key.IdAuditoriaProd
                //            }).Distinct();
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
                    return Ok(model);
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

        [HttpPut]
        public async Task<ActionResult<ProdLogAuditoria>> Put([FromBody] ProdLogAuditoria model)
        {
            try
            {
                var auditoriaExiste = _context.ProdLogAuditoria.FirstOrDefault(x =>
                x.IdCatAuditoria == model.IdCatAuditoria && x.IdProdAuditoria==model.IdProdAuditoria);

                if (auditoriaExiste != null)
                {
                    auditoriaExiste.Fecha = DateTime.Now;
                    auditoriaExiste.Opcion = model.Opcion;
                    auditoriaExiste.Justificacion = model.Justificacion;
                     await _context.SaveChangesAsync();
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
