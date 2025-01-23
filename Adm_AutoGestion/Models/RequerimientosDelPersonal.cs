using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class RequerimientosDelPersonal
    {
        public int Id { get; set; }

        public int EmpleadoRegistraId { get; set; }
        [ForeignKey("EmpleadoRegistraId")]
        public Empleado Empleado { get; set; }

        public int EmpresaId { get; set; }//Maestro
        [ForeignKey("EmpresaId")]
        public Sociedad Empresa { get; set; }

        public string Cargo { get; set; }
        public DateTime Fecha { get; set; }
        public string Area { get; set; }
        public string Direccion { get; set; }
        public int NumeroPresonas { get; set; }
        [StringLength(15)]
        public string Sexo { get; set; }

        public int MtvSolicitudID { get; set; }//Maestro
        [ForeignKey("MtvSolicitudID")]
        public MtvSolicitud MtvSolicitud { get; set; }

        

        public int JornadaRequeridaId { get; set; }//Maestro
        [ForeignKey("JornadaRequeridaId")]
        public Jornada Jornada { get; set; }

        public int HorarioTrabajoId { get; set; }//Maestro
        [ForeignKey("HorarioTrabajoId")]
        public Horario Horario { get; set; }

        public string FechaSugeridaIngreso { get; set; }
        public string JustificacionCargo { get; set; }

        public int EstadoID { get; set; }
        [ForeignKey("EstadoID")]
        public EstadoRdP EstadoRdP { get; set; }

        public int EstadoSeleccion { get; set; }
        [ForeignKey("EstadoSeleccion")]
        public EstadoSeleccionRdP EstadoSeleccionRdP { get; set; }

        public string TiempoContratacion { get; set; }
        public string RequisitosAdicionales { get; set; }
        public int EncargadoContratacion { get; set; }
        public string Archivo { get; set; }
        public string TipoConcurso { get; set; }
        public string Contratacion { get; set; }
        public int CeCos { get; set; }
        public string Cual { get; set; }
        [NotMapped]
        public virtual string Color { get; set; }
        [NotMapped]
        public virtual string Tiempo { get; set; }
        [NotMapped]
        public List<Empleado> Empleados { get; set; }
        [NotMapped]
        public List<DetalleRequerimiento> DetalleRequerimiento { get; set; }
        [NotMapped]
        public DetalleRequerimiento detalle { get; set; }

        //--Cantidad Requerimientos Cubiertos
        [NotMapped]
        public List<DetalleRequerimiento> Detalle_Requerimientos_Cont { get; set; }
        [NotMapped]
        public int Requerimientos_Cont { get; set; }

    }
}