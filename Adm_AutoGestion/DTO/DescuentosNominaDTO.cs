using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class DescuentosNominaDTO
    {
        public int Id { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; }
        public int DependenciaId { get; set; }
        public string DependenciaNombre{ get; set; }
        public int SedeId { get; set; }
        public string SedeNombre{ get; set; }

        public int ValorCompra { get; set; }
        public int? ValorAbono { get; set; }
        public int Saldo { get; set; }
        public int NumeroCuotas { get; set; }
        public string Observacion { get; set; }
        public string HistorialObservacion { get; set; }
        public string Factura { get; set; }

    }
}