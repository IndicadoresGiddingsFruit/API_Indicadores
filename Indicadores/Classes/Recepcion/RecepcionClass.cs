using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class RecepcionClass
    {
        public string Temporada { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public Int16? IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public Decimal? SaldoFinal { get; set; }
        public Decimal? Financiamiento_otorgado { get; set; }
        public Decimal? Financiamiento_recuperado { get; set; }
        public double? Pronostico { get; set; }
        public double? EntregadoTotal { get; set; }
        public double? Entregado { get; set; }
        public double? Diferencia { get; set; }
        public double? PronosticoAA { get; set; }
        public double? DiferenciaAA { get; set; }
        public int? Semana { get; set; }
        public double? PronosticoSA { get; set; }
        public double? EntregadoSA { get; set; }
        public double? DiferenciaSA { get; set; }
    }
}
