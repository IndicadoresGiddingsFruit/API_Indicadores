using System;

namespace ApiIndicadores.Classes.Expediente
{
    public class RendimientoExpedienteClass
    {
        public string Temporada { get; set; }
        public Int16 IdAgen { get; set; }
        public string Asesor { get; set; }
        public Int16 IdRegion { get; set; }
        public string Region { get; set; }
        public int? IdZona { get; set; }
        public string Zona { get; set; }
        public int CodTipo { get;set;}
        public string DescProducto { get; set; }
        public double Ha { get; set; }
        public double Entregado { get; set; }
        public int RenxHa { get; set; }
        public int? RendvsTA { get; set; }

    }
}
