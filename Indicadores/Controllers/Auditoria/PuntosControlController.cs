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
