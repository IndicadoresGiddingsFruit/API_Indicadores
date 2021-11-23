using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Auditoria
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuntosControlController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PuntosControlController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idAuditoriaProd}")]
        public ActionResult<ProdAudInocCat> Get(int idAuditoriaProd)
        {
            try
            {
                //var item = _context.ProdAudInocCat.Where(x=>x.IdAuditoriaCat == idAuditoriaCat).Distinct();

                //var item = (from c in _context.ProdAudInocCat
                //            join p in _context.ProdAudInoc on c.IdAuditoriaCat equals p.IdNorma
                //            join l in _context.ProdLogAuditoria on c.Id equals l.IdCatAuditoria                              
                //            where p.Id == idAuditoriaProd
                //            group l by new
                //            {
                //                Id = c.Id,
                //                Consecutivo = c.Consecutivo,
                //                NoPunto = c.NoPunto,
                //                NoPuntoDesc = c.NoPuntoDesc,
                //                PuntoControl = c.PuntoControl,
                //                PuntoControlDesc = c.PuntoControlDesc,
                //                Respondida = l.IdCatAuditoria,
                //            } into x
                //            select new
                //            {
                //                Id = x.Key.Id,
                //                Consecutivo = x.Key.Consecutivo,
                //                NoPunto = x.Key.NoPunto,
                //                NoPuntoDesc = x.Key.NoPuntoDesc,
                //                PuntoControl = x.Key.PuntoControl,
                //                PuntoControlDesc = x.Key.PuntoControlDesc,
                //                Respondida = x.Key.Respondida == null ? null : x.Key.Respondida,
                //            }).Distinct();

                //return Ok(item.OrderBy(x => x.Consecutivo).ToList());
                 
                var item = _context.LogClass.FromSqlRaw($"sp_GetLogAuditoria " + idAuditoriaProd + "").ToList();

                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{IdNorma}/{idPunto}")]
        public ActionResult<ProdAudInocCat> Get(int IdNorma, int idPunto)
        {
            try
            {
                var item = _context.ProdAudInocCat.Where(x => x.IdNorma == IdNorma && x.Id == idPunto).Distinct();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
