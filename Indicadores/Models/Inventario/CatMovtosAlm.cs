using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Inventario
{
    [Table("CatMovtosAlm")]
    public class CatMovtosAlm
    {
        [Key]
        public Int16 Cod_Mov { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public byte ModifUsuario { get; set; }
        public byte AfeCenCosto { get; set; }
        public byte AfeCostoProm { get; set; }
        public Int16 Orden { get; set; }
        public byte EsConsignacion { get; set; }
    }
}
