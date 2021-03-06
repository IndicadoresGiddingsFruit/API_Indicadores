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
    public class AuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuditoriaController(AppDbContext context)
        {
            _context = context;
        }

        //Datos de audiorias
        [HttpGet("{idAgen}/{IdProdAuditoria}")]
        public ActionResult Get(int idAgen, int IdProdAuditoria)
        {
            try
            {
                var item = _context.AuditoriaClass.FromSqlRaw($"sp_GetAuditoria " + idAgen + "," + IdProdAuditoria + "").ToList();
                return Ok(item.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Nueva auditoria
        [HttpPost]
        public async Task<ActionResult<ProdAudInoc>> Post([FromBody] ProdAudInoc model)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now.Date >= m.Inicio && DateTime.Now.Date <= m.Fin);
                model.Temporada = catSemanas.Temporada;
                model.Fecha = DateTime.Now;
                _context.ProdAudInoc.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Finalizar auditoría
        [HttpPut("{Id}")]
        public async Task<ActionResult<ProdAudInoc>> Put(int Id)
        {
            try
            {
                var model = _context.ProdAudInoc.Find(Id);
                if (model != null)
                {
                    model.Fecha_termino = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok(model);
                }
                else
                {
                    return BadRequest("Id inválido");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
