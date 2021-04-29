using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("EncuestasCat")]
    public class EncuestasCat
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? Fecha_modificacion { get; set; }
        public string Estatus { get; set; }
        public int IdTipo { get; set; }
    }
}
