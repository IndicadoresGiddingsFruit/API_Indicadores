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

        //Lista de campos por auditoría 
        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {
                var listCampos = (from a in _context.ProdAudInocCampos
                            join c in _context.ProdCamposCat on new { a.Cod_Prod, a.Cod_Campo } equals new { c.Cod_Prod, c.Cod_Campo }
                            where a.IdProdAuditoria == IdProdAuditoria
                            group a by new
                            {
                                Id=a.Id,
                                IdProdAuditoria = a.IdProdAuditoria,
                                Cod_Prod=a.Cod_Prod,
                                Cod_Campo=a.Cod_Campo,
                                Campo=c.Descripcion
                            } into x
                            select new
                            {
                                Id = x.Key.Id,
                                IdProdAuditoria = x.Key.IdProdAuditoria,
                                Cod_Prod = x.Key.Cod_Prod,
                                Cod_Campo = x.Key.Cod_Campo,
                                Campo = x.Key.Campo
                            }).Distinct();

                return Ok(listCampos.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Agregar campos por auditoria
        [HttpPost]
        public async Task<ActionResult<ProdAudInocCampos>> Post([FromBody] ProdAudInocCampos model)
        {
            try
            {
                var auditoriaExiste = _context.ProdAudInoc.FirstOrDefault(x => x.Id == model.IdProdAuditoria);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdAudInocCampos>> Delete(int id)
        {
            try
            {
                var model = _context.ProdAudInocCampos.Find(id);
                if (model != null)
                {
                    _context.ProdAudInocCampos.Remove(model);
                    await _context.SaveChangesAsync();
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
