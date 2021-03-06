using ApiIndicadores.Context;
using ApiIndicadores.Models.Auditoria;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                var file = _context.ProdAuditoriaFoto.FirstOrDefault(x => x.IdProdAuditoria == IdProdAuditoria && (x.Id == Id || x.IdLogAC == Id));
                if (file == null)
                {
                    return Ok("No hay archivo");
                }
                else
                {
                    if (file.extension == "pdf  " || file.extension == "pdf")
                    {
                        var fileBytes = System.IO.File.ReadAllBytes(file.Ruta);
                        new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(file.Ruta), out var contentType);
                        return File(fileBytes, contentType ?? "application/octet-stream", file.Descripcion);                   
                    }
                    else
                    {
                        string Imagen = getFile(file.Ruta);
                        return Ok(Imagen);
                    }
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

        //POST: Subir datos de las fotos
        [HttpPost]
        public ActionResult Post([FromBody] ProdAuditoriaFoto model)
        {
            try
            {
                var item = _context.ProdAuditoriaFoto.FirstOrDefault(x => x.IdProdAuditoriaCampo == model.IdProdAuditoriaCampo && x.IdLogAC == model.IdLogAC && x.Descripcion.Equals(model.Descripcion));

                if (item == null)
                {
                    _context.ProdAuditoriaFoto.Add(model);
                    _context.SaveChanges();

                    //Revisar cuantas AccionesCorrectivas faltan para dar por terminada la auditoria 
                    var accionesCorrectivasFaltantes = (from a in _context.ProdLogAccionesCorrectivas
                                                        join l in _context.ProdLogAuditoria on a.IdLogAuditoria equals l.Id
                                                        where l.IdProdAuditoria == model.IdProdAuditoria && l.Opcion == "NO"
                                                        group a by new
                                                        {
                                                            Faltantes = a.Id
                                                        } into x
                                                        select new
                                                        {
                                                            Faltantes = x.Key.Faltantes,
                                                        }).Count();

                    //Revisar cuantas fotos de las AccionesCorrectivas faltan para dar por terminada la auditoria 
                    var accionesCorrectivasFotos = (from a in _context.ProdLogAccionesCorrectivas
                                                    join l in _context.ProdLogAuditoria on a.IdLogAuditoria equals l.Id
                                                    join f in _context.ProdAuditoriaFoto on a.Id equals f.IdLogAC
                                                    where l.IdProdAuditoria == model.IdProdAuditoria && f.Ruta == null
                                                    group a by new
                                                    {
                                                        Faltantes = f.Id
                                                    } into x
                                                    select new
                                                    {
                                                        Faltantes = x.Key.Faltantes,
                                                    }).Count();

                    //Revisar que no haya acciones correctivas pendientes
                    if (accionesCorrectivasFaltantes == 0 && accionesCorrectivasFotos == 0)
                    {
                        var auditoria = _context.ProdAudInoc.Find(model.IdProdAuditoria);
                        //Revisar que se haya finalizado la auditoria
                        if (auditoria.Fecha_termino != null)
                        {
                            auditoria.Finalizada = 1;
                            _context.SaveChangesAsync();
                        }
                    }

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

        //PATCH: Subir fotos
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
