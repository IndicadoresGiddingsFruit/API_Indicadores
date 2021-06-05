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
                var catSemanas = _context.CatSemanas.FirstOrDefault(m => DateTime.Now >= m.Inicio && DateTime.Now <= m.Fin);

                IQueryable<MuestreosClass> analisis = null;
                IQueryable<MuestreosClass> item = (from m in (from m in _context.ProdMuestreo
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

                                                   join al in (from m in _context.ProdAnalisis_Residuo
                                                               group m by new
                                                               {
                                                                   Cod_Empresa = m.Cod_Empresa,
                                                                   Cod_Prod = m.Cod_Prod,
                                                                   Cod_Campo = m.Cod_Campo
                                                               } into x
                                                               select new
                                                               {
                                                                   Cod_Empresa = x.Key.Cod_Empresa,
                                                                   Cod_Prod = x.Key.Cod_Prod,
                                                                   Cod_Campo = x.Key.Cod_Campo,
                                                                   Num_analisis = x.Max(m => m.Num_analisis),
                                                                   Fecha = x.Max(m => m.Fecha)
                                                               }) on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { al.Cod_Empresa, al.Cod_Prod, al.Cod_Campo } into Analisis
                                                   from al in Analisis.DefaultIfEmpty()

                                                   join man in _context.ProdAnalisis_Residuo on new { al.Cod_Empresa, al.Cod_Prod, al.Cod_Campo, al.Num_analisis, al.Fecha } equals new { man.Cod_Empresa, man.Cod_Prod, man.Cod_Campo, man.Num_analisis, man.Fecha } into Analisis2
                                                   from man in Analisis2.DefaultIfEmpty()

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

                                                   join t in _context.CatTiposProd on new { mcam.Tipo } equals new { t.Tipo } into Tipos
                                                   from t in Tipos.DefaultIfEmpty()

                                                   join p in _context.CatProductos on new { mcam.Tipo, mcam.Producto } equals new { p.Tipo, p.Producto } into Productos
                                                   from p in Productos.DefaultIfEmpty()

                                                   join z in _context.ProdZonasRastreoCat on man.CodZona equals z.Codigo into Zonas
                                                   from z in Zonas.DefaultIfEmpty()

                                                   where man.Estatus != "L" && an.Temporada == catSemanas.Temporada
                                                   select new MuestreosClass
                                                   {
                                                       IdAnalisis_Residuo = man.Id,
                                                       IdMuestreo = an.Id,
                                                       IdAgen = mcam.IdAgen,
                                                       Asesor = ageP.Abrev,
                                                       Cod_Prod = m.Cod_Prod,
                                                       Productor = prod.Nombre,
                                                       Cod_Campo = m.Cod_Campo,
                                                       Campo = mcam.Descripcion,
                                                       Sector = (short)ms.Sector,
                                                       Ha = mcam.Hectareas,
                                                       Compras_oportunidad = mcam.Compras_Oportunidad,
                                                       Tipo = t.Descripcion,
                                                       Producto = p.Descripcion,
                                                       Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                                                       Inicio_cosecha = (DateTime)an.Inicio_cosecha,
                                                       Ubicacion = loc.Descripcion,
                                                       Telefono = an.Telefono,
                                                       Liberacion = an.Liberacion,
                                                       Fecha_ejecucion = (DateTime)an.Fecha_ejecucion,
                                                       Analisis = man.Estatus,
                                                       IdAgenI = mcam.IdAgenI,
                                                       Estatus = mc.Estatus,
                                                       IdAgenC = (short)mcam.IdAgenC,
                                                       AsesorC = ageC.Abrev,
                                                       AsesorCS = ageCS.Abrev,
                                                       Tarjeta = an.Tarjeta,
                                                       IdRegion = ageP.IdRegion,
                                                       Fecha_analisis = man.Fecha,
                                                       //Folio = man.Folio,
                                                       //Zona = z.DescZona,
                                                       //Fecha_envio = man.Fecha_envio,
                                                       //Fecha_entrega = man.Fecha_entrega,
                                                       //LiberacionUSA = man.LiberacionUSA,
                                                       //LiberacionEU = man.LiberacionEU,
                                                       //Num_analisis = man.Num_analisis,
                                                       //Laboratorio = man.Laboratorio,
                                                       //Traza = man.Traza
                                                   }).Distinct();

                if (idAgen == 205)
                {
                    analisis = (from m in _context.ProdMuestreo

                                join a in _context.ProdAgenteCat on m.IdAgen equals a.IdAgen into MuestreoAgentes
                                from ageP in MuestreoAgentes.DefaultIfEmpty()

                                join p in _context.ProdProductoresCat on m.Cod_Prod equals p.Cod_Prod into MuestreoProd
                                from prod in MuestreoProd.DefaultIfEmpty()

                                join r in _context.ProdAnalisis_Residuo on m.Id equals r.Id_Muestreo into MuestreoAn
                                from man in MuestreoAn.DefaultIfEmpty()

                                join s in _context.ProdMuestreoSector on m.IdSector equals s.id into MuestreoSc
                                from ms in MuestreoSc.DefaultIfEmpty()

                                join cf in _context.ProdCalidadMuestreo on m.Id equals cf.Id_Muestreo into MuestreoCa
                                from mc in MuestreoCa.DefaultIfEmpty()

                                join c in _context.ProdCamposCat on new { m.Cod_Empresa, m.Cod_Prod, m.Cod_Campo } equals new { c.Cod_Empresa, c.Cod_Prod, c.Cod_Campo } into MuestreoCam
                                from mcam in MuestreoCam.DefaultIfEmpty()

                                join a in _context.ProdAgenteCat on mc.IdAgen equals a.IdAgen into MuestreoAgenC
                                from ageC in MuestreoAgenC.DefaultIfEmpty()

                                join a in _context.ProdAgenteCat on mcam.IdAgenC equals a.IdAgen into MuestreoAgenSC
                                from ageCS in MuestreoAgenSC.DefaultIfEmpty()

                                join l in _context.CatLocalidades on mcam.CodLocalidad equals l.CodLocalidad into MuestreoLoc
                                from loc in MuestreoLoc.DefaultIfEmpty()

                                join t in _context.CatTiposProd on new { mcam.Tipo } equals new { t.Tipo } into Tipos
                                from t in Tipos.DefaultIfEmpty()

                                join p in _context.CatProductos on new { t.Tipo, mcam.Producto } equals new { p.Tipo,p.Producto } into Productos
                                from p in Productos.DefaultIfEmpty()

                                join z in _context.ProdZonasRastreoCat on man.CodZona equals z.Codigo into Zonas
                                from z in Zonas.DefaultIfEmpty()

                                where m.Temporada == catSemanas.Temporada && man.Fecha == (from c in _context.ProdAnalisis_Residuo where c.Cod_Prod == man.Cod_Prod select c).Max(c => c.Fecha) && man.Estatus == null
                                select new MuestreosClass
                                {
                                    IdMuestreo = m.Id,
                                    IdAgen = mcam.IdAgen,
                                    Asesor = ageP.Abrev,
                                    Cod_Prod = m.Cod_Prod,
                                    Productor = prod.Nombre,
                                    Cod_Campo = m.Cod_Campo,
                                    Campo = mcam.Descripcion,
                                    Sector = (short)ms.Sector,
                                    Ha = mcam.Hectareas,
                                    Compras_oportunidad = mcam.Compras_Oportunidad,
                                    Tipo=t.Descripcion,
                                    Producto=p.Descripcion,
                                    Fecha_solicitud = (DateTime)m.Fecha_solicitud,
                                    Inicio_cosecha = (DateTime)m.Inicio_cosecha,
                                    Ubicacion = loc.Descripcion,
                                    Telefono = m.Telefono,
                                    Liberacion = m.Liberacion,
                                    Fecha_ejecucion = (DateTime)m.Fecha_ejecucion,
                                    IdAnalisis_Residuo = man.Id,
                                    Analisis = man.Estatus,
                                    IdAgenI = mcam.IdAgenI,
                                    Estatus = mc.Estatus,
                                    IdAgenC = (short)mcam.IdAgenC,
                                    AsesorC = ageC.Abrev,
                                    AsesorCS = ageCS.Abrev,
                                    Tarjeta = m.Tarjeta,
                                    IdRegion = ageP.IdRegion,
                                    Fecha_analisis = man.Fecha,
                                    //Folio= man.Folio,
                                    //Zona=z.DescZona,
                                    //Fecha_envio=man.Fecha_envio,
                                    //Fecha_entrega=man.Fecha_entrega,
                                    //LiberacionUSA=man.LiberacionUSA,
                                    //LiberacionEU=man.LiberacionEU,
                                    //Num_analisis=man.Num_analisis,
                                    //Laboratorio=man.Laboratorio,
                                    //Traza=man.Traza
                                }).Distinct();
                    var res = Tuple.Create(item.OrderByDescending(x => x.Fecha_solicitud).ToList(), analisis.OrderByDescending(x => x.Fecha_solicitud).ToList());
                    return Ok(res);
                }
                else
                {
                    if (idAgen == 153 || idAgen == 281 || idAgen == 167 || idAgen == 182)
                    {
                        item = item.Distinct();
                    }
                    else if (idAgen == 1)
                    {
                        item = item.Where(x => x.IdRegion == 1 || x.IdRegion == 3 || x.IdRegion == 4 || x.IdRegion == 5).Distinct();
                    }
                    else if (idAgen == 5)
                    {
                        item = item.Where(x => x.IdRegion == 2).Distinct();
                    }
                    else
                    {
                        item = item.Where(x => x.IdAgen == idAgen || x.IdAgenC == idAgen || x.IdAgenI == idAgen).Distinct();
                    }
                    return Ok(item.OrderByDescending(x => x.Fecha_solicitud).ToList());
                }
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

                var modeloExistente = _context.ProdMuestreo.FirstOrDefault(m => m.Cod_Prod == model.Cod_Prod && m.Cod_Campo == model.Cod_Campo && m.Temporada == catSemanas.Temporada);
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
        //fecha_ejecucion  
        [HttpPut("{id}/{idAgen}/{sector}")]
        public ActionResult Put(int id, short idAgen, short sector,[FromBody] ProdMuestreo model)
        {
            try
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
                            body = "Fecha de muestreo agregada: " + model.Fecha_ejecucion;
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }               

                _context.SaveChanges();
                notificaciones.SendNotificationJSON(title, body);
                return CreatedAtRoute("GetMuestreo", new { id = 0, idAgen = idAgen }, model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //Liberar solicitud 
        [HttpPatch("{id}/{idAgen}")]
        public ActionResult Patch(int id, short idAgen)
        {
            try
            {
                var muestreo = _context.ProdMuestreo.Find(id);

                if (muestreo.Id == id)
                {
                    muestreo.Liberacion = "S";
                    title = "Código: " + muestreo.Cod_Prod + " campo: " + muestreo.Cod_Campo;
                    body = "Solicitud de muestreo liberada por producción ";

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
