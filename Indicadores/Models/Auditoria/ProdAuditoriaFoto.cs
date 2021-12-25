using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdAuditoriaFoto")]
    public class ProdAuditoriaFoto
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
        public int? IdProdAuditoria { get; set; }
        public int? IdLogAC { get; set; }
        public int? IdProdAuditoriaCampo { get; set; }
        public string extension { get; set; }

    }
}
