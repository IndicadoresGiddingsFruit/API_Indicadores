using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("Seguimiento_financ")]
    public class Seguimiento_financ
    {
        [Key]
        public int Id { get; set; }
        public string Cod_Prod { get; set; }
        public short? Cod_Campo { get; set; }
        public string Comentarios { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public short? Cod_Empresa { get; set; }
        public string Estatus { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public string AP { get; set; }
        public Nullable<System.DateTime> Fecha_Up { get; set; }
    }
}
