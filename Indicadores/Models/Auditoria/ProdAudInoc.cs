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
        public string Cod_Prod { get; set; }
        public DateTime Fecha { get; set; }       
        public int IdZona { get; set; }
        public string Temporada { get; set; }
        public int IdNorma { get; set; }
    }
}
