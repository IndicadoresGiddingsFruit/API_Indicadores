using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("ProdProductoresCat")]
    public class ProdProductoresCat
    {
        [Key]
        public string Cod_Prod { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string RFC { get; set; }
        public short CodZona { get; set; }
        public string Cta { get; set; }
        public string SCta { get; set; }
        public string SSCta { get; set; }
        public string SSSCta { get; set; }
        public short Cod_Alm { get; set; }
        public short Cod_Sub { get; set; }
        public string Cod_Prov { get; set; }
        public string Calle { get; set; }
        public string noExterior { get; set; }
        public string noInterior { get; set; }
        public string Colonia { get; set; }
        public string referencia { get; set; }
        public string CP { get; set; }
        public string Ciudad { get; set; }
        public string municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public string Correo { get; set; }
        public string Pagina { get; set; }
        public string Banco { get; set; }
        public string Sucursal { get; set; }
        public string CLABE { get; set; }
        public decimal TopeAutoFact { get; set; }
        public bool Activo { get; set; }
        public short ConsCampos { get; set; }
        public System.DateTime Fecha_Alta { get; set; }
        public string CtaD { get; set; }
        public string SCtaD { get; set; }
        public string SSCtaD { get; set; }
        public string SSSCtaD { get; set; }
        public string TipoCta { get; set; }
        public string pila { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public short tipopago { get; set; }
        public string AgenteZ { get; set; }
        public short TipoAnt { get; set; }
        public short IdArea { get; set; }
        public short CodMoneda { get; set; }
        public short IdAgen { get; set; }
        public string CtaM { get; set; }
        public string SCtaM { get; set; }
        public string SSCtaM { get; set; }
        public string SSSCtaM { get; set; }
        public string FinancCont { get; set; }
        public string Matcont { get; set; }
        public bool DesctoMat { get; set; }
        public short IdRango { get; set; }
        public bool Liquida { get; set; }
        public Nullable<bool> SalidaMat { get; set; }
        public Nullable<System.DateTime> FechaContrato { get; set; }
        public string Movil { get; set; }
        public string Nextel { get; set; }
        public string CtaAgro { get; set; }
        public string SCtaAgro { get; set; }
        public string SSCtaAgro { get; set; }
        public string SSSCtaAgro { get; set; }
        public string FinanAgroCont { get; set; }
        public string TipoProd { get; set; }
        public string CURP { get; set; }
        public string CalleP { get; set; }
        public string NumExteriorP { get; set; }
        public string NumInteriorP { get; set; }
        public string ColoniaP { get; set; }
        public string LocalidadP { get; set; }
        public string MunicipioP { get; set; }
        public string CPP { get; set; }
        public string NIP { get; set; }
        public Nullable<bool> Verifico_Domicilio { get; set; }
        public string EstadoP { get; set; }
    }
}
