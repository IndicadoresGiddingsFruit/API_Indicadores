using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("SIPGUsuarios")]
    public class SIPGUsuarios
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Completo { get; set; }
        public string correo { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public Nullable<short> IdRegion { get; set; }
        public string Tipo { get; set; }      
        public int? id_empleado { get; set; }
    }
}
