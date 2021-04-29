using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Classes
{
    public class SeguimientoClass
    {
            public int Id { get; set; }
            public short? IdAgen { get; set; }
            public string Asesor { get; set; }
            public string Cod_Prod { get; set; }
            public string Productor { get; set; }
            public string Estatus { get; set; }
            public string Comentarios { get; set; }
            public string AP { get; set; }
            public Int16? Cod_Campo { get; set; }
            public string Campo { get; set; }
            public string Fecha { get; set; }
            public int? dias { get; set; }
            public double? caja1 { get; set; }
            public double? caja2 { get; set; }
            public short? IdRegion { get; set; }
            public short? IdAgenS { get; set; }
            public DateTime? Fecha_Up { get; set; }
            public string SaldoFinal { get; set; }
            public byte? Semana { get; set; }
        
    }
}
