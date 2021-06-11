using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("CatSemanas")]
    public class CatSemanas
    {
        [Key]
        public string Temporada { get; set; }
        public byte Semana { get; set; }
        public System.DateTime Inicio { get; set; }
        public System.DateTime Fin { get; set; }
        public string Yearr { get; set; }
        public string Mes { get; set; }
    }
}
