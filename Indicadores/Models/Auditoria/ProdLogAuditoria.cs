using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdLogAuditoria")]
    public class ProdLogAuditoria
    {
        [Key]
        public int  Id { get; set; }
        public int? IdProdAuditoria { get; set; }
        public int? IdCatAuditoria { get; set; } 
        public string Opcion { get; set; } 
        public string Justificacion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
