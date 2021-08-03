using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models
{
    [Table("EncuestasRelacion")]
    public class EncuestasRelacion
    {
        [Key]
        public int? Id { get; set; }
        public int? IdPregunta { get; set; }
        public int? IdRespuesta { get; set; }
    }
}
