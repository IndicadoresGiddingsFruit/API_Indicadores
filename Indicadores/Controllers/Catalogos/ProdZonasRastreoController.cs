using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Catalogos
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdZonasRastreoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdZonasRastreoController(AppDbContext context)
        {
            this._context = context;
        }

        //zonas
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                var model = _context.ProdZonasRastreoCat.OrderBy(x => x.DescZona).ToList();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
