using System;

namespace ApiIndicadores.Classes.Expediente
{
    public class VisitasExpedienteClass
    {
        public string Temporada { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public int TotalCampos { get; set; }
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public int TotalCamposVisit { get; set; }
        public Double Eficiencia { get; set; }
        public Double Efectividad { get; set; }
    }
}
