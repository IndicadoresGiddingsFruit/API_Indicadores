﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Models
{
    [Table("ProdAnalisis_Residuo")]
    public class ProdAnalisis_Residuo
    {
        [Key]
        public int Id { get; set; }
        public string Cod_Prod { get; set; }
        public short? Cod_Campo { get; set; }
        public string CodZona { get; set; }
        public Nullable<System.DateTime> Fecha_envio { get; set; }
        public Nullable<System.DateTime> Fecha_entrega { get; set; }
        public string Estatus { get; set; }
        public Nullable<int> Num_analisis { get; set; }
        public string Laboratorio { get; set; }
        public string Comentarios { get; set; }
        public Nullable<short> IdAgen { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<int> Id_Muestreo { get; set; }
        public Nullable<System.DateTime> LiberacionUSA { get; set; }
        public Nullable<System.DateTime> LiberacionEU { get; set; }
        public Nullable<int> IdSector { get; set; }
        public short? Cod_Empresa { get; set; }
        public string Folio { get; set; }
        public string Temporada { get; set; }
    }
}