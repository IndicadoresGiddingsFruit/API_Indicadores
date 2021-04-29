using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indicadores.Models
{
    [Table("CatLocalidades")]
    public class CatLocalidades
    {
        [Key]
        public string CodLocalidad { get; set; }
        public string Descripcion { get; set; }
        public string CodMunicipio { get; set; }
        public string CodEstado { get; set; }
        public string Area { get; set; }
    }
}
