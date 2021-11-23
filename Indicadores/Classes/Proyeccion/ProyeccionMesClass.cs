using System;

namespace ApiIndicadores.Classes.Proyeccion
{
    public class ProyeccionMesClass
    {
        public Int16? IdAgen { get; set; }        
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Campo { get; set; }
        public string Tipo { get; set; }
        public string Producto { get; set; }
        public string NoMes { get; set; }
        public string Mes { get; set; }
        public int Cambios { get; set; }
        public double Pronostico { get; set; }
        public string Fecha {get; set;}
    }
}
