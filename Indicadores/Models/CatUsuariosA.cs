using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indicadores.Models
{
    [Table("CatUsuariosA")]
    public class CatUsuariosA
    {
        [Key]
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public bool Grupo { get; set; }
        public int CodGrupo { get; set; }
        public DateTime FechaExp { get; set; }
        public string Tipo { get; set; }
        public bool Expiro { get; set; }
        public Guid rowguid { get; set; }
        public string Completo { get; set; }
        public string correo { get; set; }
        public string tel { get; set; }
        public string nextel { get; set; }
    }
}
