using ApiIndicadores.Classes;
using ApiIndicadores.Models;
using ApiIndicadores.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Context
{
    public class AppDBContextRH : DbContext
    {
        public AppDBContextRH(DbContextOptions<AppDBContextRH> options) : base(options)
        {
            Options = options;
        }
        public DbSet<SIPGUsuarios> SIPGUsuarios { get; set; }
        public DbSet<CatUsuariosA> CatUsuariosA { get; set; }
       

        //Empleados
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Puesto> Puesto { get; set; }
        public DbSet<Departamentos> Departamentos { get; set; }
        public DbSet<Subacopio> Subacopio { get; set; }
        public DbSet<Centro_Acopio> Centro_Acopio { get; set; }
        public DbSet<Zonas> Zonas { get; set; }
        public DbSet<RespuestasTotal> RespuestasTotal { get; set; }
        public DbContextOptions<AppDBContextRH> Options { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RespuestasTotal>().HasNoKey();
        }
    }
}
