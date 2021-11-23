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
    public class AuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuditoriaController(AppDbContext context)
        {
            _context = context;
        } 

        [HttpGet("{idAgen}")]
        public ActionResult Get(int idAgen)
        {
            try
            {
                if (idAgen == 281 || idAgen== 326)
                {
                    var item = (from a in _context.ProdAudInoc
                                join p in _context.ProdProductoresCat on a.Cod_Prod equals p.Cod_Prod
                                join c in _context.ProdCamposCat on new { a.Cod_Prod, a.Cod_Campo } equals new { c.Cod_Prod, c.Cod_Campo } into ProdCamposCat
                                from c in ProdCamposCat.DefaultIfEmpty()
                                join z in _context.ProdZonasRastreoCat on a.IdZona equals z.IdZona
                                join i in _context.ProdAgenteCat on a.IdAgen equals i.IdAgen                                 
                                group a by new
                                {
                                    Id = a.Id,
                                    Cod_Prod = a.Cod_Prod,
                                    Productor = p.Nombre,
                                    Cod_Campo = a.Cod_Campo,
                                    Campo = c.Descripcion,
                                    IdZona = a.IdZona,
                                    Zona = z.DescZona,
                                    Fecha = a.Fecha,
                                    IdAgen = a.IdAgen,
                                    Asesor = i.Nombre
                                } into x
                                select new
                                {
                                    Id = x.Key.Id,
                                    Cod_Prod = x.Key.Cod_Prod,
                                    Productor = x.Key.Productor,
                                    Cod_Campo = x.Key.Cod_Campo,
                                    Campo = x.Key.Campo,
                                    IdZona = x.Key.IdZona,
                                    Zona = x.Key.Zona,
                                    Fecha = x.Key.Fecha,
                                    IdAgen = x.Key.IdAgen,
                                    Asesor = x.Key.Asesor
                                }).Distinct();
                    return Ok(item.ToList());
                }
                else
                {
                    var item = (from a in _context.ProdAudInoc
                                join p in _context.ProdProductoresCat on a.Cod_Prod equals p.Cod_Prod
                                join c in _context.ProdCamposCat on new { a.Cod_Prod, a.Cod_Campo } equals new { c.Cod_Prod, c.Cod_Campo } into ProdCamposCat
                                from c in ProdCamposCat.DefaultIfEmpty()
                                join z in _context.ProdZonasRastreoCat on a.IdZona equals z.IdZona
                                join i in _context.ProdAgenteCat on a.IdAgen equals i.IdAgen
                                where a.IdAgen == idAgen
                                group a by new
                                {
                                    Id = a.Id,
                                    Cod_Prod = a.Cod_Prod,
                                    Productor = p.Nombre,
                                    Cod_Campo = a.Cod_Campo,
                                    Campo = c.Descripcion,
                                    IdZona = a.IdZona,
                                    Zona = z.DescZona,
                                    Fecha = a.Fecha,
                                    IdAgen = a.IdAgen,
                                    Asesor = i.Nombre
                                } into x
                                select new
                                {
                                    Id = x.Key.Id,
                                    Cod_Prod = x.Key.Cod_Prod,
                                    Productor = x.Key.Productor,
                                    Cod_Campo = x.Key.Cod_Campo,
                                    Campo = x.Key.Campo,
                                    IdZona = x.Key.IdZona,
                                    Zona = x.Key.Zona,
                                    Fecha = x.Key.Fecha,
                                    IdAgen = x.Key.IdAgen,
                                    Asesor = x.Key.Asesor
                                }).Distinct();
                    return Ok(item.ToList());
                }
                 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("{idAgen}/{idAuditoria}")]
        public ActionResult Get(int idAgen, int IdProdAuditoria)
        {
            try
            {
                if (idAgen == 281 || idAgen == 326)
                {
                    var item = (from a in _context.ProdAudInoc
                                join p in _context.ProdProductoresCat on a.Cod_Prod equals p.Cod_Prod
                                join c in _context.ProdCamposCat on new { a.Cod_Prod, a.Cod_Campo } equals new { c.Cod_Prod, c.Cod_Campo } into ProdCamposCat
                                from c in ProdCamposCat.DefaultIfEmpty()
                                join z in _context.ProdZonasRastreoCat on a.IdZona equals z.IdZona
                                join i in _context.ProdAgenteCat on a.IdAgen equals i.IdAgen
                                where a.Id == IdProdAuditoria
                                group a by new
                                {
                                    Id = a.Id,
                                    Cod_Prod = a.Cod_Prod,
                                    Productor = p.Nombre,
                                    Cod_Campo = a.Cod_Campo,
                                    Campo = c.Descripcion,
                                    IdZona = a.IdZona,
                                    Zona = z.DescZona,
                                    Fecha = a.Fecha,
                                    IdAgen = a.IdAgen,
                                    Asesor = i.Nombre
                                } into x
                                select new
                                {
                                    Id = x.Key.Id,
                                    Cod_Prod = x.Key.Cod_Prod,
                                    Productor = x.Key.Productor,
                                    Cod_Campo = x.Key.Cod_Campo,
                                    Campo = x.Key.Campo,
                                    IdZona = x.Key.IdZona,
                                    Zona = x.Key.Zona,
                                    Fecha = x.Key.Fecha,
                                    IdAgen = x.Key.IdAgen,
                                    Asesor = x.Key.Asesor
                                }).Distinct();

                    return Ok(item.ToList());
                }
                else
                {
                    var item = (from a in _context.ProdAudInoc
                                join p in _context.ProdProductoresCat on a.Cod_Prod equals p.Cod_Prod
                                join c in _context.ProdCamposCat on new { a.Cod_Prod, a.Cod_Campo } equals new { c.Cod_Prod, c.Cod_Campo } into ProdCamposCat
                                from c in ProdCamposCat.DefaultIfEmpty()
                                join z in _context.ProdZonasRastreoCat on a.IdZona equals z.IdZona
                                join i in _context.ProdAgenteCat on a.IdAgen equals i.IdAgen
                                where a.Id == IdProdAuditoria && a.IdAgen == idAgen
                                group a by new
                                {
                                    Id = a.Id,
                                    Cod_Prod = a.Cod_Prod,
                                    Productor = p.Nombre,
                                    Cod_Campo = a.Cod_Campo,
                                    Campo = c.Descripcion,
                                    IdZona = a.IdZona,
                                    Zona = z.DescZona,
                                    Fecha = a.Fecha,
                                    IdAgen = a.IdAgen,
                                    Asesor = i.Nombre
                                } into x
                                select new
                                {
                                    Id = x.Key.Id,
                                    Cod_Prod = x.Key.Cod_Prod,
                                    Productor = x.Key.Productor,
                                    Cod_Campo = x.Key.Cod_Campo,
                                    Campo = x.Key.Campo,
                                    IdZona = x.Key.IdZona,
                                    Zona = x.Key.Zona,
                                    Fecha = x.Key.Fecha,
                                    IdAgen = x.Key.IdAgen,
                                    Asesor = x.Key.Asesor
                                }).Distinct();

                    return Ok(item.ToList());
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult<ProdAudInoc>> Post([FromBody] ProdAudInoc model)
        {
            try
            {
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now.Date >= m.Inicio && DateTime.Now.Date <= m.Fin);
               
                var auditoriaExiste = _context.ProdAudInoc.FirstOrDefault(x => 
                x.Cod_Prod == model.Cod_Prod && 
                x.Cod_Campo == model.Cod_Campo && 
                x.Temporada == catSemanas.Temporada);

                if (auditoriaExiste == null)
                {
                    model.Temporada = catSemanas.Temporada;
                    model.Fecha = DateTime.Now;
                    _context.ProdAudInoc.Add(model);
                    await _context.SaveChangesAsync();
                    return Ok(model); 
                }
                else
                {
                    return BadRequest("El campo ya fué auditado");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
