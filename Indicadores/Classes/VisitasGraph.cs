using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Classes
{
    public class VisitasGraph
    {
        [Key]
        public string Mes { get; set; }
        public int Total { get; set; }
        
    }
}
