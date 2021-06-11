using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("CatProductos")]
    public class CatProductos
    {
        [Key]
        public short Tipo { get; set; }
        public short Producto { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionIngles { get; set; }
        public short Consecutivo { get; set; }
        public string CodCaades { get; set; }
        public short CtaEmb { get; set; }
        public string SubCtaGral { get; set; }
        public short ConsecClasif { get; set; }
        public string Temperatura { get; set; }
        public string NomBotanico { get; set; }
        public string CodTrazabilidad { get; set; }
        public bool organico { get; set; }
        public Nullable<bool> Pedido { get; set; }
        public string Abreviacion { get; set; }
        public bool Propia { get; set; }
        public Nullable<short> Contrato { get; set; }
        public string Grupo { get; set; }
    }
}
