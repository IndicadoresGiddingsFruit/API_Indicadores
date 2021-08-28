using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("EncuestasUsuarios")]
    public class EncuestasUsuarios
    {
        [Key]
        public int? Id { get; set; }
        public int? IdEncuesta { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime? Fecha { get; set; } 
    }
}
