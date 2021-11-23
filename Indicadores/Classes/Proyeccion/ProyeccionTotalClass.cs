using System;

namespace ApiIndicadores.Classes.Proyeccion
{
    public class ProyeccionTotalClass
    {
        public Int16? IdAgen { get; set; }
        public string Asesor { get; set; }
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; } 
        public int CamposCurva { get; set; }
        public double Pronostico { get; set; }
        public string Fecha { get; set; }
        public int? Diferencia { get; set; }
    }
}

