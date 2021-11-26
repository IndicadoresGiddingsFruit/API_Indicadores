using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Auditoria
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdAudInocCamposController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdAudInocCamposController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<ProdAudInocCampos>> Post([FromBody] ProdAudInocCampos model)
        {
            try
            {
                var auditoriaExiste = _context.ProdAudInoc.FirstOrDefault(x =>x.Id ==  model.IdProdAuditoria);
                if (auditoriaExiste != null)
                {
                    var campos =
                        _context.ProdAudInocCampos.FirstOrDefault(x =>
                    x.Cod_Prod == auditoriaExiste.Cod_Prod && x.Cod_Campo == model.Cod_Campo &&
                    x.IdProdAuditoria == model.IdProdAuditoria);

                    if (campos == null)
                    {
                        model.Cod_Prod = auditoriaExiste.Cod_Prod;
                        _context.ProdAudInocCampos.Add(model);
                        await _context.SaveChangesAsync();
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest("El campo ya fué agregado anteriormente");
                    }
                }
                else
                {
                    return BadRequest("La auditoría no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
