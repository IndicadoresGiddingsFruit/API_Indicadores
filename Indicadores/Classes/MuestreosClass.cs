using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Indicadores.Classes
{
    public class MuestreosClass
    {
        [Key]
        public int? IdMuestreo { get; set; }
        public int? IdAnalisis_Residuo { get; set; }        
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Campo { get; set; }
        public decimal? Ha { get; set; }
        public string Ubicacion { get; set; }
        public string Telefono { get; set; }
        public string Liberacion { get; set; }
        public string Estatus { get; set; }
        public string Tarjeta { get; set; }
        public Int16? Sector { get; set; }
        public DateTime? Fecha_solicitud { get; set; }
        public DateTime? Fecha_ejecucion { get; set; }
        public Int16? IdAgen { get; set; }
        public string Asesor { get; set; }
        public Int16? IdRegion { get; set; }
        public Int16? IdAgenC { get; set; }
        public string AsesorC { get; set; }
        public string AsesorCS { get; set; }
        public Int16? IdAgenI { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Inicio_cosecha { get; set; }
        public string Incidencia { get; set; }
        public string Propuesta { get; set; }
        public string Compras_oportunidad { get; set; }
        public DateTime? Fecha_analisis { get; set; }
        public string Analisis { get; set; }

    }
}
