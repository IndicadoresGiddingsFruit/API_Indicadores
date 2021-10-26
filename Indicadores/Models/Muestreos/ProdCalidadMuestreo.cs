using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("ProdCalidadMuestreo")]
    public class ProdCalidadMuestreo
    {
        [Key]
        public int Id { get; set; }
        public string Estatus { get; set; }
        public string Incidencia { get; set; }
        public string Propuesta { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public Nullable<int> Id_Muestreo { get; set; }
    }
}
