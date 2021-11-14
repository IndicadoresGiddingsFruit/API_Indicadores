using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Auditoria
{
    [Table("ProdAudInocCat")]
    public class ProdAudInocCat
    {
        [Key]
        public int Id { get; set; }
        public int Consecutivo { get; set; }
        public string NoPunto { get; set; }
        public string PuntoControl { get; set; }
        public string NoPuntoDesc { get; set; }
        public string PuntoControlDesc { get; set; }
        public string Criterio { get; set; }
        public string Nivel { get; set; }
        public string Justificacion { get; set; }
        public int IdAuditoriaCat { get; set; }
    }
}
