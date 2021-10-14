using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class SeguimientoClass
    { 
        public int Id { get; set; }       
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public short? IdAgen { get; set; }
        public string Asesor { get; set; }
        public int? Estatus { get; set; }
        public string DescEstatus { get; set; }
        public string Comentarios { get; set; }
        public DateTime? Fecha { get; set; }
        public int? dias { get; set; }
        public double? caja1 { get; set; }
        public double? caja2 { get; set; }
        public string SaldoFinal { get; set; }
        public int? Semana { get; set; }

    }
}
