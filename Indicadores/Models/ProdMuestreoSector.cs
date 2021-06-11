using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace ApiIndicadores.Models
{
    [Table("ProdMuestreoSector")]
    public class ProdMuestreoSector
    {
        [Key]
        public int id { get; set; }
        public string Cod_Prod { get; set; }
        public Nullable<short> Cod_Campo { get; set; }
        public Nullable<short> Sector { get; set; }
    }
}
