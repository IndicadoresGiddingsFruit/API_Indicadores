﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models.Catalogos
{
    [Table("tbZonasAgricolas")]
    public class tbZonasAgricolas
    {
        [Key]
        public int CodZona { get; set; }
        public string Descripcion { get; set; }
    }
}
