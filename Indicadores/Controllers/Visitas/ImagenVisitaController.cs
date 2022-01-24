using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenVisitaController : ControllerBase
    {
        // GET Foto por visita
        [HttpGet("{IdVisita}")]
        //imagen
        public ActionResult obtenerImagen(int IdVisita = 0)
        {
            string ruta = "//192.168.0.21/recursos season/VisitasProd/" + IdVisita + "/1.jpg";
            string Imagen = getImage(ruta);
            return Ok(Imagen);
        }

        public string getImage(string ruta)
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
    }
}
