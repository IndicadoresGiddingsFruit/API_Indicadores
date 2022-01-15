using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models.Catalogos
{
    [Table("MunicipioSAT")]
    public class MunicipioSAT
    {
        [Key]
        public string CodMunicipio { get; set; }
        public string CodEstado { get; set; }
        public string Descripcion { get; set; }

    }
}
