using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Services;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Adm_AutoGestion.Services


{
    
    public class EncabezadoPrestamoRepository
    {

        private ServiciosRepository _servicios = new ServiciosRepository();
        

        public string Crear(Tercero model) {

             string message = "";

            using (var db = new AutogestionContext()) {

                try {
                    //var tercero = db.Tercero.Where(x => x.Documento == model.Documento).FirstOrDefault();
                    string QRtext = model.Nombres + "|" + model.Documento + "|" + model.Cargo;
                    string respuesta = _servicios.encriptar(QRtext);                 
                    model.QRPrestamo = respuesta;


                    db.Tercero.Add(model);
                    db.SaveChanges();


                    message = "Registro Guardado Correctamente";
                }
                catch{
                    message = "Error al Guardar Registro";
                }

                return message; 

            
            }



        
        }


        public string RegistrarPrestamo(EncabezadoPrestamo model, string correo, string nombres) {

            _servicios = new ServiciosRepository();
            string message = "";
            using (var db = new AutogestionContext()) {


                try
                {

                    EncabezadoPrestamo encabezado = new EncabezadoPrestamo(); 
                    DetalleEncabezadoPrestamo detalle =  new DetalleEncabezadoPrestamo();

                    encabezado.Documento = model.Documento;
                    encabezado.FechaRegistro = model.FechaRegistro;
                    encabezado.AreaDirige = model.AreaDirige;
                    encabezado.TipoArea = model.TipoArea;
                    encabezado.LugarEntrega = model.LugarEntrega;
                    encabezado.Estado = model.Estado;
                    encabezado.Sociedad = model.Sociedad;
                    encabezado.QRPrestamos = model.QRPrestamos;
                    encabezado.UsuarioRegistraId = model.UsuarioRegistraId;
                    db.EncabezadoPrestamo.Add(encabezado);
                    db.SaveChanges();

                    foreach (var item in model.ListadoElementos) {

                        detalle.IdEncabezadoPrestamo = encabezado.Id;
                        detalle.IdTipoElementos = item.IdTipoElementos;
                        detalle.Documento = model.Documento;
                        detalle.Cantidad = item.Cantidad;
                        detalle.FechaEntrega = model.FechaRegistro;
                        detalle.Talla = item.Talla;
                        detalle.Estado = model.Estado;
                        db.DetalleEncabezadoPrestamo.Add(detalle);
                        db.SaveChanges(); 
                    
                    }

                    List<DetalleEncabezadoPrestamo> detalleprestamo = new List<DetalleEncabezadoPrestamo>();

                    detalleprestamo = db.DetalleEncabezadoPrestamo.Where(x => x.IdEncabezadoPrestamo == encabezado.Id).ToList();

                    var IdEncabezado = encabezado.Id;

                    _servicios.EnviarEmailPrestamo(correo, IdEncabezado, nombres);
                   


                   
                    message = "Registro Guardado Correctamente";


                }
                catch {

                    message = "Error al guardar registro";
                }
                return message; 

            }
        
        }


        public string RegistrarDevolucion(int id, EncabezadoPrestamo model, int IdUsuarioR)
        {
            
            string message = "";

            _servicios = new ServiciosRepository();

            using (var db = new AutogestionContext()) {

                try
                {

                    EncabezadoPrestamo encabezado =  new EncabezadoPrestamo(); 
                    DetalleEncabezadoPrestamo  detalle = new DetalleEncabezadoPrestamo(); 

                    encabezado = db.EncabezadoPrestamo.Find(id);
                    encabezado.Estado = model.Estado;
                    db.SaveChanges();

                    var detalleprestamos = db.DetalleEncabezadoPrestamo.Where(e => e.IdEncabezadoPrestamo == id).ToList(); 

                    foreach (var item in detalleprestamos ) {
                        detalle  = db.DetalleEncabezadoPrestamo.Find(item.Id);
                        db.DetalleEncabezadoPrestamo.Attach(detalle);
                        detalle.Estado =  "INACTIVO";
                        detalle.UsuarioModificaId = IdUsuarioR; 
                        detalle.FechaFirmaRecibido = DateTime.Now;
                        db.SaveChanges(); 
                  
                    }

                        var correo = "";
                        var nombres = "";
                        Empleado empleado = new Empleado();
                        empleado = db.Empleados.FirstOrDefault(x => x.Documento == model.Documento);

                        if (empleado != null)
                        {

                            correo = empleado.Correo;
                            nombres = empleado.Nombres;

                        }
                        else
                        {

                            Tercero tercero = new Tercero();
                            tercero = db.Tercero.FirstOrDefault(x => x.Documento == model.Documento);

                            if (tercero != null)
                            {
                                correo = tercero.CorreoPersonal;
                                nombres = tercero.Nombres;

                            }

                        }

                        _servicios.EnviarEmailDevolucion(correo, id, nombres);
                    message = "Registro modificado correctamente";
                    
                }
                catch (SystemException ex)
                {

                    return ex.Message.ToString(); 
                
                }
                return message; 
            
            
            
            }   
        }

        public string AnularPrestamo(string Documento, int IdUsuarioR)
        {

            string message = "";
            _servicios = new ServiciosRepository();
            using (var db = new AutogestionContext())
            {
                try
                {
                    
                   
                    EncabezadoPrestamo prestamo = new EncabezadoPrestamo();

                    prestamo = db.EncabezadoPrestamo.FirstOrDefault(e => e.Documento == Documento && e.Estado =="ACTIVO");
                    db.EncabezadoPrestamo.Attach(prestamo);

                    
                        prestamo.Estado = "ANULADO";
                    

                        List<DetalleEncabezadoPrestamo> Items = db.DetalleEncabezadoPrestamo.Where(x => x.Documento == Documento && x.Estado == "ACTIVO").ToList();
                        foreach (DetalleEncabezadoPrestamo Item in Items)
                        {
                            db.DetalleEncabezadoPrestamo.Attach(Item);
                            
                                Item.Estado = "ANULADO";
                                Item.UsuarioModificaId = IdUsuarioR;      
                            db.SaveChanges();
                        }

                        var correo = "";
                        var nombres = "";
                        Empleado empleado = new Empleado();
                        empleado = db.Empleados.FirstOrDefault(x => x.Documento == Documento);

                        if (empleado != null)
                        {

                            correo = empleado.Correo;
                            nombres = empleado.Nombres;

                        }
                        else
                        {

                            Tercero tercero = new Tercero();
                            tercero = db.Tercero.FirstOrDefault(x => x.Documento == Documento);

                            if (tercero != null)
                            {
                                correo = tercero.CorreoPersonal;
                                nombres = tercero.Nombres;

                            }

                        }


                        _servicios.EnviarEmailAnulacion(correo,prestamo.Id, nombres);
                       return message = "El registro fue Anulado Correctamente"; 

   

                }
                catch (SystemException ex)
                {

                    return ex.Message.ToString(); 
                }
            }
        }
        
        public string ActualizarQR(Tercero model, string QR) 
        {
            using (var db = new AutogestionContext())
            {
                try 
                {
                    Tercero tercero = new Tercero();

                    tercero = db.Tercero.Find(model.Id);
                    tercero.QRPrestamo = QR;
                    db.SaveChanges();
                    return "Se ha actualizado correctamente";
                }
                catch (Exception ex) 
                {

                    return "" + ex;
                }
               
            }
              
        }

    }


}