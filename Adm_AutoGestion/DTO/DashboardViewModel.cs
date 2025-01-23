using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class DashboardViewModel
    {
        public IEnumerable<ReporteDosimetriaViewModelDTO> DatosHp10 { get; set; }
        public IEnumerable<ReporteDosimetriaViewModelDTO> DatosHp3 { get; set; }
        public IEnumerable<ReporteDosimetriaViewModelDTO> DatosAcumuladoHp10 { get; set; }
        public IEnumerable<ReporteDosimetriaViewModelDTO> DatosAcumuladoHp3 { get; set; }
        public object Limites { get; set; }
    }
}