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
