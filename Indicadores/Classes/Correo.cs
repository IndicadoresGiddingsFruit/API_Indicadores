using ApiIndicadores.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ApiIndicadores.Classes
{
    public class Correo
    {
        private readonly AppDbContext _context;
        public Correo(AppDbContext context)
        {
           _context = context;
        }
        public Correo()
        {
        }

        string correo_p, correo_c, correo_i;

        public void enviar(short? idAgen_Session, string cod_Prod, short? cod_Campo, string tipo_correo)
        {
            
            var campo = _context.ProdCamposCat.FirstOrDefault(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo);
            var email_p = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen && m.Tipo == "P");
            var email_c = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenC && m.Tipo == "C");
            var email_i = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgenI && m.Tipo == "I");

            correo_p = email_p.correo;

            var sesion = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == idAgen_Session);


            var muestreo = _context.ProdMuestreo.Where(m => m.Cod_Prod == cod_Prod && m.Cod_Campo == cod_Campo && m.IdAgen == idAgen_Session).FirstOrDefault();
            var analisis = _context.ProdAnalisis_Residuo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);
            var calidad_muestreo = _context.ProdCalidadMuestreo.FirstOrDefault(x => x.Id_Muestreo == muestreo.Id);

            if (email_c == null)
            {
                var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).First();
                if (sesion.IdRegion == 3)
                {
                    item.IdAgenC = 168;
                    correo_c = "juan.mares@giddingsfruit.mx";
                }
                if (sesion.IdRegion == 1)
                {
                    item.IdAgenC = 167;
                    correo_c = "mayra.ramirez@giddingsfruit.mx";
                }
                _context.SaveChanges();
            }
            else
            {
                correo_c = email_c.correo;
            }

            if (email_i == null)
            {
                if (sesion.IdRegion == 3)
                {
                    correo_c = "hector.torres@giddingsfruit.mx";
                }
                if (sesion.IdRegion == 1)
                {
                    var item = _context.ProdCamposCat.Where(x => x.Cod_Prod == cod_Prod && x.Cod_Campo == cod_Campo).First();
                    item.IdAgenI = 205;
                    _context.SaveChanges();
                    correo_i = "jesus.palafox@giddingsfruit.mx";
                    //}
                }
                else
                {
                    correo_i = email_i.correo;
                }
            }

            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                var prod = _context.ProdProductoresCat.FirstOrDefault(x => x.Cod_Prod == campo.Cod_Prod);

                if (tipo_correo == "nuevo")
                {

                    var Inicio_cosecha = String.Format("{0:d}", muestreo.Inicio_cosecha);

                    //if (sesion.Tipo == "P")
                    //{
                    //    correo.To.Add(sesion.correo);
                    //    correo.CC.Add(correo_c);
                    //    if (sesion.IdAgen == 158 || sesion.IdAgen == 173)
                    //    {
                    //        if (correo_i != "maria.lopez@giddingsfruit.mx")
                    //        {
                    //            correo.CC.Add("maria.lopez@giddingsfruit.mx");
                    //        }
                    //        if (correo_i != "maria.jimenez@giddingsfruit.mx")
                    //        {
                    //            correo.CC.Add("maria.jimenez@giddingsfruit.mx");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        correo.CC.Add(correo_i);
                    //    }
                    //}
                    //else if (sesion.Tipo == "I")
                    //{
                    //    correo.To.Add(sesion.correo);
                    //    correo.CC.Add(correo_c);
                    //    correo.CC.Add(correo_p);
                    //}
                    //else if (sesion.Tipo == "C")
                    //{
                    //    correo.To.Add(sesion.correo);
                    //    correo.CC.Add(correo_i);
                    //    correo.CC.Add(correo_p);

                    //    if (sesion.IdRegion == 1)
                    //    {
                    //        correo.CC.Add("marco.velazquez@giddingsfruit.mx");
                    //    }

                    //    if (sesion.IdRegion == 3)
                    //    {
                    //        if (correo_c != "juan.mares@giddingsfruit.mx")
                    //        {
                    //            correo.CC.Add("juan.mares@giddingsfruit.mx");
                    //        }
                    //        correo.CC.Add("genaro.morales@giddingsfruit.mx");
                    //    }
                    //}

                    //correo.CC.Add("oscar.castillo@giddingsfruit.mx");
                    correo.To.Add("marholy.martinez@giddingsfruit.mx");
                    correo.Subject = "Nuevo Muestreo: " + campo.Cod_Prod;
                    correo.Body = "Solicitado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + campo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + campo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Inicio de cosecha: " + Inicio_cosecha + "<br/>";

                }

                else if (tipo_correo == "Muestreo Liberado")
                {
                    correo.To.Add(sesion.correo);//correo_p
                    correo.CC.Add(correo_c);
                    correo.CC.Add(correo_i);
                    correo.Subject = "Muestreo Liberado: " + cod_Prod;
                    correo.Body = "Liberado por: " + sesion.Completo + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Ubicacion: " + campo.Ubicacion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Telefono: " + muestreo.Telefono + "<br/>";
                    correo.Body += " <br/>";
                }

                else if (tipo_correo == "Liberar tarjeta ")
                {
                    correo.CC.Add(correo_p);
                    correo.CC.Add(correo_c);
                    correo.CC.Add(correo_i);
                    correo.Subject = "Entrega de tarjeta: " + cod_Prod;
                    correo.Body = "Autorizado por: " + sesion.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Se ha autorizado la liberación para entrega de tarjeta del productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "campo: " + cod_Campo + " - " + campo.Descripcion + " ubicado en " + campo.Ubicacion + " <br/>";
                    correo.Body += " <br/>";
                    if (calidad_muestreo.Estatus == "1")
                    {
                        correo.Body += "Evaluacion de calidad: APTA <br/>";
                    }
                    else if (calidad_muestreo.Estatus == "2")
                    {
                        correo.Body += "Evaluacion de calidad: APTA CON CONDICIONES<br/>";
                    }
                    else if (calidad_muestreo.Estatus == "3")
                    {
                        correo.Body += "Evaluacion de calidad: PENDIENTE <br/>";
                    }
                    correo.Body += " <br/>";
                    if (calidad_muestreo != null)
                    {
                        correo.Body += "Incidencias encontradas: " + calidad_muestreo.Incidencia + " <br/>";
                        correo.Body += " <br/>";
                        if (calidad_muestreo.Propuesta != "")
                        {
                            correo.Body += "Propuesta: " + calidad_muestreo.Propuesta + " <br/>";
                            correo.Body += " <br/>";
                        }
                    }

                    if (muestreo.Liberar_Tarjeta != "")
                    {
                        correo.Body += "Justificación: " + muestreo.Liberar_Tarjeta + "<br/>";
                        correo.Body += " <br/>";
                    }
                    if (analisis != null)
                    {
                        if (analisis.Estatus == "R")
                        {
                            correo.Body += "Resultado del analisis: CON RESIDUOS <br/>";
                        }
                        else if (analisis.Estatus == "P")
                        {
                            correo.Body += "Resultado del analisis: EN PROCESO <br/>";
                        }
                        else if (analisis.Estatus == "F")
                        {
                            correo.Body += "Resultado del analisis: FUERA DE LIMITE <br/>";
                        }
                        else if (analisis.Estatus == "L")
                        {
                            correo.Body += "Resultado del analisis: LIBERADO <br/>";
                        }
                        correo.Body += " <br/>";
                    }
                }

                else if (tipo_correo == "Calidad fruta")
                {

                    correo.To.Add(sesion.correo);//correo_c                        
                    correo.CC.Add(correo_p);
                    correo.CC.Add(correo_i);

                    correo.Subject = "Calidad de fruta evaluada: " + cod_Prod;
                    correo.Body += "Evaluado por: " + sesion.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Productor: " + cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    if (calidad_muestreo.Estatus == "1")
                    {
                        correo.Body += "Estatus: APTA <br/>";
                        correo.Body += " <br/>";
                        if (analisis != null)
                        {
                            if (analisis.Estatus == "L")
                            {
                                correo.Body += "Entregar tarjeta <br/>";
                                correo.Body += " <br/>";
                            }
                        }
                    }
                    else if (calidad_muestreo.Estatus == "2")
                    {
                        correo.Body += "Estatus: APTA CON CONDICIONES<br/>";
                        correo.Body += " <br/>";
                        if (analisis.Estatus == "L")
                        {
                            correo.Body += "Entregar tarjeta <br/>";
                            correo.Body += " <br/>";
                        }
                    }
                    else if (calidad_muestreo.Estatus == "3")
                    {
                        correo.Body += "Estatus: PENDIENTE <br/>";
                        correo.Body += " <br/>";
                        correo.Body += "No entregar tarjeta <br/>";
                        correo.Body += " <br/>";
                    }
                    if (calidad_muestreo.Incidencia != "")
                    {
                        correo.Body += "Incidencia: " + calidad_muestreo.Incidencia + " <br/>";
                        correo.Body += " <br/>";
                        if (calidad_muestreo.Propuesta != "")
                        {
                            correo.Body += "Propuesta: " + calidad_muestreo.Propuesta + " <br/>";
                            correo.Body += " <br/>";
                        }
                    }
                }

                else if (tipo_correo == "Re-asignar codigo")
                {
                    //agente reasignado
                    var agente = _context.SIPGUsuarios.FirstOrDefault(m => m.IdAgen == campo.IdAgen);
                    correo.To.Add(sesion.correo);
                    correo.CC.Add(agente.correo);
                    correo.CC.Add("oscar.castillo@giddingsfruit.mx");
                    correo.Subject = "Reasignación de código: " + cod_Prod;
                    correo.Body += "El productor: " + cod_Prod + " - " + prod.Nombre + " con campo: " + cod_Campo + " - " + campo.Descripcion +
                        " ha sido reasignado a " + agente.Completo + " por " + sesion.Completo + " <br/>";
                }

                else if (tipo_correo == "Resultado análisis")
                {
                    string res_analisis="";
                    if (analisis.Estatus == "R")
                    {
                        res_analisis = "CON RESIDUOS";
                    }
                    else if (analisis.Estatus == "P")
                    {
                        res_analisis = "EN PROCESO";
                    }
                    else if (analisis.Estatus == "F")
                    {
                        res_analisis = "FUERA DEL LIMITE";
                    }
                    else if (analisis.Estatus == "L")
                    {
                        res_analisis = "LIBERADO";
                    }

                    correo.To.Add(sesion.correo);//"marholy.martinez@giddingsfruit.mx"
                    correo.CC.Add(correo_p);
                    correo.CC.Add(correo_c);
                    if (correo_i != "jesus.palafox@giddingsfruit.mx")
                    {
                        correo.CC.Add(correo_i);
                    }
                    correo.Subject = "Analisis de residuos: " + campo.Cod_Prod + " - " + res_analisis;
                    correo.Body += "Productor: " + campo.Cod_Prod + " - " + prod.Nombre + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Campo: " + campo.Cod_Campo + " - " + campo.Descripcion + " <br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de envio: " + analisis.Fecha_envio + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Fecha de entrega: " + analisis.Fecha_entrega + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Estatus: " + analisis + "<br/>";
                    correo.Body += " <br/>";
                    if (muestreo.Tarjeta != null)
                    {
                        if (analisis.Estatus == "L" && muestreo.Tarjeta == "S")
                        {
                            correo.Body += "Entregar tarjeta <br/>";
                            correo.Body += " <br/>";
                        }
                    }
                    if (analisis.Estatus == "F")
                    {
                        if (analisis.LiberacionUSA != null)
                        {
                            correo.Body += "Fecha de liberacion para USA: " + analisis.LiberacionUSA + "<br/>";
                            correo.Body += " <br/>";
                        }
                        if (analisis.LiberacionEU != null)
                        {
                            correo.Body += "Fecha de liberacion para EUROPA: " + analisis.LiberacionEU + "<br/>";
                            correo.Body += " <br/>";
                        }
                    }
                    correo.Body += "Numero de analisis: " + analisis.Num_analisis + "<br/>";
                    correo.Body += " <br/>";
                    correo.Body += "Laboratorio: " + analisis.Laboratorio + "<br/>";
                    correo.Body += " <br/>";
                    if (analisis.Comentarios != null)
                    {
                        correo.Body += "Comentarios: " + analisis.Comentarios + "<br/>";
                        correo.Body += " <br/>";
                    }
                }

                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                string sSmtpServer = "";
                sSmtpServer = "smtp.gmail.com";

                SmtpClient a = new SmtpClient();
                a.Host = sSmtpServer;
                a.Port = 587;//25
                a.EnableSsl = true;
                a.UseDefaultCredentials = true;
                a.Credentials = new System.Net.NetworkCredential
                   ("indicadores.giddingsfruit@gmail.com", "indicadores2019");
                a.Send(correo);
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}