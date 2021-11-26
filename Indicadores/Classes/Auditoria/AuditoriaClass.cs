using System;
using System.ComponentModel.DataAnnotations;

namespace ApiIndicadores.Classes.Auditoria
{
    public class AuditoriaClass
    {
        [Key]
        public int Id { get; set; }
        public int IdAgen { get; set; }
        public string Asesor { get; set; }
        public string Cod_Prod { get; set; }
        public string Productor { get; set; }
        public string Cod_Campo { get; set; }
        public int IdZona { get; set; }
        public string Zona { get; set; }
        public int IdNorma { get; set; }
        public string Norma { get; set; }
        public DateTime Fecha  { get; set; }

}
}
