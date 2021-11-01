using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Inventario
{
    [Table("CatUniMed")]
    public class CatUniMed
    {
        [Key]
        public Int16 Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Abrev { get; set; }
        public byte Decimales { get; set; }
        public string CodigoSAT { get; set; }
    }
}
