using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class DestinoViatico 
    {
        
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Estado { get; set; }


    }
}
