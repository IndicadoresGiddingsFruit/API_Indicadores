using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class EvaluacionClass
    {
        public string Asesor { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public string Campo { get; set; }
        public string Ubicacion { get; set; }
        public DateTime? Fecha_solicitud { get; set; }
        public DateTime? Inicio_cosecha { get; set; }
        public string Fecha_real{ get; set; }
        public DateTime? Fecha_analisis { get; set; }
        public string Estatus { get; set; }
        public int? Dias { get; set; }
        public string Analisis { get; set; }
    }
}
