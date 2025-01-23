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


                if (opcion == "Confirmar")
                {
                    Items = db.ProcesoDisciplinario.Where(e => e.Estado =="Activo").ToList();
                    foreach (ProcesoDisciplinario Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
                    }
                }

                if (opcion == "Respuesta")
                {
                    Items = db.ProcesoDisciplinario.Where(e => e.Estado == "Remitido a Juridica").ToList();
                    foreach (ProcesoDisciplinario Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoRegistraId);
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

        internal void Modificar(string RJuridica, string Id, string Empleado, string Estado, int IdUsuarioM)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    ProcesoDisciplinario ProcesoDisciplinario = new ProcesoDisciplinario();
                    int empleado = 0;
                    int id = 0;
                    int.TryParse(Empleado, out empleado);
                    int.TryParse(Id, out id);

                    ProcesoDisciplinario = db.ProcesoDisciplinario.FirstOrDefault(e => e.Id == id);
                    db.ProcesoDisciplinario.Attach(ProcesoDisciplinario);

                    ProcesoDisciplinario.RespuestaJuridica = RJuridica;
                    ProcesoDisciplinario.Estado = Estado;
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }
       
    }
}