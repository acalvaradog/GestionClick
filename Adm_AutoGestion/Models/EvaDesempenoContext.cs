using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Adm_AutoGestion.Models.EvaDesempeno;
namespace Adm_AutoGestion.Models
{
    public class EvaDesempenoContext : DbContext
    {
        public EvaDesempenoContext()
            :base("ConexionEvaDesempeno")
        {
 
        }

        //para poder hacerle querys al modelo, se indica al frente el nombre de la tabla que a crear

        public DbSet<encuestadores_x_empleado> Encuestadoresxempleado { get; set; }
        public DbSet<empleados> empleados { get; set; }
        public DbSet<areas> areas { get; set; }
        public DbSet<aspectos_generales> aspectos_generales { get; set; }
        public DbSet<calificaciones> Calificaciones { get; set; }
        public DbSet<cargos_x_competencias> Cargos_x_competencias { get; set; }
        public DbSet<cargos> Cargos { get; set; }
        public DbSet<dominios> Dominios { get; set; }
        public DbSet<Empleados_Cargo_Area_x_Periodo> Empleados_Cargo_Area_x_Periodo { get; set; }
        public DbSet<estados> Estados { get; set; }
        public DbSet<evaluacion_aspecto_general> Evaluacion_aspecto_general { get; set; }
        public DbSet<evaluacion_conductuales> Evaluacion_conductuales { get; set; }
        public DbSet<evaluacion_encabezado> Evaluacion_encabezado { get; set; }
        public DbSet<evaluacion_institucionales> Evaluacion_institucionales { get; set; }
        public DbSet<evaluacion_otros> Evaluacion_otros { get; set; }
        public DbSet<niveles> Niveles { get; set; }
        public DbSet<perfiles> Perfiles { get; set; }
        public DbSet<periodos> Periodos { get; set; }
        public DbSet<sedes> Sedes { get; set; }
        public DbSet<seguimientos> Seguimientos { get; set; }
        public DbSet<sociedades> Sociedades { get; set; }
        public DbSet<Temp> Temp { get; set; }
        public DbSet<Temp_Cod> Temp_Cod { get; set; }
        public DbSet<tipo_competencias> Tipo_competencias { get; set; }
        public DbSet<tipo_evaluacion> Tipo_evaluacion { get; set; }
        public DbSet<tipo_evaluador> Tipo_evaluador { get; set; }
        public DbSet<tipo_identificacion> Tipo_identificacion { get; set; }
        public DbSet<usuario_perfil> Usuario_perfil { get; set; }
        public DbSet<usuarios_x_sociedad> Usuarios_x_sociedad { get; set; }
        public DbSet<Periodos_Evaluacion_Desempeño> Periodos_Evaluacion_Desempeño { get; set; }
        public DbSet<Estructura_Jerarquica_EVADES> Estructura_Jerarquica_EVADES { get; set; }
        public DbSet<EstadosEvaluacionEncabezado> EstadosEvaluacionEncabezado { get; set; }
        public DbSet<Seguimientos_x_Registro> Seguimientos_x_Registro { get; set; }

    }

}