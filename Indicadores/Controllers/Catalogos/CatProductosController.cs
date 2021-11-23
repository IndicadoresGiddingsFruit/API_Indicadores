using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatProductosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CatProductosController(AppDbContext context)
        {
            this._context = context;
        }

        // GET: api/<CatProductosController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {

                var item = (from t in _context.CatTiposProd
                            join p in _context.CatProductos on t.Tipo equals p.Tipo
                            group t by new
                            {
                               IdTipo = t.Tipo,
                               Tipo=t.Descripcion,
                               IdProducto=p.Producto,
                               Producto=p.Descripcion                               
                            } into x
                            select new
                            {
                                IdTipo = x.Key.IdTipo,
                                Tipo = x.Key.Tipo,
                                IdProducto = x.Key.IdProducto,
                                Producto = x.Key.Producto
                            }).ToList();
                return Ok(item);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
