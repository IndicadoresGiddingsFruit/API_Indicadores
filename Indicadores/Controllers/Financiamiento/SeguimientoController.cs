
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using ApiIndicadores.Models;
using ApiIndicadores.Classes;
using ApiIndicadores.Context;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiIndicadores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeguimientoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SeguimientoController(AppDbContext context)
        {
            this._context = context;
        }

        Notificaciones notificaciones = new Notificaciones();
        Email email = new Email();
        string title = "", body = "";

        // GET: api/<SeguimientoController>
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                return Ok(_context.Seguimiento_financ.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // GET api/<SeguimientoController>/5 
        [HttpGet("{id}/{idAgen}", Name = "GetSeguimiento")]
        public Tuple<List<SeguimientoClass>, List<SeguimientoClass>> Get(int id, short idAgen)
        {
            var item = (dynamic)null;
            var item2 = (dynamic)null;

            if (id == 391)
            {

                item = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.IdAgen, A.Nombre as Asesor, S.Estatus, S.Comentarios, S.Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1,0), 0) as caja1, round(isnull(R2.cjs2,0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal,0),0)), 1)as SaldoFinal, SE.Semana " +
                  $"From Seguimiento_financ S " +
                  $"left join(select Cod_Prod, (sum(Saldo) + sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                  $"left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                  $"left join ProdAgenteCat A on S.IdAgen = A.IdAgen " +
                  $"left join(Select sum(Convertidas) as cjs1, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod)R on S.Cod_Prod = R.Cod_Prod " +
                  $"left join(Select sum(Convertidas) as cjs2, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod)R2 on S.Cod_Prod = R2.Cod_Prod " +
                  $"left join(Select distinct S.Cod_Prod, R.Semana from(Select distinct Cod_Prod, max(Fecha) as Fecha from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S " +
                  $"left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                  $"where S.Enviado is null " +
                  $"order by S.Fecha").ToList();

                //item2 = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.IdAgen, A.Nombre as Asesor, S.Estatus, S.Comentarios, S.Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1,0), 0) as caja1, round(isnull(R2.cjs2,0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal,0),0)), 1)as SaldoFinal, SE.Semana " +
                //    $"From Seguimiento_financ S " +
                //    $"left join(select Cod_Prod, (sum(Saldo) + sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                //    $"left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                //    $"left join ProdAgenteCat A on S.IdAgen = A.IdAgen " +
                //    $"left join(Select sum(Convertidas) as cjs1, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod)R on S.Cod_Prod = R.Cod_Prod " +
                //    $"left join(Select sum(Convertidas) as cjs2, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod)R2 on S.Cod_Prod = R2.Cod_Prod " +
                //    $"left join(Select distinct S.Cod_Prod, R.Semana from(Select distinct Cod_Prod, max(Fecha) as Fecha from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S " +
                //    $"left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                //    $"where S.Enviado='S' " +
                //    $"order by S.Fecha").ToList();

                item2 = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.IdAgen, A.Nombre as Asesor, " +
                    $"(case when S.Estatus = 'A' then 'ATENCIÓN A PRODUCTORES' else case when S.Estatus = 'M' then 'CIERRE DE MATERIAL' else case when S.Estatus = 'C' then 'COBRANZA' " +
                    $"else case when S.Estatus = 'R' then 'PENDIENTE REVISIÓN' else case when S.Estatus = 'G' then 'REVISA GERENCIA' else case when S.Estatus = 'S' then 'SALDADO' else case when S.Estatus = 'T' then ' TERMINO TEMPORADA' " +
                    $"else case when S.Estatus = 'E' then 'VA A ENTREGAR' else case when S.Estatus = 'P' then 'VA A PAGAR' else '' end end end end end end end end end) as Estatus, " +
                    $"S.Comentarios, S.Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1, 0), 0) as caja1, round(isnull(R2.cjs2, 0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal, 0), 0)), 1) as SaldoFinal, SE.Semana " +
                    $"From Seguimiento_financ S " +
                    $"left join(select Cod_Prod, (sum(Saldo) +sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                    $"left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                    $"left join ProdAgenteCat A on S.IdAgen = A.IdAgen " +
                    $"left join(Select sum(Convertidas) as cjs1, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod)R on S.Cod_Prod = R.Cod_Prod " +
                    $"left join(Select sum(Convertidas) as cjs2, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod)R2 on S.Cod_Prod = R2.Cod_Prod " +
                    $"left join(Select distinct S.Cod_Prod, R.Semana from(Select distinct Cod_Prod, max(Fecha) as Fecha from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S " +
                    $"left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                    $"where S.Enviado='S' order by S.Fecha").ToList();
            }
            else
            {
                item2 = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.IdAgen, A.Nombre as Asesor, " +
                  $"(case when S.Estatus = 'A' then 'ATENCIÓN A PRODUCTORES' else case when S.Estatus = 'M' then 'CIERRE DE MATERIAL' else case when S.Estatus = 'C' then 'COBRANZA' " +
                  $"else case when S.Estatus = 'R' then 'PENDIENTE REVISIÓN' else case when S.Estatus = 'G' then 'REVISA GERENCIA' else case when S.Estatus = 'S' then 'SALDADO' else case when S.Estatus = 'T' then ' TERMINO TEMPORADA' " +
                  $"else case when S.Estatus = 'E' then 'VA A ENTREGAR' else case when S.Estatus = 'P' then 'VA A PAGAR' else '' end end end end end end end end end) as Estatus, " +
                  $"S.Comentarios, S.Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1, 0), 0) as caja1, round(isnull(R2.cjs2, 0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal, 0), 0)), 1) as SaldoFinal, SE.Semana " +
                  $"From Seguimiento_financ S " +
                  $"left join(select Cod_Prod, (sum(Saldo) +sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                  $"left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                  $"left join ProdAgenteCat A on S.IdAgen = A.IdAgen " +
                  $"left join(Select sum(Convertidas) as cjs1, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod)R on S.Cod_Prod = R.Cod_Prod " +
                  $"left join(Select sum(Convertidas) as cjs2, Cod_prod from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod)R2 on S.Cod_Prod = R2.Cod_Prod " +
                  $"left join(Select distinct S.Cod_Prod, R.Semana from(Select distinct Cod_Prod, max(Fecha) as Fecha from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S " +
                  $"left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                  $"where S.Enviado='S' and S.IdAgen=" + idAgen + " " +
                  $"order by S.Fecha").ToList();
            }
            var tuple = Tuple.Create<List<SeguimientoClass>, List<SeguimientoClass>>(item, item2);
            return tuple;
        }

        // POST api/<SeguimientoController>
        [HttpPost]
        public ActionResult Post([FromBody] Seguimiento_financ model)
        {
            try
            {
                model.Cod_Empresa = 2;
                model.Fecha = DateTime.Now;
                _context.Seguimiento_financ.Add(model);
                _context.SaveChanges();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        // PUT api/<SeguimientoController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Seguimiento_financ model)
        {
            try
            {
                    var item = _context.Seguimiento_financ.Where(x => x.Id == id).First();
                    if (model.Estatus.Length == 1)
                    {
                        item.Estatus = model.Estatus;
                    }
                    item.Comentarios = model.Comentarios;
                    item.Fecha_Up = DateTime.Now;
                    _context.SaveChanges();

                    title = "Código: " + item.Cod_Prod;
                    body = "Estatus modificado";

                    notificaciones.SendNotificationJSON(title, body);
                    return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        public ActionResult Patch([FromBody] List<Seguimiento_financ> model)
        {
            try
            {               
                foreach (var x in model)
                {
                    var item = _context.Seguimiento_financ.Where(i => i.Id == x.Id).First();
                    item.Enviado = "S";
                    _context.SaveChanges();                   
                }

                var result = (from y in model
                              group y by new { y.IdAgen } into g
                              select new
                              {
                                  IdAgen = g.Key.IdAgen
                              }).ToList();

                var q = result.AsQueryable();

                foreach (var x in q)
                {
                    var agente = _context.SIPGUsuarios.FirstOrDefault(a => a.IdAgen == x.IdAgen);

                    email.sendmail(agente.correo, agente.IdRegion);

                    title = "Asignar estatus de financiamientos";
                    body = "Usted tiene nuevos códigos a revisar";

                    notificaciones.SendNotificationJSON(title, body);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        // DELETE api/<SeguimientoController>/5  
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Seguimiento_financ>> Delete(int id)
        {
            try
            {
                var model = _context.Seguimiento_financ.Find(id);

                if (model == null)
                {
                    return NotFound();
                }

                _context.Seguimiento_financ.Remove(model);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
