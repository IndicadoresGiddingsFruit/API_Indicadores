using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("EstatusFinanciamiento")]
    public class EstatusFinanciamiento
    {
        [Key]
        public int Id { get; set; }
        public string DescEstatus { get; set; }
    }
         
}
