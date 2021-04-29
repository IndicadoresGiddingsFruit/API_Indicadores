using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Classes
{
    public class AnalisisClass
    {
        [Key]
        public int IdAnalisis_Residuo { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Campo { get; set; }
        public Int16? Sector { get; set; }
        public string Tipo { get; set; }
        public string Producto { get; set; }
        public string Zona { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Fecha_envio { get; set; }
        public DateTime? Fecha_entrega { get; set; }
        public string Estatus { get; set; }
        public int? Num_analisis { get; set; }
        public string Laboratorio { get; set; }
        public DateTime? LiberacionUSA { get; set; }
        public DateTime? LiberacionEU { get; set; }
        public string Comentarios { get; set; }
        public Int16? IdAgen { get; set; }
        public Int16? IdAgenC { get; set; }
        public Int16? IdAgenI { get; set; }

    }
}
