using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class IncapacidadesRepository
    {

        public List<Incapacidades> ObtenerTodos(string opcion)
        {
            using (var db = new AutogestionContext())
            {

                List<Incapacidades> Items = new List<Incapacidades>();
                int eps = 0;

                if (opcion == "Index")
                {
                     Items = db.Incapacidades.ToList();
                    foreach (Incapacidades Item in Items)
                    {
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadosIncapacidades = db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                        Item.ListadoTiposInc = db.TiposIncapacidad.ToList();
                    }
                }


                if (opcion == "Aprobar") {

                    Items = db.Incapacidades.Where(e => e.EstadoId == 1).ToList();
                    foreach (Incapacidades Item in Items)
                    {
                        Int32.TryParse(Item.EPS, out eps);
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadosIncapacidades = db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                        Item.ListadoTiposInc = db.TiposIncapacidad.ToList();
                        Item.ListadoAdjuntos = db.IncapacidadAdjuntos.ToList();
                        Item.ListadoEps = db.EPS.FirstOrDefault(s => s.Id == eps);
                    }
                }

                if (opcion == "Cargar") {

                    Items = db.Incapacidades.Where(e => e.EstadoId == 2).ToList();
                    foreach (Incapacidades Item in Items)
                    {
                        Int32.TryParse(Item.EPS, out eps);
                        Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                        Item.EstadosIncapacidades = db.EstadosIncapacidades.FirstOrDefault(x => x.Id == Item.EstadoId);
                        Item.ListadoTiposInc = db.TiposIncapacidad.ToList();
                        Item.ListadoAdjuntos = db.IncapacidadAdjuntos.ToList();
                        Item.ListadoEps = db.EPS.FirstOrDefault(s => s.Id == eps);
                    }
                }



                    return Items;
            }
        }

        internal void Crear(Incapacidades model)
        {

            using (var db = new AutogestionContext())
            {
                try
                {
                    Incapacidades Incapacita = new Incapacidades();
                    IncapacidadAdjuntos Adjuntos = new IncapacidadAdjuntos();
                    var empleado = db.Empleados.Find(model.EmpleadoId);

                    Incapacita.EmpleadoId = model.EmpleadoId;
                    Incapacita.Fecha = model.Fecha;
                    Incapacita.FechaInicio = model.FechaInicio;
                    Incapacita.FechaFin = model.FechaFin;
                    Incapacita.CantidadDias = model.CantidadDias;
                    Incapacita.EPS = model.EPS;
                    Incapacita.Empresa = empleado.Empresa;
                    //Incapacita.Diagnostico = diagnostico;
                    Incapacita.EstadoId = 1;

                    db.Incapacidades.Add(Incapacita);
                    db.SaveChanges();

                    foreach (var Item in model.ListadoAdjuntos)
                    {
                        Adjuntos.IncapacidadId = Incapacita.Id;
                        Adjuntos.TipoIncapacidad = Item.TipoIncapacidad;
                        Adjuntos.Adjunto = Item.Adjunto;
                        db.IncapacidadAdjuntos.Add(Adjuntos);
                        db.SaveChanges();
                    }
                }
                catch
                {
                }
            }

        }

        public List<IncapacidadAdjuntos> ObtenerAdjuntos(string Id, int IndJefe)
        {
            using (var db = new AutogestionContext())
            {

                List<IncapacidadAdjuntos> Items = new List<IncapacidadAdjuntos>();
                int id = 0;
                Int32.TryParse(Id, out id);

                List<string> idsOmitir = new List<string> { "2", "3", "7", "11", "12", "16", "26", "28" };

                //Items = db.IncapacidadAdjuntos.Where(x => x.IncapacidadId == id).ToList();

                // Realiza la consulta condicionando la inclusión basada en IndJefe de manera directa
                Items = db.IncapacidadAdjuntos
                              .Where(x => x.IncapacidadId == id && (IndJefe != 1 || !idsOmitir.Contains(x.TipoAdjunto)))
                              .ToList();

                int tipos = 0;
                int tipoAdjunto = 0;
                foreach (IncapacidadAdjuntos Item in Items)

                {
                    Int32.TryParse(Item.TipoIncapacidad, out tipos);
                    Int32.TryParse(Item.TipoAdjunto, out tipoAdjunto);
                    Item.ListadoTiposInc = db.TiposIncapacidad.FirstOrDefault(x => x.Id ==tipos);

                    Item.NombreAdjunto = db.Adjunto
                                            .Where(x => x.Id == tipoAdjunto)
                                            .Select(a => a.Nombre) 
                                            .FirstOrDefault();
                }
                return Items;
            }
            
        }


        internal void Modificar(string Id, string Empleado, string Observacion, string Estado)
        {

            using (var db = new AutogestionContext())
            {
                try
                {

                    HistorialIncapacidades Historial = new HistorialIncapacidades();
                    Incapacidades Incapacita = new Incapacidades();
                    int estado = 0;
                    int empleado = 0;
                    int id = 0;
                    DateTime Fecha2 = DateTime.Now;

                    int.TryParse(Estado, out estado);
                    int.TryParse(Empleado, out empleado);
                    int.TryParse(Id, out id);


                    Incapacita = db.Incapacidades.FirstOrDefault(e => e.Id == id);
                    db.Incapacidades.Attach(Incapacita);
                    Incapacita.EstadoId = estado;
                    db.SaveChanges();


                    Historial.IncapacidadId = id;
                    Historial.Fecha = Fecha2;
                    Historial.Accion = Estado;
                    Historial.EmpleadoId = empleado;
                    Historial.Observaciones = Observacion;
                    db.HistorialIncapacidades.Add(Historial);
                    db.SaveChanges();

                }
                catch
                {
                }
            }

        }





    }
}