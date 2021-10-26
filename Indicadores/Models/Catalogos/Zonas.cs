using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Zonas")]
    public class Zonas
    {
        [Key]
        public int id_zona { get; set; }
        public string nombre_zona { get; set; }
        public int id_estado { get; set; }
        public bool estado { get; set; }
    }
}
