using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indicadores.Models
{
    [Table("EncuestasRes")]
    public class EncuestasRes
    {
        [Key]
        public int Id { get; set; }
        public string Respuesta { get; set; }
    }
}

