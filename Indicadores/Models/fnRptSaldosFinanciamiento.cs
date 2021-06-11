using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiIndicadores.Models
{
    [Table("fnRptSaldosFinanciamiento")]
    public class fnRptSaldosFinanciamiento
    {
        [Key]
        public string Cod_prod { get; set; }
        public string Nombre { get; set; }
        public decimal Financ { get; set; }
        public Nullable<decimal> Pagos { get; set; }
        public decimal Intereses { get; set; }
        public Nullable<decimal> Saldo { get; set; }
        public int Cajas { get; set; }
        public decimal Agroquimico { get; set; }
        public decimal DescAgq { get; set; }
        public Nullable<decimal> SaldoAGQ { get; set; }
        public Nullable<decimal> SaldoTemp { get; set; }
    }
}
