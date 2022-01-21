using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using ApiIndicadores.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ApiIndicadores.Controllers.Muestreos
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuestreoSectorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MuestreoSectorController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<MuestreoSectorController>
        [HttpGet]
        public async Task<ActionResult<ProdMuestreoSector>> Get()
        {
            var sectores = _context.ProdMuestreoSector.Distinct();
            return Ok(await sectores.ToListAsync());
        }

        // GET api/<MuestreoSectorController>/5
        [HttpGet("{idMuestreo}")]
        public ActionResult<ProdMuestreoSector> Get(int idMuestreo)
        {
            try
            {
                var model_sector = (from y in _context.ProdMuestreoSector
                                    where y.IdMuestreo == idMuestreo
                                    group y by new
                                    {
                                        y.id,
                                        y.Sector
                                    } into g
                                    select new
                                    {
                                        Id = g.Key.id,
                                        Sector = g.Key.Sector
                                    }).ToList();

                if (model_sector != null)
                {

                    return Ok(model_sector);
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // POST api/<MuestreoSectorController>
        [HttpPost("{idMuestreo}")]
        public async Task<ActionResult<ProdMuestreoSector>> Post(int idMuestreo, [FromBody] ProdMuestreoSector model)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(idMuestreo);
                if (muestreo != null)
                {
                    ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                    prodMuestreoSector.Cod_Prod = muestreo.Cod_Prod;
                    prodMuestreoSector.Cod_Campo = muestreo.Cod_Campo;
                    prodMuestreoSector.Sector = model.Sector;
                    prodMuestreoSector.IdMuestreo = idMuestreo;
                    _context.ProdMuestreoSector.Add(prodMuestreoSector);
                    _context.SaveChanges();

                    var item = _context.ProdMuestreoSector.Where(x => x.IdMuestreo == idMuestreo).Distinct();
                    return Ok(await item.ToListAsync());
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdMuestreoSector>> Delete(int id)
        {
            try
            {
                var model = _context.ProdMuestreoSector.Find(id);
                if (model != null)
                {
                    _context.ProdMuestreoSector.Remove(model);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
