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

        //GET: Todos los puntos de control respondidos y sin responder
        [HttpGet("{idAgen}/{idAuditoriaProd}")]
        public ActionResult<ProdAudInocCat> Get(int idAgen, int idAuditoriaProd)
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

        //GET: Punto de control por id
        [HttpGet("{idPunto}")]
        public ActionResult<ProdAudInocCat> Get(int idPunto)
        {
            try
            {
                var item = _context.ProdAudInocCat.Find(idPunto);
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
