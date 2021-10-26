using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("ProdAgenteCat")]
    public class ProdAgenteCat
    {
        [Key]
        public short IdAgen { get; set; }
        public string Nombre { get; set; }
        public string Depto { get; set; }
        public string Abrev { get; set; }
        public Nullable<bool> Activo { get; set; }
        public short IdRegion { get; set; }
        public string Codigo { get; set; }
    }
}
