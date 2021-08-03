using ApiIndicadores.Models;
using ApiIndicadores.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
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
        public DbSet<VisitasReport> VisitasReport { get; set; }

        public DbSet<Seguimiento_financ> Seguimiento_financ { get; set; }
        public DbSet<SeguimientoClass> SeguimientoClass { get; set; }

        public DbSet<UV_ProdRecepcion> UV_ProdRecepcion { get; set; }
        public DbSet<ProdZonasRastreoCat> ProdZonasRastreoCat { get; set; }
        
        //Encuestas
        public DbSet<EncuestasCat> EncuestasCat { get; set; }
        public DbSet<EncuestasTipo> EncuestasTipo { get; set; }
        public DbSet<EncuestasDet> EncuestasDet { get; set; }
        public DbSet<EncuestasRes> EncuestasRes { get; set; }
        public DbSet<EncuestasRelacion> EncuestasRelacion { get; set; }
        public DbSet<EncuestasUsuarios> EncuestasUsuarios { get; set; }
        public DbSet<EncuestasLog> EncuestasLog { get; set; }

        //Empleados
        //public DbSet<Empleado> Empleado { get; set; }
        //public DbSet<Puesto> Puesto { get; set; }
        //public DbSet<Departamentos> Departamentos { get; set; }
        //public DbSet<Subacopio> Subacopio { get; set; }
        //public DbSet<Centro_Acopio> Centro_Acopio { get; set; }
        //public DbSet<Zonas> Zonas { get; set; }
        public DbSet<RespuestasTotal> RespuestasTotal { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RespuestasTotal>().HasNoKey();
            modelBuilder.Entity<ProdCamposCat>().HasKey(c => new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo });
            modelBuilder.Entity<MuestreosClass>().HasNoKey();
        }
    }
}
