using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdLogAccionesCorrectivas")]
    public class ProdLogAccionesCorrectivas
    {
        [Key]
        public int Id { get; set; }
        public int IdLogAuditoria { get; set; }
        public string Justificacion { get; set; }
        public DateTime Fecha { get; set; }
    }   
}
