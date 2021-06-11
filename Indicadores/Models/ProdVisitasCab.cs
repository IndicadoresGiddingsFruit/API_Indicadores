using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("ProdVisitasCab")]
    public class ProdVisitasCab
    {
        [Key]
        public int IdVisita { get; set; }
        public short IdAgen { get; set; }
        public string Cod_prod { get; set; }
        public short Cod_Campo { get; set; }
        public Nullable<short> IdSector { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Estatus { get; set; }
        public string Comentarios { get; set; }
        public string Atendio { get; set; }
        public string UidApp { get; set; }
        public Nullable<System.Guid> rowguid { get; set; }
        public string TipoVisita { get; set; }
    }
}
