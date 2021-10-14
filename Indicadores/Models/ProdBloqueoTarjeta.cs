using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("ProdBloqueoTarjeta")]
    public class ProdBloqueoTarjeta
    {
        [Key]
        public int Id { get; set; }
        public string Cod_Prod { get; set; }
        public Int16 Cod_Campo { get; set; }
        public Int16 IdAgen { get; set; }
        public string Justificacion { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
