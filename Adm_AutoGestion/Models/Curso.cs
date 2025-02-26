﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Categoria { get; set; }
        public bool EsObligatorio { get; set; }
        public string Modalidad { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime FechaCreado { get; set; }
    }
}