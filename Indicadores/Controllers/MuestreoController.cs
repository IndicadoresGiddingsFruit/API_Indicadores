using Indicadores.Classes;
using Indicadores.Context;
using Indicadores.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


namespace Indicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuestreoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MuestreoController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        string title = "", body = "";

        // GET: api/<MuestreoController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.ProdMuestreo.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<MuestreoController>/5
        [HttpGet("{id}/{idAgen}", Name = "GetMuestreo")]
        public ActionResult Get(int id, short idAgen)
        {
            try
            {
                IQueryable<MuestreosClass> item = null;
                if (id == 0)
                {
                    if (idAgen == 205)
                    {
                        item = (from m in (from m in _context.ProdAnalisis_Residuo
                                           group m by new
                                           {
                                               Cod_Empresa = m.Cod_Empresa,
                                               Cod_Prod = m.Cod_Prod,
                                               Cod_Campo = m.Cod_Campo,
                                               IdSector = m.IdSector
                                           } into x
                                           select new
                                           {
                                               Cod_Empresa = x.Key.Cod_Empresa,
                                               Cod_Prod = x.Key.Cod_Prod,
                                               Cod_Campo = x.Key.Cod_Campo,
                                               IdSector = x.Key.IdSector,
                                               Fecha = x.Max(m => m.Fecha)
                                           })

                                join an in _context.ProdAnalisis_Residuo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha } into AnalisisR
                                from an in AnalisisR.DefaultIfEmpty()

                                join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                                from mcam in MuestreoCam.DefaultIfEmpty()

                                join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                                from prod in MuestreoProd.DefaultIfEmpty()

                                join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                                from ms in MuestreoSc.DefaultIfEmpty()

                                join r in _context.ProdMuestreo on an.Id_Muestreo equals r.Id into MuestreoAn
                                from man in MuestreoAn.DefaultIfEmpty()

                                join a in _context.ProdAgenteCat on mcam.IdAgen equals a.IdAgen into MuestreoAgentes
                                from ageP in MuestreoAgentes.DefaultIfEmpty()

                                join cf in _context.ProdCalidadMuestreo on man.Id equals cf.Id_Muestreo into MuestreoCa
                                from mc in MuestreoCa.DefaultIfEmpty()

                                join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                                from ageC in MuestreoAgenC.DefaultIfEmpty()

                                join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                                from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                                join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                                from loc in MuestreoLoc.DefaultIfEmpty()

                                where an.Estatus != "L"

                                group m by new
                                {
                                    IdAnalisis_Residuo = an.Id,
                                    IdMuestreo = man.Id,
                                    Asesor = ageP.Abrev,
                                    Cod_Prod = m.Cod_Prod,
                                    Productor = prod.Nombre,
                                    Cod_Campo = m.Cod_Campo,
                                    Campo = mcam.Descripcion,
                                    Sector = (short)ms.Sector,
                                    Ha = mcam.Hectareas,
                                    Compras_oportunidad = mcam.Compras_Oportunidad,
                                    Fecha_solicitud = (DateTime)man.Fecha_solicitud,
                                    Inicio_cosecha = (DateTime)man.Inicio_cosecha,
                                    Ubicacion = loc.Descripcion,
                                    Telefono = man.Telefono,
                                    Liberacion = man.Liberacion,
                                    Fecha_ejecucion = (DateTime)man.Fecha_ejecucion,
                                    Analisis = an.Estatus,
                                    Calidad_fruta = mc.Estatus,
                                    IdAgenC = (short)mcam.IdAgenC,
                                    AsesorC = ageC.Abrev,
                                    AsesorCS = ageCS.Abrev,
                                    Tarjeta = man.Tarjeta,
                                    IdRegion = ageP.IdRegion,
                                    Fecha_analisis = m.Fecha
                                } into x
                                select new MuestreosClass()
                                {
                                    IdAnalisis_Residuo = x.Key.IdAnalisis_Residuo,
                                    IdMuestreo = x.Key.IdMuestreo,
                                    Asesor = x.Key.Asesor,
                                    Cod_Prod = x.Key.Cod_Prod,
                                    Productor = x.Key.Productor,
                                    Cod_Campo = x.Key.Cod_Campo,
                                    Campo = x.Key.Campo,
                                    Sector = (short)x.Key.Sector,
                                    Ha = x.Key.Ha,
                                    Compras_oportunidad = x.Key.Compras_oportunidad,
                                    Fecha_solicitud = (DateTime)x.Key.Fecha_solicitud,
                                    Inicio_cosecha = (DateTime)x.Key.Inicio_cosecha,
                                    Ubicacion = x.Key.Ubicacion,
                                    Telefono = x.Key.Telefono,
                                    Liberacion = x.Key.Liberacion,
                                    Fecha_ejecucion = (DateTime)x.Key.Fecha_ejecucion,
                                    Analisis = x.Key.Analisis,
                                    Estatus = x.Key.Calidad_fruta,
                                    IdAgenC = (short)x.Key.IdAgenC,
                                    AsesorC = x.Key.AsesorC,
                                    AsesorCS = x.Key.AsesorCS,
                                    Tarjeta = x.Key.Tarjeta,
                                    IdRegion = x.Key.IdRegion,
                                    Fecha_analisis = x.Key.Fecha_analisis
                                }).Distinct();
                    }
                    //else if (idAgen == 153 || idAgen == 281 || idAgen == 167 || idAgen == 182)
                    //{
                    //    item = (from m in (from m in _context.ProdMuestreo
                    //                       group m by new
                    //                       {
                    //                           Cod_Empresa = m.Cod_Empresa,
                    //                           Cod_Prod = m.Cod_Prod,
                    //                           Cod_Campo = m.Cod_Campo,
                    //                           IdSector = m.IdSector
                    //                       } into x
                    //                       select new
                    //                       {
                    //                           Cod_Empresa = x.Key.Cod_Empresa,
                    //                           Cod_Prod = x.Key.Cod_Prod,
                    //                           Cod_Campo = x.Key.Cod_Campo,
                    //                           IdSector = x.Key.IdSector,
                    //                           Fecha_solicitud = x.Max(m => m.Fecha_solicitud)
                    //                       })

                    //            join an in _context.ProdMuestreo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha_solicitud } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha_solicitud } into MuestreoR
                    //            from an in MuestreoR.DefaultIfEmpty()

                    //            join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                    //            from mcam in MuestreoCam.DefaultIfEmpty()

                    //            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                    //            from prod in MuestreoProd.DefaultIfEmpty()

                    //            join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                    //            from ms in MuestreoSc.DefaultIfEmpty()

                    //            join r in _context.ProdAnalisis_Residuo on an.Id equals r.Id_Muestreo into MuestreoAn
                    //            from man in MuestreoAn.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on an.IdAgen equals a.IdAgen into MuestreoAgentes
                    //            from ageP in MuestreoAgentes.DefaultIfEmpty()

                    //            join cf in _context.ProdCalidadMuestreo on an.Id equals cf.Id_Muestreo into MuestreoCa
                    //            from mc in MuestreoCa.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                    //            from ageC in MuestreoAgenC.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                    //            from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                    //            join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                    //            from loc in MuestreoLoc.DefaultIfEmpty()

                    //            where man.Estatus != "L"
                    //            select new MuestreosClass
                    //            {
                    //                IdAnalisis_Residuo = man.Id,
                    //                IdMuestreo = an.Id,
                    //                Asesor = ageP.Abrev,
                    //                Cod_Prod = m.Cod_Prod,
                    //                Productor = prod.Nombre,
                    //                Cod_Campo = m.Cod_Campo,
                    //                Campo = mcam.Descripcion,
                    //                Sector = (short)ms.Sector,
                    //                Ha = mcam.Hectareas,
                    //                Compras_oportunidad = mcam.Compras_Oportunidad,
                    //                Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                    //                Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                    //                Ubicacion = loc.Descripcion,
                    //                Telefono = an.Telefono,
                    //                Liberacion = an.Liberacion,
                    //                Fecha_ejecucion = (DateTime)an.Fecha_ejecucion,
                    //                Analisis = man.Estatus,
                    //                Calidad_fruta = mc.Estatus,
                    //                IdAgenC = (short)mcam.IdAgenC,
                    //                AsesorC = ageC.Abrev,
                    //                AsesorCS = ageCS.Abrev,
                    //                Tarjeta = an.Tarjeta,
                    //                IdRegion = ageP.IdRegion,
                    //                Fecha_analisis = man.Fecha
                    //            }).Distinct();
                    //}
                    //else if (idAgen == 1)
                    //{
                    //    item = (from m in (from m in _context.ProdMuestreo
                    //                       group m by new
                    //                       {
                    //                           Cod_Empresa = m.Cod_Empresa,
                    //                           Cod_Prod = m.Cod_Prod,
                    //                           Cod_Campo = m.Cod_Campo,
                    //                           IdSector = m.IdSector
                    //                       } into x
                    //                       select new
                    //                       {
                    //                           Cod_Empresa = x.Key.Cod_Empresa,
                    //                           Cod_Prod = x.Key.Cod_Prod,
                    //                           Cod_Campo = x.Key.Cod_Campo,
                    //                           IdSector = x.Key.IdSector,
                    //                           Fecha_solicitud = x.Max(m => m.Fecha_solicitud)
                    //                       })

                    //            join an in _context.ProdMuestreo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha_solicitud } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha_solicitud } into MuestreoR
                    //            from an in MuestreoR.DefaultIfEmpty()

                    //            join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                    //            from mcam in MuestreoCam.DefaultIfEmpty()

                    //            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                    //            from prod in MuestreoProd.DefaultIfEmpty()

                    //            join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                    //            from ms in MuestreoSc.DefaultIfEmpty()

                    //            join r in _context.ProdAnalisis_Residuo on an.Id equals r.Id_Muestreo into MuestreoAn
                    //            from man in MuestreoAn.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on an.IdAgen equals a.IdAgen into MuestreoAgentes
                    //            from ageP in MuestreoAgentes.DefaultIfEmpty()

                    //            join cf in _context.ProdCalidadMuestreo on an.Id equals cf.Id_Muestreo into MuestreoCa
                    //            from mc in MuestreoCa.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                    //            from ageC in MuestreoAgenC.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                    //            from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                    //            join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                    //            from loc in MuestreoLoc.DefaultIfEmpty()

                    //            where man.Estatus != "L" && (ageP.IdRegion == 1 || ageP.IdRegion == 3 || ageP.IdRegion == 4 || ageP.IdRegion == 5)
                    //            select new MuestreosClass
                    //            {
                    //                IdAnalisis_Residuo = man.Id,
                    //                IdMuestreo = an.Id,
                    //                Asesor = ageP.Abrev,
                    //                Cod_Prod = m.Cod_Prod,
                    //                Productor = prod.Nombre,
                    //                Cod_Campo = m.Cod_Campo,
                    //                Campo = mcam.Descripcion,
                    //                Sector = (short)ms.Sector,
                    //                Ha = mcam.Hectareas,
                    //                Compras_oportunidad = mcam.Compras_Oportunidad,
                    //                Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                    //                Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                    //                Ubicacion = loc.Descripcion,
                    //                Telefono = an.Telefono,
                    //                Liberacion = an.Liberacion,
                    //                Fecha_ejecucion = (DateTime)an.Fecha_ejecucion,
                    //                Analisis = man.Estatus,
                    //                Calidad_fruta = mc.Estatus,
                    //                IdAgenC = (short)mcam.IdAgenC,
                    //                AsesorC = ageC.Abrev,
                    //                AsesorCS = ageCS.Abrev,
                    //                Tarjeta = an.Tarjeta,
                    //                IdRegion = ageP.IdRegion,
                    //                Fecha_analisis = man.Fecha
                    //            }).Distinct();
                    //}
                    //else if (idAgen == 5)
                    //{
                    //    item = (from m in (from m in _context.ProdMuestreo
                    //                       group m by new
                    //                       {
                    //                           Cod_Empresa = m.Cod_Empresa,
                    //                           Cod_Prod = m.Cod_Prod,
                    //                           Cod_Campo = m.Cod_Campo,
                    //                           IdSector = m.IdSector
                    //                       } into x
                    //                       select new
                    //                       {
                    //                           Cod_Empresa = x.Key.Cod_Empresa,
                    //                           Cod_Prod = x.Key.Cod_Prod,
                    //                           Cod_Campo = x.Key.Cod_Campo,
                    //                           IdSector = x.Key.IdSector,
                    //                           Fecha_solicitud = x.Max(m => m.Fecha_solicitud)
                    //                       })

                    //            join an in _context.ProdMuestreo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha_solicitud } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha_solicitud } into MuestreoR
                    //            from an in MuestreoR.DefaultIfEmpty()

                    //            join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                    //            from mcam in MuestreoCam.DefaultIfEmpty()

                    //            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                    //            from prod in MuestreoProd.DefaultIfEmpty()

                    //            join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                    //            from ms in MuestreoSc.DefaultIfEmpty()

                    //            join r in _context.ProdAnalisis_Residuo on an.Id equals r.Id_Muestreo into MuestreoAn
                    //            from man in MuestreoAn.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on an.IdAgen equals a.IdAgen into MuestreoAgentes
                    //            from ageP in MuestreoAgentes.DefaultIfEmpty()

                    //            join cf in _context.ProdCalidadMuestreo on an.Id equals cf.Id_Muestreo into MuestreoCa
                    //            from mc in MuestreoCa.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                    //            from ageC in MuestreoAgenC.DefaultIfEmpty()

                    //            join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                    //            from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                    //            join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                    //            from loc in MuestreoLoc.DefaultIfEmpty()

                    //            where man.Estatus != "L" && ageP.IdRegion == 2
                    //            select new MuestreosClass
                    //            {
                    //                IdAnalisis_Residuo = man.Id,
                    //                IdMuestreo = an.Id,
                    //                Asesor = ageP.Abrev,
                    //                Cod_Prod = m.Cod_Prod,
                    //                Productor = prod.Nombre,
                    //                Cod_Campo = m.Cod_Campo,
                    //                Campo = mcam.Descripcion,
                    //                Sector = (short)ms.Sector,
                    //                Ha = mcam.Hectareas,
                    //                Compras_oportunidad = mcam.Compras_Oportunidad,
                    //                Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                    //                Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                    //                Ubicacion = loc.Descripcion,
                    //                Telefono = an.Telefono,
                    //                Liberacion = an.Liberacion,
                    //                Fecha_ejecucion = (DateTime)an.Fecha_ejecucion,
                    //                Analisis = man.Estatus,
                    //                Calidad_fruta = mc.Estatus,
                    //                IdAgenC = (short)mcam.IdAgenC,
                    //                AsesorC = ageC.Abrev,
                    //                AsesorCS = ageCS.Abrev,
                    //                Tarjeta = an.Tarjeta,
                    //                IdRegion = ageP.IdRegion,
                    //                Fecha_analisis = man.Fecha
                    //            }).Distinct();
                    //}
                    else
                    {
                        item = (from m in (from m in _context.ProdMuestreo
                                       group m by new
                                       {
                                           Cod_Empresa = m.Cod_Empresa,
                                           Cod_Prod = m.Cod_Prod,
                                           Cod_Campo = m.Cod_Campo,
                                           IdSector = m.IdSector
                                       } into x
                                       select new
                                       {
                                           Cod_Empresa = x.Key.Cod_Empresa,
                                           Cod_Prod = x.Key.Cod_Prod,
                                           Cod_Campo = x.Key.Cod_Campo,
                                           IdSector = x.Key.IdSector,
                                           Fecha_solicitud = x.Max(m => m.Fecha_solicitud)
                                       })

                            join an in _context.ProdMuestreo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha_solicitud } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha_solicitud } into MuestreoR
                            from an in MuestreoR.DefaultIfEmpty()

                            join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                            from mcam in MuestreoCam.DefaultIfEmpty()

                            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                            from prod in MuestreoProd.DefaultIfEmpty()

                            join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                            from ms in MuestreoSc.DefaultIfEmpty()

                            join r in _context.ProdAnalisis_Residuo on an.Id equals r.Id_Muestreo into MuestreoAn
                            from man in MuestreoAn.DefaultIfEmpty()

                            join a in _context.ProdAgenteCat on an.IdAgen equals a.IdAgen into MuestreoAgentes
                            from ageP in MuestreoAgentes.DefaultIfEmpty()

                            join cf in _context.ProdCalidadMuestreo on an.Id equals cf.Id_Muestreo into MuestreoCa
                            from mc in MuestreoCa.DefaultIfEmpty()

                            join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                            from ageC in MuestreoAgenC.DefaultIfEmpty()

                            join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                            from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                            join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                            from loc in MuestreoLoc.DefaultIfEmpty()

                            where man.Estatus != "L" && (mcam.IdAgen == idAgen || mcam.IdAgenC == idAgen || mcam.IdAgenI == idAgen) //&& (cf.Estatus != "1" || cf.Estatus != "2") && m.Tarjeta != "S"                           

                            select new MuestreosClass
                            {
                                IdMuestreo = an.Id,
                                IdAgen=an.IdAgen,
                                Cod_Prod = m.Cod_Prod,
                                Productor = prod.Nombre,
                                Cod_Campo = m.Cod_Campo,
                                Campo = mcam.Descripcion,
                                Sector=ms.Sector,
                                Compras_oportunidad = mcam.Compras_Oportunidad,
                                Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                                Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                                Ubicacion = loc.Descripcion,
                                Telefono = an.Telefono,
                                Liberacion = an.Liberacion,                               
                                IdAgenI = an.IdAgenI,
                                Fecha_ejecucion = an.Fecha_ejecucion,
                                IdAgenC = mc.IdAgen,
                                Estatus = mc.Estatus,
                                Incidencia=mc.Incidencia,
                                Propuesta=mc.Propuesta
                            }).Distinct();
                    }        
                }
                else
                {
                    item = (from m in (from m in _context.ProdMuestreo
                                       group m by new
                                       {
                                           Cod_Empresa = m.Cod_Empresa,
                                           Cod_Prod = m.Cod_Prod,
                                           Cod_Campo = m.Cod_Campo,
                                           IdSector = m.IdSector
                                       } into x
                                       select new
                                       {
                                           Cod_Empresa = x.Key.Cod_Empresa,
                                           Cod_Prod = x.Key.Cod_Prod,
                                           Cod_Campo = x.Key.Cod_Campo,
                                           IdSector = x.Key.IdSector,
                                           Fecha_solicitud = x.Max(m => m.Fecha_solicitud)
                                       })

                            join an in _context.ProdMuestreo on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo, m.IdSector, m.Fecha_solicitud } equals new { an.Cod_Empresa, an.Cod_Prod, an.Cod_Campo, an.IdSector, an.Fecha_solicitud } into MuestreoR
                            from an in MuestreoR.DefaultIfEmpty()

                            join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                            from mcam in MuestreoCam.DefaultIfEmpty()

                            join p in _context.ProdProductoresCat on mcam.Cod_Prod equals p.Cod_Prod into MuestreoProd
                            from prod in MuestreoProd.DefaultIfEmpty()

                            join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                            from loc in MuestreoLoc.DefaultIfEmpty()

                            where an.Id == id
                            select new MuestreosClass
                            {
                                IdMuestreo = an.Id,
                                Cod_Prod = m.Cod_Prod,
                                Productor = prod.Nombre,
                                Cod_Campo = m.Cod_Campo,
                                Campo = mcam.Descripcion,
                                Compras_oportunidad = mcam.Compras_Oportunidad,
                                Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                                Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                                Ubicacion = loc.Descripcion,
                                Telefono = an.Telefono,
                                Liberacion = an.Liberacion,
                                IdAgen = an.IdAgen,
                                IdAgenI = an.IdAgenI,
                                Fecha_ejecucion = an.Fecha_ejecucion
                            }).Distinct();
                }
                return Ok(item.OrderByDescending(x => x.Fecha_solicitud).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<MuestreoController>
        [HttpPost]
        public ActionResult Post([FromBody] ProdMuestreo model)
        {
            try
            {                
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                var modeloExistente = _context.ProdMuestreo.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Temporada== catSemanas.Temporada);
                if (modeloExistente == null)
                {
                    var usuario = _context.SIPGUsuarios.Where(x => x.IdAgen == model.IdAgen).First();

                    if (usuario.Tipo == "P")
                    {
                        model.Liberacion = "S";
                    }
                    model.Fecha_solicitud = DateTime.Now;
                    model.Temporada = catSemanas.Temporada;
                    _context.ProdMuestreo.Add(model);
                    _context.SaveChanges();

                    title = "Nuevo muestreo solicitado";
                    body = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    notificaciones.SendNotificationJSON(title, body);

                    return CreatedAtRoute("GetMuestreo", new { id = model.Id, idAgen = 0 }, model);
                }
                else
                {
                    return BadRequest("La solicitud ya existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<MuestreoController>/5
        //fecha_ejecucion ---------- Reasignar codigo
        [HttpPut("{id}/{idAgen}/{sector}/{tipo}")]
        public ActionResult Put(int id, short idAgen, short sector, string tipo, [FromBody] ProdMuestreo model)
        {
            try
            {
                if (tipo == "")
                {
                    ProdMuestreoSector prodMuestreoSector = new ProdMuestreoSector();
                    int IdMuestreoSector = 0;

                    var item = _context.ProdMuestreo.Find(id);
                    if (item.Id == id)
                    {
                        var model_sector = _context.ProdMuestreoSector.Where(x => x.Cod_Prod == item.Cod_Prod && x.Cod_Campo == item.Cod_Campo && x.Sector == sector).FirstOrDefault();
                        if (model_sector == null)
                        {
                            prodMuestreoSector.Cod_Prod = item.Cod_Prod;
                            prodMuestreoSector.Cod_Campo = item.Cod_Campo;
                            prodMuestreoSector.Sector = sector;
                            _context.ProdMuestreoSector.Add(prodMuestreoSector);
                            _context.SaveChanges();

                            IdMuestreoSector = prodMuestreoSector.id;
                        }
                        else
                        {
                            IdMuestreoSector = model_sector.id;
                        }

                        if (IdMuestreoSector != 0)
                        {
                            item.IdSector = IdMuestreoSector;
                            item.IdAgenI = idAgen;
                            item.Fecha_ejecucion = model.Fecha_ejecucion;
                            title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                            body = "Fecha de muestreo agregada: " +  model.Fecha_ejecucion;
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                }

                else
                {
                    var model_campo = _context.ProdCamposCat.FirstOrDefault(x => x.Cod_Prod == model.Cod_Prod && x.Cod_Campo == model.Cod_Campo);
                    if (tipo == "P")
                    {
                        model_campo.IdAgen = idAgen;
                    }
                    else if (tipo == "C")
                    {
                        model_campo.IdAgenC = idAgen;
                    }
                    else if (tipo == "I")
                    {
                        model_campo.IdAgenI = idAgen;
                    }
                    //_context.SaveChanges();

                    title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                    body = "Se le ha asignado un nuevo código";
                }
                notificaciones.SendNotificationJSON(title, body);
                _context.SaveChanges();
                return CreatedAtRoute("GetMuestreo", new { id = 0, idAgen = idAgen }, model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Liberar solicitud - Calidad
        [HttpPut("{id}/{idAgen}")]
        public ActionResult Put(int id, short idAgen, [FromBody] ProdCalidadMuestreo model)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);

                if (muestreo.Id == id)
                {
                    if (model.Estatus != null)
                    {
                        var item_calidad = _context.ProdCalidadMuestreo.Where(x => x.Id_Muestreo == id).FirstOrDefault();
                        if (item_calidad == null)
                        {
                            ProdCalidadMuestreo prodCalidadMuestreo = new ProdCalidadMuestreo();
                            prodCalidadMuestreo.Estatus = model.Estatus;
                            prodCalidadMuestreo.Fecha = DateTime.Now;
                            prodCalidadMuestreo.Incidencia = model.Incidencia;
                            prodCalidadMuestreo.Propuesta = model.Propuesta;
                            prodCalidadMuestreo.IdAgen = idAgen;
                            prodCalidadMuestreo.Id_Muestreo = id;
                            _context.ProdCalidadMuestreo.Add(prodCalidadMuestreo);
                        }
                        else
                        {
                            item_calidad.Estatus = model.Estatus;
                            item_calidad.Fecha = DateTime.Now;
                            item_calidad.Incidencia = model.Incidencia;
                            item_calidad.Propuesta = model.Propuesta;
                            item_calidad.IdAgen = idAgen;
                        }

                        _context.SaveChanges();

                        if (model.Estatus == "1")
                        {
                            muestreo.Tarjeta = "S";
                        }
                        else if (model.Estatus == "2")
                        {
                            muestreo.Tarjeta = "N";
                        }
                        else if (model.Estatus == "3")
                        {
                            muestreo.Tarjeta = "N";
                        }

                        _context.SaveChanges();

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Calidad evaluada: estatus " + model;
                    }
                    else
                    {
                        muestreo.Liberacion = "S";

                        title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                        body = "Solicitud de muestreo liberada por producción " + model;
                    }

                    notificaciones.SendNotificationJSON(title, body);
                    _context.SaveChanges();
                    return CreatedAtRoute("GetMuestreo", new { id = 0, idAgen = idAgen }, muestreo);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<MuestreoController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.FirstOrDefault(m => m.Id == id);
                if (muestreo != null)
                {
                    _context.ProdMuestreo.Remove(muestreo);
                    _context.SaveChanges();
                    return Ok(id);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
