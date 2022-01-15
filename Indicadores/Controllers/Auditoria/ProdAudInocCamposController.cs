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
                            join t in _context.CatTiposProd on c.Tipo equals t.Tipo
                            join prod in _context.CatProductos on new { c.Tipo, c.Producto } equals new { prod.Tipo,  prod.Producto }
                            join p in _context.ProdProductoresCat on a.Cod_Prod equals p.Cod_Prod
                            join l in _context.CatLocalidades on c.CodLocalidad equals l.CodLocalidad
                            join m in _context.MunicipioSAT on new { l.CodMunicipio, l.CodEstado } equals new { m.CodMunicipio, m.CodEstado }
                            join e in _context.EstadoSAT on l.CodEstado equals e.CodEstado
                            join ac in _context.ProdAudInoc on a.IdProdAuditoria equals ac.Id
                            where a.IdProdAuditoria == IdProdAuditoria
                            group a by new
                            {
                                Id=a.Id,
                                IdProdAuditoria = a.IdProdAuditoria,
                                Cod_Prod=a.Cod_Prod,
                                Cod_Campo=a.Cod_Campo,
                                Campo=c.Descripcion,
                                Tipo=t.Descripcion,
                                Producto=prod.Descripcion,
                                TipoCertificacion =a.TipoCertificacion,
                                RFC = p.RFC,
                                Telefono = p.Telefono,
                                Email=p.Correo,
                                Ubicacion = c.Ubicacion,
                                CodLocalidad =c.CodLocalidad,
                                Localidad = l.Descripcion,
                                Municipio=m.Descripcion,
                                Estado=e.Descripcion,
                                Gps_Latitude = c.Gps_Latitude,
                                Gps_Longitude = c.Gps_Longitude,
                                Proyeccion=a.Proyeccion,
                                Titular=p.Contacto
                            } into x
                            select new
                            {
                                Id = x.Key.Id,
                                IdProdAuditoria = x.Key.IdProdAuditoria,
                                Cod_Prod = x.Key.Cod_Prod,
                                Cod_Campo = x.Key.Cod_Campo,
                                Campo = x.Key.Campo,
                                Tipo = x.Key.Tipo,
                                Producto = x.Key.Producto,
                                TipoCertificacion = x.Key.TipoCertificacion,
                                RFC = x.Key.RFC,
                                Telefono = x.Key.Telefono,
                                Email = x.Key.Email,
                                Ubicacion = x.Key.Ubicacion,
                                CodLocalidad = x.Key.CodLocalidad,
                                Localidad = x.Key.Localidad,
                                Municipio = x.Key.Municipio,
                                Estado = x.Key.Estado,
                                Gps_Latitude = x.Key.Gps_Latitude,
                                Gps_Longitude = x.Key.Gps_Longitude,
                                Proyeccion = x.Key.Proyeccion,
                                Titular = x.Key.Titular,

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

        // PUT api/<EncuestasDetController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProdAudInocCampos>> Put(int id, [FromBody] ProdAudInocCampos model)
        {
            try
            {
                var item = _context.ProdAudInocCampos.Find(id);
                if (item != null)
                {
                    if (model.Proyeccion != null)
                    {
                        item.Proyeccion = model.Proyeccion;
                    }
                    if (model.TipoCertificacion != "")
                    {
                        item.TipoCertificacion = model.TipoCertificacion;
                    }
                    await _context.SaveChangesAsync();
                    return Ok(model);
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
