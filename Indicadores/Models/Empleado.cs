using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiIndicadores.Models
{
    [Table("Empleado")]
    public class Empleado
    {
        [Key]
        public int id_empleado { get; set; }
        public string foto { get; set; }
        public string nombre { get; set; }
        public string apellido_paterno { get; set; }
        public string apellido_materno { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public DateTime fecha_registro { get; set; }
        public string NSS { get; set; }
        public string RFC { get; set; }
        public string CURP { get; set; }
        public string calle { get; set; }
        public string colonia { get; set; }
        public string municipio { get; set; }
        public string estado_CP { get; set; }
        public string ciudad_nacimiento { get; set; }
        public string estado_nacimiento { get; set; }
        public string email { get; set; }
        public string email_cooporativo { get; set; }
        public string lic_manejo { get; set; }
        public string num_tarjeta { get; set; }
        public string num_nomina_paq { get; set; }
        public string NoCreditoInfonavit { get; set; }
        public DateTime fecha_salida { get; set; }
        public DateTime fecha_alta { get; set; }
        public string usuario_alta { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public string usuario_modificacion { get; set; }
        public string numero_celular { get; set; }
        public string numero_telefono { get; set; }
        public int id_usuario_autoriza { get; set; }
        public int id_sueldo { get; set; }
        public int id_banco { get; set; }
        public int id_estado_civil { get; set; }
        public int id_escolaridad_titulo { get; set; }
        public int id_puesto { get; set; }
        public int id_pservicio { get; set; }
        public int id_tipo_sanguineo { get; set; }
        public int id_empresa { get; set; }
        public int id_funcion { get; set; }
        public int id_genero { get; set; }
        public int id_contrato { get; set; }
        public int id_pais { get; set; }
        public int id_subacopio { get; set; }
        public int status { get; set; }
        public bool recontratable { get; set; }
        public int id_usuario_modifica { get; set; }
        public string pin_checador { get; set; }
    }
}
