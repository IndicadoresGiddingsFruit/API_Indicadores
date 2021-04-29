using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Indicadores.Classes
{
    public class DistribucionCurva
    {
        //        string semana1 = "", semana2 = "";
        //        double x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18, x19, x20, x21, x22, x23, x24;
        //        List<double> semanas_c1 = new List<double>();
        //        List<double> semanas_c2 = new List<double>();
        //        double x = 0, y = 0;
        //        int z = 0, z1 = 0, a = 0, b = 0;

        //        //Antes de guardar una nueva curva revisar por cod_prod,cod_campoy sector que no exista aunque tenga ingeniero diferente no deben ser igual
        //        protected void Guardar_Click(object sender, EventArgs e)
        //        {
        //            sems_xcultivos();
        //            semanas();
        //       }

        //        private void sems_xcultivos()
        //        {
        //            double.TryParse(cjs1.Text, out x);//cajas del corte 1
        //            double.TryParse(cjs2.Text, out y);//cajas del corte 2

        //            if (dropCultivo.SelectedValue == "ZARZAMORA")
        //            {
        //                if (x > 0)
        //                {
        //                    x1 = x * 0.01182925;
        //                    x2 = x * 0.04587745;
        //                    x3 = x * 0.11407154;
        //                    x4 = x * 0.18949608;
        //                    x5 = x * 0.21157628;
        //                    x6 = x * 0.16946517;
        //                    x7 = x * 0.10575229;
        //                    x8 = x * 0.06978053;
        //                    x9 = x * 0.03563078;
        //                    semanas_c1 = new List<double> { x1, x2, x3, x4, x5, x6, x7, x8, x9 };
        //                }
        //                if (y > 0)
        //                {
        //                    x10 = y * 0.01182925;
        //                    x11 = y * 0.04587745;
        //                    x12 = y * 0.11407154;
        //                    x13 = y * 0.18949608;
        //                    x14 = y * 0.21157628;
        //                    x15 = y * 0.16946517;
        //                    x16 = y * 0.10575229;
        //                    x17 = y * 0.06978053;
        //                    semanas_c2 = new List<double> { x10, x11, x12, x13, x14, x15, x16, x17 };
        //                }
        //            }
        //            if (dropCultivo.SelectedValue == "FRAMBUESA")
        //            {
        //                if (x > 0)
        //                {
        //                    x1 = x * 0.072002021;
        //                    x2 = x * 0.092011271;
        //                    x3 = x * 0.128120111;
        //                    x4 = x * 0.138171205;
        //                    x5 = x * 0.130002213;
        //                    x6 = x * 0.131171834;
        //                    x7 = x * 0.113202513;
        //                    x8 = x * 0.101102521;
        //                    x9 = x * 0.094216311;
        //                    semanas_c1 = new List<double> { x1, x2, x3, x4, x5, x6, x7, x8, x9 };
        //                }

        //                if (y > 0)
        //                {
        //                    x10 = y * 0.07750311;
        //                    x11 = y * 0.101500301;
        //                    x12 = y * 0.11021502;
        //                    x13 = y * 0.128202035;
        //                    x14 = y * 0.13800101;
        //                    x15 = y * 0.135022;
        //                    x16 = y * 0.115132242;
        //                    x17 = y * 0.10131218;
        //                    x18 = y * 0.093112102;
        //                    semanas_c2 = new List<double> { x10, x11, x12, x13, x14, x15, x16, x17, x18 };
        //                }
        //            }

        //            if (dropCultivo.SelectedValue == "ARANDANO")
        //            {
        //                x1 = x * 0.005887505;
        //                x2 = x * 0.007216989;
        //                x3 = x * 0.008765486;
        //                x4 = x * 0.010527477;
        //                x5 = x * 0.012944629;
        //                x6 = x * 0.016748946;
        //                x7 = x * 0.022616706;
        //                x8 = x * 0.030795171;
        //                x9 = x * 0.040887722;
        //                x10 = x * 0.051900755;
        //                x11 = x * 0.062515345;
        //                x12 = x * 0.071438163;
        //                x13 = x * 0.077676018;
        //                x14 = x * 0.080659535;
        //                x15 = x * 0.08024201;
        //                x16 = x * 0.076645869;
        //                x17 = x * 0.070405294;
        //                x18 = x * 0.062303151;
        //                x19 = x * 0.053276801;
        //                x20 = x * 0.044286237;
        //                x21 = x * 0.036171987;
        //                x22 = x * 0.02954509;
        //                x23 = x * 0.024736616;
        //                x24 = x * 0.021806499;

        //                semanas_c1 = new List<double> { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16, x17, x18, x19, x20, x21, x22, x23, x24 };
        //            }
        //            if (dropCultivo.SelectedValue == "FRESA")
        //            {
        //                x1 = x * 0.021500031;
        //                x2 = x * 0.022121241;
        //                x3 = x * 0.031500031;
        //                x4 = x * 0.036221152;
        //                x4 = x * 0.04486109;
        //                x6 = x * 0.048331095;
        //                x7 = x * 0.062002291;
        //                x8 = x * 0.071801242;
        //                x9 = x * 0.090220142;
        //                x10 = x * 0.10752201;
        //                x11 = x * 0.111631211;
        //                x12 = x * 0.113263201;
        //                x13 = x * 0.11801314;
        //                x14 = x * 0.121012123;

        //                semanas_c1 = new List<double> { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14 };
        //            }

        //        }

        //        private bool semanas()
        //        {
        //            try
        //            {
        //                string query_sem1 = "select semana from catsemanas where CONVERT(VARCHAR(10), '" + Fechacorte1.Text + "', 23)  between CONVERT(VARCHAR(10), Inicio, 23) and CONVERT(VARCHAR(10), Fin, 23)";
        //                SqlCommand cm = new SqlCommand(query_sem1, con);
        //                semana1 = Convert.ToString(cm.ExecuteScalar());

        //                string query_sem2 = "select semana from catsemanas where CONVERT(VARCHAR(10), '" + Fechacorte2.Text + "', 23)  between CONVERT(VARCHAR(10), Inicio, 23) and CONVERT(VARCHAR(10), Fin, 23)";
        //                SqlCommand cm2 = new SqlCommand(query_sem2, con);
        //                semana2 = Convert.ToString(cm2.ExecuteScalar());

        //                int.TryParse(semana1, out a);

        //                con.Open();
        //                if (semanas_c1.Count > 0)
        //                {
        //                    for (int i = 0; i <= semanas_c1.Count - 1; i++)
        //                    {
        //                        while (z <= i)
        //                        {
        //                            string query = "Update " + bd + " SET [" + a + "] = " + semanas_c1[i] + " " +
        //                                "where Id=" + Id.Text + " ";
        //                            SqlCommand cmd = new SqlCommand(query, con);
        //                            cmd.ExecuteNonQuery();
        //                            a++;
        //                            if (a > 52)
        //                            {
        //                                a = 1;
        //                            }
        //                            z++;
        //                        }
        //                    }

        //                }

        //                int.TryParse(semana2, out b);

        //                if (semanas_c2.Count > 0)
        //                {
        //                    for (int i = 0; i <= semanas_c2.Count - 1; i++)
        //                    {
        //                        while (z1 <= i)
        //                        {
        //                            string query = "Update " + bd + " SET [" + b + "] = " + semanas_c2[i] + " " +
        //                                "where Id=" + Id.Text + " ";
        //                            SqlCommand cmd = new SqlCommand(query, con);
        //                            cmd.ExecuteNonQuery();
        //                            b++;
        //                            if (b > 52)
        //                            {
        //                                b = 1;
        //                            }
        //                            z1++;
        //                        }
        //                    }
        //                }

        //                con.Close();
        //                return true;
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.ToString();
        //                return false;
        //            }
        //        }
    }
}
