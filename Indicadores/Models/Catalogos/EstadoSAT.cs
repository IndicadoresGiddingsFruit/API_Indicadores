using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Catalogos
{
    [Table("EstadoSAT")]
    public class EstadoSAT
    {
        [Key]
        public string CodEstado { get; set; }
        public string CodPais { get; set; }
        public string Descripcion { get; set; }
        public int? ZonaNielsen { get; set; }

    }
    
}
