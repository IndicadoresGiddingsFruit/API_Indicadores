using System;

namespace ApiIndicadores.Classes.Expediente
{
    public class ProyeccionExpedienteClass
    {
        public string Temporada { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }        
        public double Pronostico { get; set; }
        public double PronosticoAA { get; set; }
        public double EntregadoSC { get; set; }
        public double EntregadoCC { get; set; }
        public double Asertividad { get; set; }
    }
}
