using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class MovimientosInventarioClass
    {
        public string CodAlmacen { get; set; }
        public string DescAlmacen { get; set; }
        public string Almacen { get; set; }
        public int CodSubAlmacen { get; set; }
        public string DescSubAlmacen { get; set; }
        public string SubAlmacen { get; set; }
        public Int16 Cod_Artic { get; set; }
        public string DescArt { get; set; }
        public string descripcion { get; set; }
        public Int16 IdUso { get; set; }
        public Int16 Cod_lin { get; set; }
        public string DescLin { get; set; }

        public decimal InventIni { get; set; }
        public decimal Entradas { get; set; }
        public decimal Salidas { get; set; }
        public decimal CostoIni { get; set; }
        public decimal CostoEntradas { get; set; }
        public decimal CostoSalidas { get; set; }
        public decimal CantidadFinal { get; set; }
        public decimal CostoFinal { get; set; }
        public int SalidasxMermas { get; set; }
    }
}
