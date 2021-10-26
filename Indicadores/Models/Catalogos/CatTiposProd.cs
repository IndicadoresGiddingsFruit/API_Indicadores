using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("CatTiposProd")]
    public class CatTiposProd
    {
        [Key]
        public short Tipo { get; set; }
        public string Descripcion { get; set; }
        public byte HPreEnfria { get; set; }
        public byte MPreEnfria { get; set; }
        public short Consecutivo { get; set; }
        public bool SeparaCto { get; set; }
        public string DescIngles { get; set; }
        public string DescripcionAd { get; set; }
        public string FraccionArancelaria { get; set; }
        public string CodigoSAT { get; set; }
        public string Cod_UniMed { get; set; }
        public string Cod_UniMedProd { get; set; }
        public string CodigoSATProd { get; set; }
    }
}
