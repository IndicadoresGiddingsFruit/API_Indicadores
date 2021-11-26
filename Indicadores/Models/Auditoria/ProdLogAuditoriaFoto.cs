using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdLogAuditoriaFoto")]
    public class ProdLogAuditoriaFoto
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public int IdProdAuditoria { get; set; }

    }
}
