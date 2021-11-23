using System;
using System.ComponentModel.DataAnnotations;

namespace ApiIndicadores.Classes.Visitas
{
    public class VisitasMes
    {
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public int TotalCampos { get; set; }
        public int NoMes { get; set; }
        public string Mes { get; set; }
        public Double? TotalCamposVisit { get; set; }
        public string Eficiencia { get; set; }
        public string Efectividad { get; set; }
        public string Primer_visita { get; set; }
        public string Ultima_visita { get; set; }
        public int? VisitasDiarias { get; set; }
        public Double? Promedio { get; set; }
    }
}
