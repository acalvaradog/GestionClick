using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class ProcesoDisciplinarioRepository
    {
        public List<ProcesoDisciplinario> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                List<ProcesoDisciplinario> Items = db.ProcesoDisciplinario.ToList();
                foreach (ProcesoDisciplinario Item in Items)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                }
                return Items;
            }
        }

        public List<ProcesoDisciplinario> ObtenerTodos2(string opcion, string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {


                List<ProcesoDisciplinario> Items = new List<ProcesoDisciplinario>();
                Empleado datos = new Empleado();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);

                datos = db.Empleados.Find(id);

                if (opcion == "Jefe")
                {
                    Items = db.ProcesoDisciplinario.Where(e => e.Estado == "Activo" && e.EmpleadoRegistraId == datos.Id).ToList();
                    foreach (ProcesoDisciplinario Item in Items)
                    {
                        var x = Item.Id;
                        string Id = "" + x;
                        var Implicados = ObtenerTodos3(Id);
                        List<Empleado> z = new List<Empleado>();
                        foreach (PDTrabajador Item2 in Implicados)
                        {

                            z.Add(Item2.Empleado2);


                        }

                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Implicados = z;
                    }
                }


                if (opcion == "Confirmar")
                {
                    Items = db.ProcesoDisciplinario.Where(e => e.Estado =="Activo").ToList();
                    foreach (ProcesoDisciplinario Item in Items)
                    {
                        var x = Item.Id;
                        string Id = "" + x;
                        var Implicados = ObtenerTodos3(Id);
                        List<Empleado> z = new List<Empleado>();
                        foreach (PDTrabajador Item2 in Implicados)
                        {

                            z.Add(Item2.Empleado2);


                        }

                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Implicados = z;
                    }
                }

                if (opcion == "Respuesta")
                {
                    Items = db.ProcesoDisciplinario.Where(e => e.Estado == "Remitido a Juridica" || e.Estado == "Activo").ToList();
                    foreach (ProcesoDisciplinario Item in Items)
                    {
                        var x = Item.Id;
                        string Id = "" + x;
                        var Implicados = ObtenerTodos3(Id);
                        List<Empleado> z = new List<Empleado>();
                        foreach (PDTrabajador Item2 in Implicados)
                        {

                            z.Add(Item2.Empleado2);


                        }

                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                        Item.Implicados = z;
                    }
                }
                


                return Items;



            }
        }
       
        public List<PDTrabajador> ObtenerTodos3(string EmpleadoId)
        {
            using (var db = new AutogestionContext())
            {
                List<PDTrabajador> Items2 = new List<PDTrabajador>();

                int id = -1;
                Int32.TryParse(EmpleadoId, out id);



                Items2 = db.PDTrabajador.Where(e => e.ProcesoDisciplinarioId == id).ToList();
                

                foreach (PDTrabajador Item in Items2)
                {

                    Item.Empleado2 = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.Jefe = db.Empleados.FirstOrDefault(e=> e.NroEmpleado==Item.Empleado2.Jefe);

                }

                return Items2;


            }
        }

        public ProcesoDisciplinario ObtenerDatos(string Id)
        {
            using (var db = new AutogestionContext())
            {
                ProcesoDisciplinario Proceso = new ProcesoDisciplinario();
                int id = -1;
                Int32.TryParse(Id, out id);
                Proceso = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);

                return Proceso;
            }
        }



        internal void Crear(ProcesoDisciplinario model)
        {

            using (var db = new AutogestionContext())
            {
                model.Estado = "Activo";
                db.ProcesoDisciplinario.Add(model);
                db.SaveChanges();
               
       
            }
        }

        //public List<PDAnexos> ObtenerAnexos(string Id)
        //{
        //    using (var db = new AutogestionContext())
        //    {
        //        List<PDAnexos> Items = new List<PDAnexos>();
        //        int id = 0;
        //        Int32.TryParse(Id, out id);
        //        Items = db.PDAnexos.Where(x => x.IdProcesoDisciplinario == id).ToList();
        //        return Items;
        //    }

        //}
        //public List<PDPruebas> ObtenerPruebas(string Id)
        //{
        //    using (var db = new AutogestionContext())
        //    {
        //        List<PDPruebas> Items = new List<PDPruebas>();
        //        int id = 0;
        //        Int32.TryParse(Id, out id);
        //        Items = db.PDPruebas.Where(x => x.IdProcesoDisciplinario == id).ToList();
        //        return Items;
        //    }

        //}

        internal void Modificar(string Justificacion, string Id, string Estado)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    ProcesoDisciplinario ProcesoDisciplinario = new ProcesoDisciplinario();
                    int id = 0;                   
                    int.TryParse(Id, out id);
                    ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
                    db.ProcesoDisciplinario.Attach(ProcesoDisciplinario);
                    ProcesoDisciplinario.Justificación = Justificacion;
                    ProcesoDisciplinario.Estado = Estado;
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }
        internal void Modificar1(string Id, string Estado, string RespuestaJuridica, string TipoFalta, string TipoSancion, string MotivoSancion, string FechaCDescargo, string DiasSuspension, string FechaDescargo, DateTime FechaRespuestaJ,string NomAdj)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    ProcesoDisciplinario ProcesoDisciplinario = new ProcesoDisciplinario();
                    int id = 0;
                    int.TryParse(Id, out id);

                    ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
                    db.ProcesoDisciplinario.Attach(ProcesoDisciplinario);

                    ProcesoDisciplinario.TipoFalta=TipoFalta;
                    ProcesoDisciplinario.Sanciones = TipoSancion;
                    ProcesoDisciplinario.Suspencion = DiasSuspension;
                    ProcesoDisciplinario.FechaCitacionDes = DateTime.Parse(FechaCDescargo);
                    ProcesoDisciplinario.FechaDescargo = DateTime.Parse(FechaDescargo);
                    ProcesoDisciplinario.FechaRespuestaJ = FechaRespuestaJ;
                    ProcesoDisciplinario.RespuestaJuridica = RespuestaJuridica;
                    ProcesoDisciplinario.Estado = Estado;
                    ProcesoDisciplinario.MtvSancion = MotivoSancion;
                    ProcesoDisciplinario.AdjJuridico = NomAdj;
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }

        internal void Modificar2(string Id, string FechaSuspencion)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    ProcesoDisciplinario ProcesoDisciplinario = new ProcesoDisciplinario();;
                    int id = 0;
                    int.TryParse(Id, out id);
                    ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
                    db.ProcesoDisciplinario.Attach(ProcesoDisciplinario);
                    ProcesoDisciplinario.FechaSuspencion = DateTime.Parse(FechaSuspencion);
                    db.SaveChanges();

                }
                catch
                {
                }
            }
        }

        internal List<ProcesoDisciplinario> ObtenerInforme(string EmpleadoCP, string NroProceso, string Estado, string FechaInicio, string FechaFin)
        {
            using (var db = new AutogestionContext())
            {
                IQueryable<ProcesoDisciplinario> registros;

                var FI = new DateTime();
                var FF = DateTime.Now;

                if (!string.IsNullOrEmpty(FechaInicio))
                {
                   FI = Convert.ToDateTime(FechaInicio);
                }
                if (!string.IsNullOrEmpty(FechaFin))
                {
                    FF = Convert.ToDateTime(FechaFin);
                }

                if (Estado == "Todos")
                {
                    registros = db.ProcesoDisciplinario.Where(x=> FI <= x.FechaRegistro && FF >= x.FechaRegistro);
                }
                else
                {
                    registros = db.ProcesoDisciplinario.Where(x => x.Estado == Estado && FI <= x.FechaRegistro && FF >= x.FechaRegistro);
                }

                //var debug = registros.ToList();

                if (!string.IsNullOrEmpty(EmpleadoCP))
                {
                    int.TryParse(EmpleadoCP, out int empCP);
                    registros = registros.Where(x => db.ProcesoDisciplinario.Any(e => e.EmpleadoRegistraId == empCP && e.Id == x.Id));
                    //debug = registros.ToList();
                }

                if (!string.IsNullOrEmpty(NroProceso))
                {
                    int.TryParse(NroProceso, out int nro);
                    registros = registros.Where(x => db.ProcesoDisciplinario.Any(e => e.Id == nro && e.Id == x.Id));
                    //debug = registros.ToList();

                }

                return registros.ToList();
            }
        }
    }
}