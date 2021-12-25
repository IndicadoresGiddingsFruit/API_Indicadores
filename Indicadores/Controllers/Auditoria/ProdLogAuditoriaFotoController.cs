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
                var model = (from m in _context.ProdAuditoriaFoto
                             join c in _context.ProdAudInocCampos on m.IdProdAuditoriaCampo equals c.Id into C
                             from c in C.DefaultIfEmpty()
                             where m.IdProdAuditoria == IdProdAuditoria
                             group m by new
                             {
                                 Id = m.Id,
                                 Descripcion = m.Descripcion,
                                 Ruta = m.Ruta,
                                 IdProdAuditoria = m.IdProdAuditoria,
                                 IdProdAuditoriaCampo = m.IdProdAuditoriaCampo,
                                 IdLogAc = m.IdLogAC,
                                 Extension = m.extension,
                                 isOpen = false,
                                 Cod_Campo = c.Cod_Campo
                             } into x
                             select new
                             {
                                 Id = x.Key.Id,
                                 Descripcion = x.Key.Descripcion,
                                 Ruta = x.Key.Ruta,
                                 IdProdAuditoria = x.Key.IdProdAuditoria,
                                 IdProdAuditoriaCampo = x.Key.IdProdAuditoriaCampo,
                                 IdLogAC = x.Key.IdLogAc,
                                 Extension = x.Key.Extension,
                                 isOpen = x.Key.isOpen,
                                 Cod_Campo = x.Key.Cod_Campo,
                             }).Distinct();
 

                //var model = _context.ProdAuditoriaFoto.Where(x => x.IdProdAuditoria == IdProdAuditoria).Distinct();
                return Ok(model.OrderBy(x => x.Descripcion).ToList());
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
                var foto = _context.ProdAuditoriaFoto.FirstOrDefault(x => x.IdProdAuditoria == IdProdAuditoria && (x.Id == Id || x.IdLogAC == Id));
                if (foto == null)
                {
                    return Ok("No hay foto"); 
                }
                else
                {
                    string Imagen = getFile(foto.Ruta);
                    return Ok(Imagen);
                }               
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
                    case ".jpeg":
                        {
                            tipoContenido = "image/jpeg";
                            break;
                        }
                    case ".jpg":
                        {
                            tipoContenido = "image/jpg";
                            break;
                        }
                    case ".gif":
                        {
                            tipoContenido = "image/gif";
                            break;
                        }
                    case ".png":
                        {
                            tipoContenido = "image/png";
                            break;
                        }
                    case ".jfif":
                        {
                            tipoContenido = "image/jfif";
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
        public ActionResult Post([FromBody] ProdAuditoriaFoto model)
        {
            try
            {
                var item = _context.ProdAuditoriaFoto.FirstOrDefault(x => x.IdProdAuditoriaCampo == model.IdProdAuditoriaCampo && x.IdLogAC==model.IdLogAC && x.Descripcion.Equals(model.Descripcion));

                if (item == null)
                {
                    _context.ProdAuditoriaFoto.Add(model);
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
        public async Task<ActionResult<ProdAudInoc>> PatchT(int IdProdAuditoria, int Id, [FromForm] IFormFile file)
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

                var model = _context.ProdAuditoriaFoto.Find(Id);
                if (model.Id == Id)
                {
                    if (file != null)
                    {
                        var extension = Path.GetExtension(file.FileName).Substring(1);
                        var path = pathString + "/" + Id + "." + extension;

                        using (var stream = System.IO.File.Create(path))
                        {
                            file.CopyTo(stream);
                        }

                        model.Ruta = path;
                        model.extension = extension;
                    }
                    await _context.SaveChangesAsync();
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
