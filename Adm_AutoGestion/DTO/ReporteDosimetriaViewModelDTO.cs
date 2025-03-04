using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class ReporteDosimetriaViewModelDTO
    {
        public int EmpleadoId { get; set; }
        public string NombreEmpleado { get; set; }
        public int SedeId { get; set; }
        public string NombreSede { get; set; }
        public List<ValoresPorMesDTO> ValoresPorMes { get; set; }
        public decimal? TotalAnualHp10 { get; set; }
        public decimal? TotalAnualHp3 { get; set; }
        public decimal? TotalAcumuladoHp10 { get; set; }
        public decimal? TotalAcumuladoHp3 { get; set; }

        public int año {  get; set; }
    }
}