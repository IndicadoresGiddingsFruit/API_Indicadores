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
    public class PuntosControlController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PuntosControlController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idAuditoriaCat}")]
        public ActionResult<ProdAudInocCat> Get(int idAuditoriaCat)
        {
            try
            {
                var item = _context.ProdAudInocCat.Where(x=>x.IdAuditoriaCat == idAuditoriaCat).Distinct();

                return Ok(item.OrderBy(x => x.Consecutivo).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{idAuditoriaCat}/{noPunto}")]
        public ActionResult<ProdAudInocCat> Get(int idAuditoriaCat, string noPunto)
        {
            try
            {
                var item = _context.ProdAudInocCat.Where(x => x.IdAuditoriaCat == idAuditoriaCat && x.NoPunto == noPunto).Distinct();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{idAuditoriaCat}/{noPunto}/{consecutivo}")]
        public ActionResult<ProdAudInocCat> Get(int idAuditoriaCat, string noPunto, int consecutivo)
        {
            try
            {
                var item = _context.ProdAudInocCat.Where(x => x.IdAuditoriaCat == idAuditoriaCat && x.NoPunto == noPunto && x.Consecutivo == consecutivo).Distinct();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
