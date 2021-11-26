using ApiIndicadores.Models;
using ApiIndicadores.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiIndicadores.Models.Inventario;
using ApiIndicadores.Models.Catalogos;
using ApiIndicadores.Classes.Visitas;
using ApiIndicadores.Models.Auditoria;
using ApiIndicadores.Classes.Proyeccion;
using ApiIndicadores.Classes.Auditoria;

namespace ApiIndicadores.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
        }

        //auditoria
        public DbSet<AuditoriaCat> AuditoriaCat { get; set; }
        public DbSet<ProdAudInoc> ProdAudInoc { get; set; }
        public DbSet<ProdAudInocCampos> ProdAudInocCampos { get; set; }
        public DbSet<ProdLogAuditoria> ProdLogAuditoria { get; set; }
        public DbSet<ProdAudInocCat> ProdAudInocCat { get; set; }
        public DbSet<ProdLogAuditoriaFoto> ProdLogAuditoriaFoto { get; set; }
        
        public DbSet<AuditoriaClass> AuditoriaClass { get; set; }
        public DbSet<LogClass> LogClass { get; set; }


        //regiones
        public DbSet<tbZonasAgricolas> tbZonasAgricolas { get; set; }

        //zonas
        public DbSet<ProdZonasRastreoCat> ProdZonasRastreoCat { get; set; }



        public DbSet<SIPGUsuarios> SIPGUsuarios { get; set; }
        public DbSet<CatUsuariosA> CatUsuariosA { get; set; }
        public DbSet<CatSemanas> CatSemanas { get; set; }
        public DbSet<CatProductos> CatProductos { get; set; }
        public DbSet<CatTiposProd> CatTiposProd { get; set; }
        public DbSet<CatLocalidades> CatLocalidades { get; set; }
        public DbSet<ProdCamposCat> ProdCamposCat { get; set; }
      
        public DbSet<ProdProductoresCat> ProdProductoresCat { get; set; }
        public DbSet<ProdAgenteCat> ProdAgenteCat { get; set; }
        public DbSet<InfoCampoClass> InfoCampoClass { get; set; }

        public DbSet<MuestreosClass> MuestreosClass { get; set; }
        public DbSet<ProdMuestreo> ProdMuestreo { get; set; }
        public DbSet<ProdCalidadMuestreo> ProdCalidadMuestreo { get; set; }
        public DbSet<ProdMuestreoSector> ProdMuestreoSector { get; set; }
        public DbSet<ProdBloqueoTarjeta> ProdBloqueoTarjeta { get; set; }

        public DbSet<ProdAnalisis_Residuo> ProdAnalisis_Residuo { get; set; }
        public DbSet<AnalisisClass> AnalisisClass { get; set; }

        //Visitas
        public DbSet<ProdVisitasCab> ProdVisitasCab { get; set; }
        public DbSet<VisitasGraph> VisitasGraph { get; set; }
        public DbSet<VisitasTable> VisitasTable { get; set; }
        public DbSet<VisitasReport> VisitasReport { get; set; }
        public DbSet<VisitasTotal> VisitasTotal { get; set; }
        public DbSet<VisitasMes> VisitasMes { get; set; }


        //Validación Cartera

        public DbSet<Seguimiento_financ> Seguimiento_financ { get; set; }
        public DbSet<SeguimientoClass> SeguimientoClass { get; set; }
        public DbSet<EstatusFinanciamiento> EstatusFinanciamiento { get; set; }

        public DbSet<UV_ProdRecepcion> UV_ProdRecepcion { get; set; } 
        public DbSet<EvaluacionClass> EvaluacionClass { get; set; }
        public DbSet<RecepcionClass> RecepcionClass { get; set; }

        //Encuestas
        public DbSet<EncuestasCat> EncuestasCat { get; set; }
        public DbSet<EncuestasTipo> EncuestasTipo { get; set; }
        public DbSet<EncuestasDet> EncuestasDet { get; set; }
        public DbSet<EncuestasRes> EncuestasRes { get; set; }
        public DbSet<EncuestasRelacion> EncuestasRelacion { get; set; }
        public DbSet<EncuestasUsuarios> EncuestasUsuarios { get; set; }
        public DbSet<EncuestasLog> EncuestasLog { get; set; }
        public DbSet<EncuestasClass> EncuestasClass { get; set; }

        //Inventario
        public DbSet<MovimientosInventarioClass> MovimientosInventarioClass { get; set; }
        public DbSet<CatMovtosAlm> CatMovtosAlm { get; set; }
        public DbSet<CatArticulos> CatArticulos { get; set; }
        public DbSet<MovtosAlmIndicadores> MovtosAlmIndicadores { get; set; }
        public DbSet<CatUniMed> CatUniMed { get; set; }
        public DbSet<EntradasAlm> EntradasAlm { get; set; }
        public DbSet<SalidasAlm> SalidasAlm { get; set; }

        //Proyeccion ProyeccionClass
        public DbSet<ProyeccionClass> ProyeccionClass { get; set; }
        public DbSet<ProyeccionMesClass> ProyeccionMesClass { get; set; }
        public DbSet<ProyeccionTotalClass> ProyeccionTotalClass { get; set; }
        public DbSet<ProyeccionMesSemanaClass> ProyeccionMesSemanaClass { get; set; }


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
            modelBuilder.Entity<EncuestasClass>().HasNoKey();
            modelBuilder.Entity<EvaluacionClass>().HasNoKey();

            modelBuilder.Entity<SeguimientoClass>().HasNoKey();

            modelBuilder.Entity<MovimientosInventarioClass>().HasNoKey();

            modelBuilder.Entity<InfoCampoClass>().HasNoKey();

            modelBuilder.Entity<RecepcionClass>().HasNoKey();

            modelBuilder.Entity<ProyeccionClass>().HasNoKey();
            modelBuilder.Entity<ProyeccionMesClass>().HasNoKey();
            modelBuilder.Entity<ProyeccionTotalClass>().HasNoKey();
            modelBuilder.Entity<ProyeccionMesSemanaClass>().HasNoKey();

            modelBuilder.Entity<VisitasMes>().HasNoKey();            

        }
    }
}
