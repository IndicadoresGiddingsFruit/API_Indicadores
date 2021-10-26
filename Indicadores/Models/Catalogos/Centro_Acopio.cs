using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Centro_Acopio")]
    public class Centro_Acopio
    {
        [Key]
        public int id_centro_acopio { get; set; }
        public string nombre_centro { get; set; }
        public int id_zona { get; set; }
        public bool estado { get; set; }
        public string ciudad { get; set; }
    }
}
