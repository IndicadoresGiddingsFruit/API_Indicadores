namespace ApiIndicadores.Classes.Auditoria
{
    public class AccionesCorrectivasClass
    {
        public int IdLogAC { get; set; }
        public int IdLog { get; set; }
        public int IdCatAuditoria { get; set; }
        public string NoPunto { get; set; }
        public string Nivel { get; set; }
        public string NoPuntoDesc { get; set; }
        public string PuntoControl { get; set; }
        public string PuntoControlDesc { get; set; }
        public string Justificacion { get; set; }
        public string Opcion { get; set; }
        public string isOpen { get; set; }
        public int? FotoAC { get; set; }
        public int? Dias { get; set; }
    }
}
