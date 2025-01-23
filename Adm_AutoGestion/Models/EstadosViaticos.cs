using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class EstadosViaticos 
    
    {
        [Key]
        public int id {  get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
         
    
    }
}
