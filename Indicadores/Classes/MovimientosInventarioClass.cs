using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class MovimientosInventarioClass
    {
        public int CodAlmacen { get; set; }
        public string DescAlmacen { get; set; }
        public int Almacen { get; set; }
        public int CodSubAlmacen { get; set; }
        public string DescSubAlmacen { get; set; }
        public string SubAlmacen { get; set; }
        public int Cod_Artic { get; set; }
        public string DescArt { get; set; }
        public string descripcion { get; set; }
        public int IdUso { get; set; }
        public int Cod_lin { get; set; }
        public string DescLin { get; set; }
        public int InventIni { get; set; }
        public int Entradas { get; set; }
        public int Salidas { get; set; }
        public int CostoIni { get; set; }
        public int CostoEntradas { get; set; }
        public int CostoSalidas { get; set; }
        public int CantidadFinal { get; set; }
        public int CostoFinal { get; set; }
        public int SalidasxMermas { get; set; }
    }
}
