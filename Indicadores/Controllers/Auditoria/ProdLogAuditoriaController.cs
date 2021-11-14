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
        [HttpGet("{idAuditoria}")]
        public ActionResult Get(int idAuditoria)
        {
            try
            {
                var item = (from c in _context.ProdAudInocCat
                            join l in _context.ProdLogAuditoria on c.Id equals l.IdAuditoriaCat 
                            where l.IdAuditoria == idAuditoria
                            group l by new
                            {
                                IdLog=l.Id,
                                IdAuditoria=l.IdAuditoria,
                                Consecutivo=c.Consecutivo,
                                IdAuditoriaCat = l.IdAuditoriaCat
                            } into x
                            select new
                            {
                                IdLog = x.Key.IdLog,
                                IdAuditoria = x.Key.IdAuditoria,
                                Consecutivo = x.Key.Consecutivo,
                                IdAuditoriaCat = x.Key.IdAuditoriaCat
                            }).Distinct();

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
                x.IdAuditoria== model.IdAuditoria &&
                x.IdAuditoriaCat == model.IdAuditoriaCat);

                if (auditoriaExiste == null)
                {                    
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

    }
}
