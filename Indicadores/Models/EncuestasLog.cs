using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("EncuestasLog")]
    public class EncuestasLog
    {
        [Key]
        public int Id { get; set; }
        public int IdAsingUsuario { get; set; }
        public int IdRelacion { get; set; }       
    }
}
