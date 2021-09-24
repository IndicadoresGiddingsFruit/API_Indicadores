using ApiIndicadores.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using ApiIndicadores.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        [HttpGet("{cod_prod}/{cod_campo}")]
        public async Task<ActionResult<ProdMuestreoSector>> Get(string cod_prod, short cod_campo)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Where(x => x.Cod_Prod == cod_prod && x.Cod_Campo == cod_campo).FirstOrDefault();
                if (muestreo != null)
                {
                    if (muestreo.IdSector != null)
                    {
                        var model_sector = _context.ProdMuestreoSector.Where(x => x.id == muestreo.IdSector).FirstOrDefault();
                     
                            if (model_sector.Sector == 1)
                            {
                                var s = _context.ProdMuestreoSector.Where(x => x.id == muestreo.IdSector).Distinct();
                                return Ok(await s.ToListAsync());
                            }
                            else
                            {
                                var sectores = _context.ProdMuestreoSector.Where(x => x.Cod_Prod == cod_prod && x.Cod_Campo == cod_campo).Distinct();
                                return Ok(await sectores.ToListAsync());
                            }
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception e) {
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
                        prodMuestreoSector.Cod_Prod = model.Cod_Prod;
                        prodMuestreoSector.Cod_Campo = model.Cod_Campo;
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

        // PUT api/<MuestreoSectorController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MuestreoSectorController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdMuestreoSector>> Delete(int id)
        {
            try
            {
                var model = _context.ProdMuestreoSector.Find(id);
                if (model != null)
                {
                    var muestreo = _context.ProdMuestreo.FirstOrDefault(x => x.IdSector == id);
                    if (muestreo != null)
                    {
                        muestreo.IdSector = null;
                        await _context.SaveChangesAsync();

                        _context.ProdMuestreoSector.Remove(model);
                        await _context.SaveChangesAsync();

                        var busca_sector = _context.ProdMuestreoSector.Where(x => x.Cod_Prod == muestreo.Cod_Prod && x.Cod_Campo == muestreo.Cod_Campo).OrderByDescending(x => x.Sector).FirstOrDefault();
                        if (busca_sector != null)
                        {
                            muestreo.IdSector = busca_sector.id;
                            _context.SaveChanges();
                        }
                    }
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
