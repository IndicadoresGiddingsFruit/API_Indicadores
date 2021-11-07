using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Inventario
{
    [Table("SalidasAlm")]
    public class SalidasAlm
    {
        [Key]
        public int Id { get; set; }
        public int IdEntrada { get; set; }
        public float Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public int Cod_mov { get; set; }
        public int IdUsuario { get; set; }
    }
}
