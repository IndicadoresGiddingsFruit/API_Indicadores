using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Inventario
{
    [Table("MovtosAlmIndicadores")]
    public class MovtosAlmIndicadores
    {
        [Key]
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public Int16 Cod_Artic { get; set; }
        public DateTime Fecha { get; set; }
        public float Cantidad { get; set; }
        public int Cod_Mov { get; set; }
        public int IdAlmacen { get; set; }
        public string Temporada { get; set; }
        public string Observaciones { get; set; }
    }
}
