using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Puesto")]
    public class Puesto
    {
        [Key]
        public int id_puesto { get; set; }
        public string nombre_puesto { get; set; }
        public string actividades_puesto { get; set; }
        public int id_departamento { get; set; }
        public bool estado { get; set; }
    }
}
