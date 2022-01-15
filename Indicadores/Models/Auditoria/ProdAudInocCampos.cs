using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdAudInocCampos")]
    public class ProdAudInocCampos
    {
        [Key]
        public int Id { get; set; }
        public int IdProdAuditoria { get; set; }
        public string Cod_Prod { get; set; }
        public Int16 Cod_Campo { get; set; }
        public int? Proyeccion { get; set; }
        public string TipoCertificacion { get; set; }
    }
}
