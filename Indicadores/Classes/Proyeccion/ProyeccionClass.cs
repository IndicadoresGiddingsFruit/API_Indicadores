using System;

namespace ApiIndicadores.Classes.Proyeccion
{
    public class ProyeccionClass
    {
        public Int16? IdAgen { get; set; }
        public string Asesor { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }     
        public int Cambios { get; set; }
        public double Pronostico { get; set; }
        public string Fecha { get; set; }
    }
}
