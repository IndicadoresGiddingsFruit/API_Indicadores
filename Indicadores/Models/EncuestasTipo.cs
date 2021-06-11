using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("EncuestasTipo")]
    public class EncuestasTipo
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }

    }
}
