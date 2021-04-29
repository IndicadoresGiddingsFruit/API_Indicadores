using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("UV_ProdRecepcion")]
    public class UV_ProdRecepcion
    {
        [Key]
        //public string Empresa { get; set; }
        //public string FolioNota { get; set; }
        //public long Ticket { get; set; }
        
        //public System.DateTime Hora { get; set; }
        public string Cod_prod { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public string CodTipo { get; set; }
        public string DescProducto { get; set; }
        public short CodProducto { get; set; }
        public string DescVariedad { get; set; }
        public short CodTamano { get; set; }
        public string DescTamano { get; set; }
        public short CodClasificacion { get; set; }
        public string DescClasificacion { get; set; }
        public short CodEnvase { get; set; }
        public string DescEnvase { get; set; }
        public short CodEtiqueta { get; set; }
        public string Producto { get; set; }
        public Nullable<short> CajasRecibidas { get; set; }
        public short Rechazo { get; set; }
        public short Reembalaje { get; set; }
        public short Disponibles { get; set; }
        public Nullable<short> Cod_Campo { get; set; }
        public string DescCampo { get; set; }
        public Nullable<decimal> Hectareas { get; set; }
        public Nullable<double> Factor { get; set; }
        public string Temporada { get; set; }
        public Nullable<byte> Semana { get; set; }
        public Nullable<short> Cajas { get; set; }
        public Nullable<short> CantidadEnvase { get; set; }
        public string Grupo { get; set; }
        public Nullable<short> CodChofer { get; set; }
        public string Chofer { get; set; }
        public string Acopio { get; set; }
        public Nullable<int> CodAcopio { get; set; }
        public string Estatus { get; set; }
        public string CodEstatus { get; set; }
        public short CodEmpaque { get; set; }
        public string Empaque { get; set; }
        public string DescEtiqueta { get; set; }
        public string Mercado { get; set; }
        public Nullable<double> Convertidas { get; set; }
        public string Mes { get; set; }
        public string Yearr { get; set; }
        public string Zona { get; set; }
        public string Area { get; set; }
        public string Agente { get; set; }
        public string REGION { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public Nullable<bool> Organico { get; set; }
        public int Certificado { get; set; }
        public string Refrigerada { get; set; }
    }
}
