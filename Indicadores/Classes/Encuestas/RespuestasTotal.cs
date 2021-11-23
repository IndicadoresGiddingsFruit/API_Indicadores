using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    [Table("RespuestasTotal")]
    public class RespuestasTotal
    {
        [Key]
        public int IdEncuesta { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Zona { get; set; }
        public string Departamento { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public int IdPregunta { get; set; }
        public string Pregunta { get; set; }
        public int IdRespuesta { get; set; }
        public string Respuesta { get; set; }
        public int IdRelacion { get; set; }
    }
}
