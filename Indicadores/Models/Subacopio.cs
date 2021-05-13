using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Subacopio")]
    public class Subacopio
    {
        [Key]
        public int id_subacopio { get; set; }
        public string nombre { get; set; }
        public int  id_centro_acopio { get; set; }
        public bool estado { get; set; }
    }
}