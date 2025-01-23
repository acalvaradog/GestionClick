using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models.CertificadosEduFoscal
{
    public class vista_certificados_reducida
    {
        [StringLength(100)]
        public string username { get; set; }
        [Key]
        public int user_id { get; set; }
        public int course_id { get; set; }
        public string certificate_url {  get; set; }
    }
}
