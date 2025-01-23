using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models.EvaDesempeno
{
    [Table("usuarios x sociedad")]
    public class usuarios_x_sociedad
    {

        //public int Id { get; set; }
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Required]
        public int usuario { get; set; }
        [Key, Column(Order = 2), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int sociedad { get; set; }
    }
}