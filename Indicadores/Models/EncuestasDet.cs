using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models
{
    [Table("EncuestasDet")]
    public class EncuestasDet
    {
        [Key]
        public int? Id { get; set; }
        public int? IdEncuesta { get; set; }
        public string Pregunta { get; set; }
    }
}
