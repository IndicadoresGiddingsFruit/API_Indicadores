using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes.Visitas
{
    public class VisitasTotal
    {
        [Key]
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public int? TotalCampos { get; set; }

        public Int16? IdRegion { get; set; }
        public string Region { get; set; }
        public int? IdZona { get; set; }
        public string Zona { get; set; }
        public Double? TotalCamposVisit { get; set; }
        public string Eficiencia { get; set; }
        public string Efectividad { get; set; }
        public string Primer_visita { get; set; }
        public string Ultima_visita { get; set; }
        public int? VisitasDiarias { get; set; }
    }
}
