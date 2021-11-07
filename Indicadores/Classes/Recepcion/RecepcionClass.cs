using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class RecepcionClass
    {
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public double? SaldoFinal { get; set; }
        public double? Financiamiento_otorgado { get; set; }
        public double? Financiamiento_recuperado { get; set; }
        public double? Pronostico { get; set; }
        public double? Entregado { get; set; }
        public double? Diferencia { get; set; }
        public double? PronosticoAA { get; set; }
        public double? DiferenciaAA { get; set; }
        public Byte? Semana { get; set; }
        public double? PronosticoSA { get; set; }
        public double? EntregadoSA { get; set; }
        public double? DiferenciaSA { get; set; }
    }
}
