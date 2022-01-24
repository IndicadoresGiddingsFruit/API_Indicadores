using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("AuditoriaCat")]
    public class AuditoriaCat
    {
        [Key]
        public int Id { get; set; }
        public string Norma { get; set; } 
    }
}
