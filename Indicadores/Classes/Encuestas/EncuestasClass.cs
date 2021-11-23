using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class EncuestasClass
    {
        public int IdEncuesta { get; set; }
        public string Encuesta { get; set; }
        public int? IdUsuario { get; set; }
        public string Usuario { get; set; }
        public int? IdPregunta { get; set; }
        public string Pregunta { get; set; }
        public int? IdRespuesta { get; set; }
        public string Respuesta { get; set; }
        public string RespuestaLibre { get; set; }
        public int? IdRelacion { get; set; }
    }
}
