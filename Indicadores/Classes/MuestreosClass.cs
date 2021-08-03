using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class MuestreosClass
    {
        public int? IdMuestreo { get; set; }
        public int? IdAnalisis_Residuo { get; set; }
        public string Cod_Prod { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Productor { get; set; }
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
        public int? IdAgenC { get; set; }
        public string AsesorC { get; set; }
        public string AsesorCS { get; set; }
        public Int16? IdAgenI { get; set; }
        public DateTime? Inicio_cosecha { get; set; }
        public string Incidencia { get; set; }
        public string Propuesta { get; set; }
        public string Compras_oportunidad { get; set; }
        public DateTime? Fecha_analisis { get; set; }

        public string Zona { get; set; }
        public string Tipo { get; set; }
        public string Producto { get; set; }
        public string Folio { get; set; }
        public DateTime? Fecha_envio { get; set; }
        public DateTime? Fecha_entrega{ get; set; }
        public DateTime? LiberacionUSA { get; set; }
        public DateTime? LiberacionEU { get; set; }
        public string Analisis { get; set; }
        public int? Num_analisis { get; set; }
        public string Laboratorio { get; set; }
        public string Traza { get; set; }
        public string Temporada { get; set; }

        //public List<SectoresClass> listaSectores { get; set; }
    }

    public class SectoresClass
    {
        [Key]
        public int? IdMuestreo { get; set; }       
        public string Cod_Prod { get; set; }
        public Int16? Cod_Campo { get; set; }
        public int? IdSector { get; set; }
        public Int16? Sector { get; set; }
    }
}
