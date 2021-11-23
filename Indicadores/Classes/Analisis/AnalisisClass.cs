using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class AnalisisClass
    {
        [Key]
        public int Id { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Campo { get; set; }
        public string Sector { get; set; }
        public string Tipo { get; set; }
        public string Producto { get; set; }

        public string codigo { get; set; }
        public string Zona { get; set; }

        public string Fecha { get; set; }
        public string Fecha_envio { get; set; }
        public string Fecha_entrega { get; set; }

        public string ParteMuestreada { get; set; }
        public string Estatus { get; set; }
        public string DescEstatus { get; set; }

        public int? Num_analisis { get; set; }
        public string Laboratorio { get; set; }
        public string LiberacionUSA { get; set; }
        public string LiberacionEU { get; set; }
        public string Comentarios { get; set; }
        public Int16? IdAgen { get; set; }
        public string Temporada { get; set; }
        public string Traza { get; set; }
        public string Organico { get; set; }
        public string Folio { get; set; }
    }
}
