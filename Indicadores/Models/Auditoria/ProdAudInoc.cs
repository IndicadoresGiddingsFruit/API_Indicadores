using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdAudInoc")]
    public class ProdAudInoc
    {
        [Key]
        public int Id { get; set; }
        public int IdAgen { get; set; }
        public DateTime Fecha { get; set; }
        public string Cod_Prod { get; set; }
        public Int16 Cod_Campo { get; set; }
        public int IdZona { get; set; }
        public string Temporada { get; set; }
        public DateTime? Fecha_termino { get;  set; }
    }
}
