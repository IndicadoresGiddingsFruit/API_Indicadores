using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdProductoresCatController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdProductoresCatController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<ProdProductoresCatController> 
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var item = _context.ProdProductoresCat.ToList();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<EncuestasDetController>/5
        [HttpPut("{cod_Prod}")]
        public async Task<ActionResult<ProdProductoresCat>> Put(string cod_Prod, [FromBody] ProdProductoresCat model)
        {
            try
            {
                var item = _context.ProdProductoresCat.Where(x => x.Cod_Prod == cod_Prod).FirstOrDefault();
                if (item != null)
                {
                    if(model.Contacto!= "")
                    {
                        item.Contacto = model.Contacto;
                    }
                    if (model.RFC != "")
                    {
                        item.RFC = model.RFC;
                    }
                    if (model.Correo != "")
                    {
                        item.Correo = model.Correo;
                    }
                    if (model.Telefono != "")
                    {
                        item.Telefono = model.Telefono;
                    }         
                    await _context.SaveChangesAsync();
                    return Ok(model);
                }
                else
                {
                    return BadRequest("El código no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
