using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("ProdZonasRastreoCat")]
    public class ProdZonasRastreoCat
    {
        [Key]
        public short IdZona { get; set; }
        public string Codigo { get; set; }
        public string DescZona { get; set; }
        public Nullable<bool> Pedido { get; set; }
        public Nullable<int> IdRegion { get; set; }
        public Nullable<int> SRegion { get; set; }
    }
}
