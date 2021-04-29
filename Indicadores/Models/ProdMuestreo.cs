using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("ProdMuestreo")]
    public class ProdMuestreo
    {
        [Key]
        public int Id { get; set; }
        public short? Cod_Empresa { get; set; }
        public string Cod_Prod { get; set; }
        public short? Cod_Campo { get; set; }
        public Nullable<System.DateTime> Fecha_solicitud { get; set; }
        public string Telefono { get; set; }
        public Nullable<System.DateTime> Inicio_cosecha { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public string Liberacion { get; set; }
        public string Tarjeta { get; set; }
        public Nullable<System.DateTime> Fecha_ejecucion { get; set; }
        public Nullable<int> IdSector { get; set; }
        public Nullable<short> IdAgenI { get; set; }
        public Nullable<short> IdAgen_Tarjeta { get; set; }
        public string Liberar_Tarjeta { get; set; }
        public string Temporada { get; set; }
    }
}
