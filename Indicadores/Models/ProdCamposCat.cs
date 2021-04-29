using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Indicadores.Models
{
    [Table("ProdCamposCat")]
    public class ProdCamposCat
    {
        [Key]
        public string Cod_Prod { get; set; }
        public short? Cod_Empresa { get; set; }              
        public short? Cod_Campo { get; set; }
        public string Descripcion { get; set; }
        public short Tipo { get; set; }
        public short Producto { get; set; }
        public string Ubicacion { get; set; }
        public string GPS1 { get; set; }
        public string GPS2 { get; set; }
        public decimal? Hectareas { get; set; }
        public string codigo { get; set; }
        public string Colindancia1 { get; set; }
        public string Colindancia2 { get; set; }
        public string Colindancia3 { get; set; }
        public string Colindancia4 { get; set; }
        public short? CodZona { get; set; }
        public Nullable<short> IdArea { get; set; }
        public Nullable<decimal> CajasEstimadas { get; set; }
        public string PeriodoInicial { get; set; }
        public string PeriodoFinal { get; set; }
        public string Activo { get; set; }
        public short? IdZona { get; set; }
        public Nullable<double> Gps_Latitude { get; set; }
        public Nullable<double> Gps_Longitude { get; set; }
        public Nullable<double> Gps_Altitude { get; set; }
        public System.Guid rowguid { get; set; }
        public short? IdAgen { get; set; }
        public Nullable<int> IdAgenC { get; set; }
        public Nullable<short> IdAgenI { get; set; }
        public string CodLocalidad { get; set; }
        public Nullable<int> IdAcopio { get; set; }
        public Nullable<int> IdChofer { get; set; }
        public string Compras_Oportunidad { get; set; }
        public Nullable<int> CodArea { get; set; }
    }
}
