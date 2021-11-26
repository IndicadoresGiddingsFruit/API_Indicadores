using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers.Auditoria
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdLogAuditoriaFotoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdLogAuditoriaFotoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{IdProdAuditoria}")]
        public ActionResult Get(int IdProdAuditoria)
        {
            try
            {
                var model = _context.ProdLogAuditoriaFoto.Where(x => x.IdProdAuditoria == IdProdAuditoria).Distinct();
                return Ok(model.OrderBy(x=>x.Descripcion).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //GET: api/<ProdLogAuditoriaFotoController> 
        [HttpGet("{IdProdAuditoria}/{Id}")]
        public ActionResult Get(int IdProdAuditoria, int Id)
        {
            try
            {
                string ruta = "//192.168.0.21/recursos season/FotosAuditoriasInocuidad/"+ IdProdAuditoria+"/" + Id + ".jpg";
                string Imagen = getFile(ruta);
                return Ok(Imagen);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public string getFile(string ruta)
        {
            try
            {
                byte[] bytesImagen = System.IO.File.ReadAllBytes(ruta);
                string imagenBase64 = Convert.ToBase64String(bytesImagen);
                string tipoContenido;
                switch (Path.GetExtension(ruta))
                {
                    case ".jpg":
                        {
                            tipoContenido = "image/jpg";
                            break;
                        }
                    default:
                        {
                            return null;
                        }
                }
                return string.Format("data:{0};base64,{1}", tipoContenido, imagenBase64);
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }


        [HttpPost]
        public ActionResult Post([FromBody] ProdLogAuditoriaFoto model)
        {
            try
            {
                var item = _context.ProdLogAuditoriaFoto.FirstOrDefault(x => x.IdProdAuditoria == model.IdProdAuditoria && x.Descripcion.Equals(model.Descripcion));

                if (item == null)
                {
                    _context.ProdLogAuditoriaFoto.Add(model);
                    _context.SaveChanges();
                    return Ok(model);
                }
                else
                {
                    return BadRequest("La fotografía ya fué agregada");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{IdProdAuditoria}/{Id}")]
        public ActionResult PatchT(int IdProdAuditoria, int Id, [FromForm] IFormFile file)
        {
            try
            {
               string main_path = "//192.168.0.21/recursos season/FotosAuditoriasInocuidad/";  
               string pathString = System.IO.Path.Combine(main_path, IdProdAuditoria.ToString());
                //System.IO.Directory.CreateDirectory(pathString);

                if (!System.IO.File.Exists(pathString))
                {
                    System.IO.Directory.CreateDirectory(pathString);
                }

                var model = _context.ProdLogAuditoriaFoto.Find(Id);
                if (model.Id == Id)
                {
                    if (file != null)
                    {
                        var extension = Path.GetExtension(file.FileName).Substring(1);
                        var path = pathString + "/" + Id + ".jpg";

                        using (var stream = System.IO.File.Create(path))
                        {
                            file.CopyToAsync(stream);
                        }

                        model.Ruta = path;
                    }
                    _context.SaveChanges();
                    return Ok(model);
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
