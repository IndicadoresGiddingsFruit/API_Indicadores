
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ApiIndicadores.Classes
{
    public class Email
    {
        public void sendmail(string correo_p = "", short? region = 0, List<SeguimientoClass> model=null)
        {
            string destinatario = "", subject = "", mensaje = "";

            subject = "IMPORTANTE:: VALIDACION DE CARTERA VIGENTE";
            mensaje = "Se necesita tu apoyo REALIZANDO LA VALIDACION  de la cartera, gracias. <br/>";
             

            MailMessage correo = new MailMessage();
            correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");

            foreach (var item in model)
            {
                if (item.Cod_Prod=="99999")
                {
                    correo.To.Add("marholy.martinez@giddingsfruit.mx");
                }

                else
                {
                    correo.To.Add(correo_p);

                    correo.CC.Add("oscar.castillo@giddingsfruit.mx");
                    correo.CC.Add("angel.lopez@giddingsfruit.mx");

                    if (correo_p != "ademir.reyes@giddingsfruit.mx")
                    {
                        destinatario = "Ingeniero";
                        if (correo_p == "aliberth.martinez@giddingsfruit.mx")
                        {
                            correo.CC.Add("jose.acosta@giddingsfruit.mx");
                        }

                        if (region == 2)
                        {
                            correo.CC.Add("jose.acosta@giddingsfruit.mx");
                            correo.CC.Add("jose.partida@giddingsfruit.mx");
                        }

                        else if (region == 3)
                        {
                            correo.CC.Add("genaro.morales@giddingsfruit.mx");
                            correo.CC.Add("fernando.fierro@giddingsfruit.mx");
                        }

                        else if (region == 4)
                        {
                            correo.CC.Add("josefina.cervantes@giddingsfruit.mx");
                        }

                        else if (region == 5)
                        {
                            correo.CC.Add("evelyn.caceres@giddingsfruit.mx");
                        }
                    }
                }
            }

            correo.Subject = subject;
            correo.Body += "Buen dia " + destinatario + " <br/>";
            correo.Body += " <br/>";
            correo.Body += "" + mensaje + " <br/>";
            correo.Body += " <br/>";
            if (model != null)
            {
                correo.Body += "<table border striped>" +
                "<thead>" +
                "<tr>" +
                "<th> Codigo </th>" +
                "<th> Productor </th>" +
                "<th> Estatus </th>" +
                "<th> Comentarios </th>" +
                "</tr>" +
                "</thead>";

                foreach (var item in model)
                {
                    correo.Body += "<tbody>" +
                    "<tr>" +
                    "<td> " + item.Cod_Prod + " </td>" +
                    "<td> " + item.Productor + " </td>" +
                    "<td> " + item.DescEstatus + " </td>" +
                    "<td> " + item.Comentarios + " </td>" +
                    "</tr>" +
                    "</tbody>";
                }
                correo.Body += "</table>";
            }
            correo.Body += " <br/>";
            correo.Body += "Saludos <br/>";
            correo.Body += " <br/>";
            correo.IsBodyHtml = true;
            correo.Priority = MailPriority.High;

            SmtpClient a = new SmtpClient();
            a.Host = "smtp.gmail.com";
            a.Port = 587;//25
            a.EnableSsl = true;
            a.UseDefaultCredentials = true;
            a.Credentials = new System.Net.NetworkCredential
               ("indicadores.giddingsfruit@gmail.com", "indicadores2019");
            a.Send(correo);

            //credenciales(correo_p);
        }

        public void credenciales(string correo_p)
        {
            try
            {
                //string img_path = HttpContext.Current.Server.MapPath("/Image/Seguimiento_financ.png");
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("indicadores.giddingsfruit@gmail.com", "Indicadores GiddingsFruit");
                correo.To.Add(correo_p); //"marholy.martinez@giddingsfruit.mx");
                correo.Subject = "Credenciales";
                correo.Body += "Buen dia <br/>";
                correo.Body += " <br/>";
                correo.Body += "Para acceder al enlace anterior utilice su usuario de SEASON y la clave 123456 <br/>";
                correo.Body += " <br/>";
                correo.Body += "Seleccione el boton editar, seleccione sun estatus, escriba un comentario, y guarde cada registro <br/>";
                correo.Body += " <br/>";
                correo.Body += "Saludos! <br/>";
                correo.Body += " <br/>";
                //correo.Attachments.Add(new Attachment(img_path));
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.High;

                SmtpClient a = new SmtpClient();
                a.Host = "smtp.gmail.com";
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
