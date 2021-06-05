using Indicadores.Classes;
using Indicadores.Context;
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
using Indicadores.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Indicadores.Controllers
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
        public ActionResult Get(int id,short idAgen)
        {
            try
            {
               var item =(dynamic) null;
                if (id == 391)
                {
                    //"(case when S.Estatus='A' THEN 'ATENCIÓN A PRODUCTORES' else case when S.Estatus='M' THEN 'CIERRE DE MATERIAL' else case when S.Estatus='C' THEN 'COBRANZA' else case when S.Estatus = 'R' THEN 'PENDIENTE REVISIÓN' else case when S.Estatus = 'G' THEN 'REVISA GERENCIA' else case when S.Estatus = 'S' THEN 'SALDADO' else case when S.Estatus = 'T' THEN 'TERMINO TEMPORADA' else case when S.Estatus = 'E' THEN 'VA A ENTREGAR' else case when S.Estatus = 'P' THEN 'VA A PAGAR' else '' end end end end end end end end end)  as Estatus," +

                    item = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.Cod_Campo, C.Descripcion as Campo, A.IdAgen, A.Nombre as Asesor, S.Estatus, " +
                        "(case when S.Comentarios is null then '' else S.Comentarios end) as Comentarios, convert(varchar, S.Fecha, 23) as Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1, 0), 0) as caja1, round(isnull(R2.cjs2, 0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal, 0), 0)), 1) as SaldoFinal, isnull(SE.Semana, 0) as Semana, S.AP, S.Fecha_Up, S.IdAgen as IdAgenS, A.IdRegion  " +
                        "From Seguimiento_financ S " +
                        "left join(select Cod_Prod, (sum(Saldo) + sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                        "left join ProdCamposCat C on S.Cod_Prod = C.Cod_Prod and S.Cod_Campo = C.Cod_Campo " +
                        "left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                        "left join ProdAgenteCat A on C.IdAgen = A.IdAgen " +
                        "left join(Select sum(Convertidas) as cjs1, Cod_prod, Cod_Campo from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod, Cod_Campo)R on S.Cod_Prod = R.Cod_Prod and S.Cod_Campo = R.Cod_Campo " +
                        "left join(Select sum(Convertidas) as cjs2, Cod_prod, Cod_Campo from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod, Cod_Campo)R2 on S.Cod_Prod = R2.Cod_Prod and S.Cod_Campo = R2.Cod_Campo " +
                        "left join(Select distinct S.Cod_Prod, R.Semana " +
                        "from(Select distinct Cod_Prod, max(Fecha) as Fecha " +
                        "from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                        "order by S.Fecha").ToList();
                }
                else
                {
                    item = _context.SeguimientoClass.FromSqlRaw($"Select S.Id, S.Cod_Prod, P.Nombre as Productor, S.Cod_Campo, C.Descripcion as Campo, A.IdAgen, A.Nombre as Asesor, S.Estatus, " +
                      "(case when S.Comentarios is null then '' else S.Comentarios end) as Comentarios, convert(varchar, S.Fecha, 23) as Fecha, (case when S.Estatus is null then DATEDIFF(day, S.Fecha, getdate()) else '' end) as dias,round(isnull(R.cjs1, 0), 0) as caja1, round(isnull(R2.cjs2, 0), 0) as caja2, CONVERT(varchar, CONVERT(money, round(isnull(F.SaldoFinal, 0), 0)), 1) as SaldoFinal, isnull(SE.Semana, 0) as Semana, S.AP, S.Fecha_Up, S.IdAgen as IdAgenS, A.IdRegion  " +
                      "From Seguimiento_financ S " +
                      "left join(select Cod_Prod, (sum(Saldo) + sum(SaldoAGQ)) as SaldoFinal from dbo.fnRptSaldosFinanciamiento(GetDate(), GetDate(), GetDate(), 50) group by Cod_Prod)F on S.Cod_Prod = F.Cod_Prod " +
                      "left join ProdCamposCat C on S.Cod_Prod = C.Cod_Prod and S.Cod_Campo = C.Cod_Campo " +
                      "left join ProdProductoresCat P on S.Cod_Prod = P.Cod_Prod " +
                      "left join ProdAgenteCat A on C.IdAgen = A.IdAgen " +
                      "left join(Select sum(Convertidas) as cjs1, Cod_prod, Cod_Campo from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2020-07-01' and '2020-12-31' group by Cod_prod, Cod_Campo)R on S.Cod_Prod = R.Cod_Prod and S.Cod_Campo = R.Cod_Campo " +
                      "left join(Select sum(Convertidas) as cjs2, Cod_prod, Cod_Campo from UV_ProdRecepcion where CodEstatus = 'V' and Fecha between '2021-01-01' and getdate() group by Cod_prod, Cod_Campo)R2 on S.Cod_Prod = R2.Cod_Prod and S.Cod_Campo = R2.Cod_Campo " +
                      "left join(Select distinct S.Cod_Prod, R.Semana " +
                      "from(Select distinct Cod_Prod, max(Fecha) as Fecha " +
                      "from UV_ProdRecepcion where CodEstatus = 'V' and Temporada = (select Temporada from CatSemanas where getdate() between Inicio and Fin) group by Cod_Prod)S left join UV_ProdRecepcion R on S.Fecha = R.Fecha)SE on S.Cod_Prod = SE.Cod_Prod " +
                      "WHERE C.IdAgen = " + idAgen + " order by S.Fecha").ToList();
                }
                return Ok(item);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<SeguimientoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SeguimientoController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Seguimiento_financ model)
        {
            try
            {
                var item = _context.Seguimiento_financ.Where(x => x.Id == id).First();
                item.Estatus = model.Estatus;
                item.Comentarios = model.Comentarios;
                item.Fecha_Up = DateTime.Now;
                _context.SaveChanges();

                title = "Código: " + model.Cod_Prod + " campo: " + model.Cod_Campo;
                body = "Estatus modificado";
                
                notificaciones.SendNotificationJSON(title, body);
                return CreatedAtRoute("GetSeguimiento", new { id= 0,idAgen = model.IdAgen },model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<SeguimientoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
