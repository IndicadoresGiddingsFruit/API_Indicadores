using System;

namespace ApiIndicadores.Classes.Proyeccion
{
    public class ProyeccionMesSemanaClass
    {
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public int IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public string NoMes { get; set; }
        public string Mes { get; set; }
        public string Semana { get; set; }
        public int Cambios { get; set; }
        public double Pronostico { get; set; }
        public string Fecha { get; set; }
    }
}
