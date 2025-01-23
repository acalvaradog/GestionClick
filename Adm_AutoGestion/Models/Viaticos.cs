using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

namespace Adm_AutoGestion.Models
{
    public class Viaticos
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public bool Prorroga { get; set; }
        public int IdSolicitud { get; set; }
        [Required]
        public int DestinoViaticoID { get; set; }
        [ForeignKey("DestinoViaticoID")]
        public DestinoViatico DestinoViatico { get; set; }
        [Required]
        public string MtvViaje { get; set; }
        [Required]
        public bool Hospedaje { get; set; }
        [Required]
        [StringLength(200)]
        public string Vehiculo { get; set; }
        public string Placa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        [Required]
        public int GastosTransporte { get; set; }
        public int GastoAlimentacion { get; set; }
        public bool CheckNomina { get; set; }
        public bool CheckTesoreria { get; set; }
        public int Estado { get; set; }
        [ForeignKey("Estado")]
        public EstadosViaticos EstadosViaticos { get; set; }
        public string Observacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool ViajeRealizado { get; set; }
        public string ViajeRealizadoObservacion { get; set; }
        public int Total { get; set; }
        public string CodContabilizacionSAP { get; set; }
        public bool CheckNominaCargue { get; set; }

        [NotMapped]
        public string Destino { get; set; }
        [NotMapped]
        public string EstadoNombre { get; set; }








    }
}
