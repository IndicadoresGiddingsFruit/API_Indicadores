using System.ComponentModel.DataAnnotations;

namespace ApiIndicadores.Classes.Auditoria
{
    public class LogClass
    {
        [Key]
        public int Id { get; set; }
        public int Consecutivo { get; set; }
        public string NoPunto { get; set; }
        public string NoPuntoDesc { get; set; }
        public string PuntoControl { get; set; }
        public string PuntoControlDesc { get; set; }
        public int? Respondida { get; set; }
        public string Opcion { get; set; }
    }
}
