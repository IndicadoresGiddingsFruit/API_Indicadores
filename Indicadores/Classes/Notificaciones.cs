using System;
using System.IO;
using System.Net;
using System.Text;

namespace ApiIndicadores.Classes
{
    public class Notificaciones
    {
        public string SendNotificationJSON(string title, string body)
        {
            try
            {
                string SERVER_API_KEY = "AAAAY2aWkGc:APA91bHgEAnne-u36oGBahpXis_gz9Tkfq3_32vnG77U6eIJ8dO8vQiCJN4ABB7qBj6BN54pW1AEiWbFombmHfVepzf0tBUkgCF6BLhwE-Lu7_TDxm1Q77KnSPDv0n9TPaaBuhs8EjRU";
                string SENDER_ID = "426922905703";
                string deviceId = "dnRlR-AmQHmX-RGlnLyHgR:APA91bFaAaMMMHKj8urHW1m77lhGTOqJ3ycdehOdfOklzDW61Oo7d-mT1pejTx6-RDFZB6gBNPEqyiGUbX3cumcuisNCcJutTtzgAm_Jlm5LjWhn7xPacAyexe_bXrIh15Rw6dyjYB1m";

                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                var a = new
                {
                    notification = new
                    {
                        title,
                        body
                        //icon = "/Image/GIDDINGS_PRIMARY_STACKED_LOGO_DRIFT_RGB.png"//,
                        //click_action,
                        //sound = "mySound"
                    },
                    to = deviceId
                };

                byte[] byteArray = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(a));
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);
                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();

                return sResponseFromServer;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}
