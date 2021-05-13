using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Departamentos")]
    public class Departamentos
    {
        [Key]
        public int id_departamentos { get; set; }
        public string nombre_departamentos { get; set; }
        public bool estado { get; set; }
    }
}
