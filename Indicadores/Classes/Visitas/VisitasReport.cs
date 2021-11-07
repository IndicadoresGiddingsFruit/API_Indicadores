using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Classes
{
    public class VisitasReport
    {
        [Key]       
        public int IdVisita { get; set; }
        public string Asesor { get; set; }
        public Int16? IdSector { get; set; }
        public string Cod_prod { get; set; }
        public string Productor { get; set; }
        public Int16? Cod_Campo { get; set; }
        public string Campo { get; set; }
        public string Tipo { get; set; }
        public string Producto { get; set; }
        public string Fecha { get; set; }
        public string Comentarios { get; set; }
        public string Atendio { get; set; }
        public string DescIncidencia { get; set; }
        public string Etapa { get; set; }                                
        public string Folio { get; set; }
    }
}
