using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Inventario
{
    [Table("CatArticulos")]
    public class CatArticulos
    {
        [Key]
        public Int16 Cod_Artic { get; set; }
        public Int16 Cod_Lin { get; set; }
        public byte Dig_Ver { get; set; }
        public string Descripcion { get; set; }
        public Int16 Cod_UniMed { get; set; }
        public byte TasaIVA { get; set; }
        public Int16 Cod_IngA { get; set; }
        public decimal PrecPrimCom { get; set; }
        public decimal PrecUltCom { get; set; }
        public Int16 CodGasto { get; set; }
        public string TipoM { get; set; }
        public bool EnvGenerico { get; set; }
        public Int16 EnvEtiqueta { get; set; }
        public Int16 Orden { get; set; }
        public decimal Porcentaje { get; set; }
        public Guid rowguid { get; set; }
        public Int16 IdUso { get; set; }
        public bool Fiscal { get; set; }     
        public string NombreComercial { get; set; }
        public int PxUnidadEmp { get; set; }
        public Int16 Cod_Lab { get; set; }
        public Int16 Cod_Presenta { get; set; }
        public Int16 Cod_UniEmp { get; set; }
        public Int16? PorcPxC { get; set; }
        public Int16? PorcCliEsp { get; set; }
        public string Activo { get; set; }
        public decimal? TasaIEPS { get; set; }
        public bool Caduca { get; set; }
        public string CodigoSAT { get; set; }
    }
}
