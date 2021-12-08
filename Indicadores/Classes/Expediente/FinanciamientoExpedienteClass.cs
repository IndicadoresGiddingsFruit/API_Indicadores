using System;

namespace ApiIndicadores.Classes.Expediente
{
    public class FinanciamientoExpedienteClass
    {
        public string Temporada { get; set; }
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public int Codigos { get; set; }
        public decimal Abono { get; set; }
        public decimal Cargo { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal Recuperacion { get; set; }
    }
}
