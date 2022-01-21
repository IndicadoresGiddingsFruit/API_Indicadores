using ApiIndicadores.Classes;
using ApiIndicadores.Context;
using ApiIndicadores.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncuestasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EncuestasController(AppDbContext context)
        {
            this._context = context;
        }

        //Get encuestas creadas
        [HttpGet(Name = "GetAllEncuestas")]
        public ActionResult Get()
        {
            try
            {
                var listaUsuarios = (from e in _context.EncuestasUsuarios
                                     join r in _context.SIPGUsuarios on e.IdUsuario equals r.Id
                                     group e by new
                                     {
                                         IdEncuestasUsuarios = e.Id,
                                         IdEncuesta = e.IdEncuesta,
                                         IdUsuario = e.IdUsuario,
                                         Fecha = e.Fecha_respuesta,
                                         //IdRelacion=e.IdRelacion,

                                         IdSIPGUsuarios = r.Id,
                                         Clave = r.Clave,
                                         Nombre = r.Nombre,
                                         Usuario = r.Completo,
                                         correo = r.correo,
                                         idAgen = r.IdAgen,
                                         idRegion = r.IdRegion,
                                         Tipo = r.Tipo
                                     } into x
                                     select new
                                     {
                                         IdUsuario = x.Key.IdUsuario,
                                         Usuario = x.Key.Usuario,
                                         IdEncuesta = x.Key.IdEncuesta,
                                         Fecha_respuesta = x.Key.Fecha,
                                     }).Distinct();

                var encuestas = (from e in _context.EncuestasCat
                                 join t in _context.EncuestasTipo on e.IdTipo equals t.Id
                                 group e by new
                                 {
                                     IdEncuesta = e.Id,
                                     Nombre = e.Nombre,
                                     Descripcion = e.Descripcion,
                                     Fecha = e.Fecha,
                                     Fecha_modificacion = e.Fecha_modificacion,
                                     Estatus = e.Estatus,
                                     IdTipo = e.IdTipo,
                                     Tipo = t.Descripcion
                                 } into x
                                 select new
                                 {
                                     IdEncuesta = x.Key.IdEncuesta,
                                     Nombre = x.Key.Nombre,
                                     Descripcion = x.Key.Descripcion,
                                     Fecha = x.Key.Fecha,
                                     Fecha_modificacion = x.Key.Fecha_modificacion,
                                     Estatus = x.Key.Estatus,
                                     IdTipo = x.Key.IdTipo,
                                     Tipo = x.Key.Tipo,
                                     listaUsuarios = listaUsuarios.Where(r => r.IdEncuesta == x.Key.IdEncuesta).ToList()
                                 }).Distinct();

                return Ok(encuestas.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetEncuesta")]
        public ActionResult Get(int id)
        {
            try
            {
                var respuestas = _context.RespuestasTotal.FromSqlRaw("Select E.Id as IdEncuesta, E.Nombre, E.Descripcion, zo.nombre_zona as Zona, de.nombre_departamentos as Departamento, U.IdUsuario,S.Completo as Usuario, P.Id as IdPregunta, P.Pregunta, Rs.Id as IdRespuesta, Rs.Respuesta, L.IdRelacion " +
                    "from EncuestasCat E left join EncuestasUsuarios U on E.Id = U.IdEncuesta " +
                    "left join SipgUsuarios S on U.IdUsuario = S.Id " +
                    "left join EncuestasDet P on E.Id = P.IdEncuesta " +
                    "left join EncuestasRelacion R on P.Id = R.IdPregunta " +
                    "left join EncuestasRes Rs on R.IdRespuesta = Rs.Id " +
                    "left join EncuestasLog L on U.Id = L.IdAsingUsuario and R.Id = L.IdRelacion " +
                    "left join RH_EntityModel..Empleado em on s.id_empleado = em.id_empleado " +
                    "inner join RH_EntityModel..Puesto pu on pu.id_puesto = em.id_puesto " +
                    "inner join RH_EntityModel..Departamentos de on de.id_departamentos = pu.id_departamento " +
                    "Inner Join RH_EntityModel..Subacopio su on su.id_subacopio = em.id_subacopio " +
                    "Inner Join RH_EntityModel..Centro_Acopio ca on ca.id_centro_acopio = su.id_centro_acopio " +
                    "Inner Join RH_EntityModel..Zonas zo on zo.id_zona = ca.id_zona " +
                    "Where em.[status] = 1 and E.Id = "+id+" and L.IdRelacion is not null").ToList();

                if (respuestas == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(respuestas.ToList());
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}/{IdUsuario}", Name = "GetIdEncuesta")]
        public ActionResult Get(int id, int IdUsuario)
        {
            try
            {
                //Lista de respuestas por pregunta
                var listaRes = (from e in _context.EncuestasRelacion
                                join r in _context.EncuestasRes on e.IdRespuesta equals r.Id
                                group e by new
                                {
                                    idRelacion = e.Id,
                                    IdPregunta = e.IdPregunta,
                                    IdRespuesta = r.Id,
                                    Respuesta = r.Respuesta
                                } into x
                                select new
                                {
                                    idRelacion = x.Key.idRelacion,
                                    IdPregunta = x.Key.IdPregunta,
                                    IdRespuesta = x.Key.IdRespuesta,
                                    Respuesta = x.Key.Respuesta
                                }).Distinct();

                //lista encuestas, preguntas y sus respuestas

                var encuestas = (from e in _context.EncuestasCat
                                 join p in _context.EncuestasDet on e.Id equals p.IdEncuesta
                                 where e.Id == id
                                 group e by new
                                 {
                                     IdEncuesta = e.Id,
                                     Encuesta = e.Nombre,
                                     Descripcion=e.Descripcion,
                                     IdPregunta = p.Id,
                                     Pregunta = p.Pregunta
                                 } into x
                                 select new
                                 {
                                     IdEncuesta = x.Key.IdEncuesta,
                                     Encuesta = x.Key.Encuesta,
                                     Descripcion = x.Key.Descripcion,
                                     IdPregunta = x.Key.IdPregunta == null ? null : x.Key.IdPregunta,
                                     Pregunta = x.Key.Pregunta == null ? null : x.Key.Pregunta,
                                     ListaRes = listaRes == null ? null : listaRes.Where(r => r.IdPregunta == x.Key.IdPregunta).ToList()
                                 }).Distinct();

                var usuarios = (from u in _context.EncuestasUsuarios
                                join e in _context.EncuestasCat on u.IdEncuesta equals e.Id
                                join s in _context.SIPGUsuarios on u.IdUsuario equals s.Id
                                where u.IdUsuario == IdUsuario
                                group e by new
                                {
                                    IdEncuesta = e.Id,
                                    Nombre = e.Nombre,
                                    Descripcion = e.Descripcion,
                                    Fecha = e.Fecha,
                                    Estatus = e.Estatus,
                                    Fecha_Respuesta = u.Fecha_respuesta,
                                    Completo = s.Completo
                                } into x
                                select new
                                {
                                    IdEncuesta = x.Key.IdEncuesta,
                                    Nombre = x.Key.Nombre,
                                    Descripcion = x.Key.Descripcion,
                                    Fecha = x.Key.Fecha,
                                    Estatus = x.Key.Estatus,
                                    Fecha_Respuesta = x.Key.Fecha_Respuesta,
                                    Completo = x.Key.Completo
                                }).Distinct();

                //lista respuestas por usuario
                var respuestas = (from log in _context.EncuestasLog
                                  join u in _context.EncuestasUsuarios on log.IdAsingUsuario equals u.Id
                                  join su in _context.SIPGUsuarios on u.IdUsuario equals su.Id
                                  join e in _context.EncuestasCat on u.IdEncuesta equals e.Id
                                  join r in _context.EncuestasRelacion on log.IdRelacion equals r.Id
                                  join p in _context.EncuestasDet on r.IdPregunta equals p.Id
                                  join rs in _context.EncuestasRes on r.IdRespuesta equals rs.Id

                                  group e by new
                                  {
                                      IdEncuesta = e.Id,
                                      Encuesta = e.Nombre,
                                      IdUsuario = u.IdUsuario,
                                      Usuario = su.Completo,
                                      IdPregunta = r.IdPregunta,
                                      Pregunta = p.Pregunta,
                                      IdRespuesta = r.IdRespuesta,
                                      Respuesta = rs.Respuesta,
                                      IdRelacion = log.IdRelacion
                                  } into x
                                  select new
                                  {
                                      IdEncuesta = x.Key.IdEncuesta,
                                      Encuesta = x.Key.Encuesta,
                                      IdUsuario = x.Key.IdUsuario,
                                      Usuario = x.Key.Usuario,
                                      IdPregunta = x.Key.IdPregunta,
                                      Pregunta = x.Key.Pregunta,
                                      IdRespuesta = x.Key.IdRespuesta,
                                      Respuesta = x.Key.Respuesta,
                                      IdRelacion = x.Key.IdRelacion
                                  }).Where(e => e.IdEncuesta == id && e.IdUsuario == IdUsuario).
                                  OrderBy(x => x.IdPregunta).Distinct();

                var res = Tuple.Create(encuestas.ToList(), usuarios.ToList(), respuestas.ToList());

                //var data = _context.EncuestasClass.FromSqlRaw($"sp_GetEncuestas " + id + "," + IdUsuario + "").ToList();
                return Ok(res);                 
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<EncuestasController>/5
        //[HttpGet("{id}", Name = "GetIdEncuesta")]
        //public async Task<ActionResult<EncuestasCat>> Get(int id)
        //{
        //    try
        //    {
        //        var item = (from e in _context.EncuestasCat
        //                    join p in _context.EncuestasDet on e.Id equals p.IdEncuesta
        //                    where e.Id == id
        //                    group e by new
        //                    {
        //                        IdEncuesta = e.Id,
        //                        Encuesta = e.Nombre,
        //                        IdPregunta = p.Id,
        //                        Pregunta = p.Pregunta,
        //                        // CodUsuario = u.CodUsuario ?? null
        //                    } into x
        //                    select new
        //                    {
        //                        IdEncuesta = x.Key.IdEncuesta,
        //                        Encuesta = x.Key.Encuesta,
        //                        IdPregunta = x.Key.IdPregunta,
        //                        Pregunta = x.Key.Pregunta
        //                    }).Distinct();

        //        if (item == null)
        //        {
        //            return NotFound();
        //        }

        //        return Ok(await item.ToListAsync());
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // POST api/<EncuestasController>

        [HttpPost]
        public async Task<ActionResult<EncuestasCat>> Post([FromBody] EncuestasCat model)
        {
            try
            {
                var modeloExistente = _context.EncuestasCat.FirstOrDefault(m => m.Nombre == model.Nombre);
                if (modeloExistente == null)
                {
                    model.Fecha = DateTime.Now;
                    model.Estatus = "1";
                    _context.EncuestasCat.Add(model);
                    await _context.SaveChangesAsync();
                    return CreatedAtRoute("GetAllEncuestas", model);
                }
                else
                {
                    return BadRequest("La encuesta ya existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<EncuestasDetController>/5/pregunta
        //[HttpPost("{id}/{pregunta}")]
        //public async Task<ActionResult<EncuestasDet>> Post(int id, string pregunta)
        //{
        //    try
        //    {
        //        var modeloExistente = _context.EncuestasCat.FirstOrDefault(m => m.Id == id);
        //        if (modeloExistente != null)
        //        {
        //            var item = _context.EncuestasDet.FirstOrDefault(m => m.Pregunta == pregunta && m.IdEncuesta == id);
        //            if (item == null)
        //            {
        //                EncuestasDet model = new EncuestasDet();
        //                model.IdEncuesta = id;
        //                model.Pregunta = pregunta;
        //                _context.EncuestasDet.Add(model);
        //                await _context.SaveChangesAsync();
        //                return CreatedAtRoute("GetIdEncuesta", new { id = id, IdUsuario = 0}, model);
        //            }
        //            else
        //            {
        //                return BadRequest("La pregunta ya existe");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest("La encuesta no existe");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // PUT api/<EncuestasDetController>/5
        //[HttpPut("{id}/{pregunta}")]
        //public async Task<ActionResult<EncuestasDet>> Put(int id, string pregunta)
        //{
        //    try
        //    {
        //        var model = _context.EncuestasDet.FirstOrDefault(m => m.Id == id);
        //        if (model != null)
        //        {
        //            model.Pregunta = pregunta;
        //            await _context.SaveChangesAsync();
        //            return CreatedAtRoute("GetIdEncuesta", new { id = model.IdEncuesta, IdUsuario = 0 }, model);
        //        }
        //        else
        //        {
        //            return BadRequest("La pregunta no existe");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // DELETE api/<EncuestasController>/5

        [HttpDelete("{id}")]
        public async Task<ActionResult<EncuestasCat>> Delete(int id)
        {
            try
            {
                var model = _context.EncuestasCat.Find(id);
                if (model != null)
                {
                    _context.EncuestasCat.Remove(model);
                    await _context.SaveChangesAsync();
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

        // POST api/<EncuestasRes/EncuestasRelacion Controller>/5/pregunta
        //[HttpPost("{id}/{idpregunta}/{respuesta}")]
        //public async Task<ActionResult<EncuestasRes>> Post(int id, int idpregunta, string respuesta)
        //{
        //    try
        //    {
        //        EncuestasRes model = new EncuestasRes();
        //        EncuestasRelacion encuestasRelacion = new EncuestasRelacion();
        //        int idrespuesta = 0;

        //        var modeloExistente = _context.EncuestasRes.FirstOrDefault(m => m.Respuesta == respuesta);
        //        if (modeloExistente == null)
        //        {
        //            model.Respuesta = respuesta;
        //            _context.EncuestasRes.Add(model);
        //            await _context.SaveChangesAsync();
        //            idrespuesta = (int)model.Id;
        //        }
        //        else
        //        {
        //            idrespuesta = (int)modeloExistente.Id;
        //        }
        //        var modeloRelacion = _context.EncuestasRelacion.Where(m => m.IdPregunta == idpregunta && m.IdRespuesta == idrespuesta).FirstOrDefault();
        //        if (modeloRelacion == null)
        //        {
        //            encuestasRelacion.IdPregunta = idpregunta;
        //            encuestasRelacion.IdRespuesta = idrespuesta;
        //            _context.EncuestasRelacion.Add(encuestasRelacion);
        //            await _context.SaveChangesAsync();
        //        }

        //        return CreatedAtRoute("GetIdEncuesta", new { id = id, IdUsuario = 0 }, model);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}

        // PUT api/<EncuestasResController>/5

        [HttpPut("{id}/{idpregunta}/{idrespuesta}/{respuesta}")]
        public async Task<ActionResult<EncuestasRes>> Put(int id, int idpregunta, int idrespuesta, string respuesta)
        {
            try
            {
                var model = _context.EncuestasRes.FirstOrDefault(m => m.Id == idrespuesta);
                if (model != null)
                {
                    var relacion = _context.EncuestasRelacion.Where(r => r.IdRespuesta == idrespuesta).ToList();
                    if (relacion.Count > 1)
                    {
                        EncuestasRes nueva_res = new EncuestasRes();
                        nueva_res.Respuesta = respuesta;
                        _context.EncuestasRes.Add(nueva_res);
                        await _context.SaveChangesAsync();

                        var modeloRelacion = _context.EncuestasRelacion.Where(m => m.IdPregunta == idpregunta && m.IdRespuesta == idrespuesta).FirstOrDefault();
                        if (modeloRelacion != null)
                        {
                            modeloRelacion.IdPregunta = idpregunta;
                            modeloRelacion.IdRespuesta = nueva_res.Id;
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        model.Respuesta = respuesta;
                        await _context.SaveChangesAsync();
                    }

                    return CreatedAtRoute("GetIdEncuesta", new { id = id, IdUsuario = 0 }, model);
                }
                else
                {
                    return BadRequest("La respuesta no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<EncuestasUsuariosController>/5/pregunta
        [HttpPost("{id}")]
        public async Task<ActionResult<EncuestasUsuarios>> Post(int id, [FromBody] EncuestasUsuarios model)
        {
            try
            {
                var modeloExistente = _context.EncuestasCat.FirstOrDefault(m => m.Id == id);
                if (modeloExistente != null)
                {
                    modeloExistente.Fecha_modificacion = DateTime.Now;
                    await _context.SaveChangesAsync();

                    var item = _context.EncuestasUsuarios.FirstOrDefault(m => m.IdUsuario == model.IdUsuario && m.IdEncuesta == id);
                    if (item == null)
                    {
                        model.IdEncuesta = id;
                        _context.EncuestasUsuarios.Add(model);
                        await _context.SaveChangesAsync();
                        return CreatedAtRoute("GetIdEncuesta", new { id = id, IdUsuario = 0 }, model);
                    }
                    else
                    {
                        return BadRequest("El usuario ya fué agregado anteriormente");
                    }
                }
                else
                {
                    return BadRequest("La encuesta no existe");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    
    }
}
