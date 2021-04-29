﻿using Indicadores.Models;
using Indicadores.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }
        public DbSet<SIPGUsuarios> SIPGUsuarios { get; set; }
        public DbSet<CatUsuariosA> CatUsuariosA { get; set; }
        public DbSet<CatSemanas> CatSemanas { get; set; }
        public DbSet<CatProductos> CatProductos { get; set; }
        public DbSet<CatTiposProd> CatTiposProd { get; set; }
        public DbSet<CatLocalidades> CatLocalidades { get; set; }
        public DbSet<ProdCamposCat> ProdCamposCat { get; set; }
        public DbSet<ProdProductoresCat> ProdProductoresCat { get; set; }
        public DbSet<ProdAgenteCat> ProdAgenteCat { get; set; }

        public DbSet<MuestreosClass> MuestreosClass { get; set; }
        public DbSet<ProdMuestreo> ProdMuestreo { get; set; }
        public DbSet<ProdCalidadMuestreo> ProdCalidadMuestreo { get; set; }
        public DbSet<ProdMuestreoSector> ProdMuestreoSector { get; set; }

        public DbSet<ProdAnalisis_Residuo> ProdAnalisis_Residuo { get; set; }
        public DbSet<AnalisisClass> AnalisisClass { get; set; }

        public DbSet<ProdVisitasCab> ProdVisitasCab { get; set; }
        public DbSet<VisitasGraph> VisitasGraph { get; set; }
        public DbSet<VisitasTable> VisitasTable { get; set; }

        public DbSet<Seguimiento_financ> Seguimiento_financ { get; set; }
        public DbSet<SeguimientoClass> SeguimientoClass { get; set; }

        public DbSet<UV_ProdRecepcion> UV_ProdRecepcion { get; set; }
        public DbSet<ProdZonasRastreoCat> ProdZonasRastreoCat { get; set; }
        

        public DbSet<EncuestasCat> EncuestasCat { get; set; }
        public DbSet<EncuestasTipo> EncuestasTipo { get; set; }
        public DbSet<EncuestasDet> EncuestasDet { get; set; }
        public DbSet<EncuestasRes> EncuestasRes { get; set; }
        public DbSet<EncuestasRelacion> EncuestasRelacion { get; set; }
        public DbSet<EncuestasUsuarios> EncuestasUsuarios { get; set; }
        public DbSet<EncuestasLog> EncuestasLog { get; set; }
    }
}
