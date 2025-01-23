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

    
    public class DetallePersonalVacunaRepository
    {

        public class resultado {
           
        }

       


        public string Crear(DetallePersonalSalud model) {

            string message = "";

            using (var db = new AutogestionContext()) {
                try {
                    if (model.TipoRegistro == 2)
                    {
                        db.DetallePersonalSalud.Add(model);
                        db.SaveChanges();
                        message = "Ok";

                    }
                    else if (model.TipoRegistro == 3){
                        DetalleApoyoLogisticoSalud detalle = new DetalleApoyoLogisticoSalud();
                        detalle.TipoRegistro = model.TipoRegistro;
                        detalle.IdTipoDocumento = model.IdTipoDocumento;
                        detalle.NumeroIdentificacion = model.NumeroIdentificacion;
                        detalle.PrimerApellido = model.PrimerApellido;
                        detalle.SegundoApellido = model.SegundoApellido;
                        detalle.PrimerNombre = model.PrimerNombre;
                        detalle.SegundoApellido = model.SegundoNombre;
                        detalle.CodigoMunicipio = "68276";
                        detalle.IdCargo = model.IdCargo;
                        detalle.CodigoEntidad = model.CodigoEntidad;
                        detalle.NombreEntidad = model.NombreEntidad;
                        detalle.IdDedicacion = model.IdDedicacion;
                        detalle.IdIndicadorActualizacion = model.IdIndicadorActualizacion;
                        detalle.UsuarioRegistraId = model.UsuarioRegistraId; 
                        detalle.FechaRegistro = DateTime.Now;
                        detalle.UsuarioModificaId = null;
                        detalle.FechaModifica = null;
                        detalle.Empresa = model.Empresa;
                        detalle.IdServicioCovid = model.IdServicioCovid;
                        detalle.IdAreaCovid = model.IdAreaCovid;
                       
                        db.DetalleApoyoLogisticoSalud.Add(detalle);
                        db.SaveChanges();
                        message = "Ok";
                       



 
                    }

                }
                
                
                catch {
                    message = "Error";
                
                }

                return message;
            
            }


        
        }


        public string Editar(int id, DetallePersonalSalud model)
        {

            string message = "";

            using (var db = new AutogestionContext()) {

            try{
             
               

                if( model.TipoRegistro == 2){

                    DetallePersonalSalud detalle = new DetallePersonalSalud(); 

                    detalle = db.DetallePersonalSalud.Find(id);
                    detalle.IdTipoDocumento = model.IdTipoDocumento;
                    detalle.IdPerfilProfesional = model.IdPerfilProfesional;
                    detalle.IdServicioCovid = model.IdServicioCovid;
                    detalle.IdAreaCovid = model.IdAreaCovid;
                    detalle.IdDedicacion = model.IdDedicacion;
                    detalle.IdCargo = model.IdCargo;
                    detalle.UsuarioModificaId = model.UsuarioModificaId;
                    detalle.FechaModifica = model.FechaModifica;
                    detalle.IdIndicadorActualizacion = model.IdIndicadorActualizacion;
                    db.SaveChanges();
                 

                    message = "Ok";

                }
                else if (model.TipoRegistro == 3) {
                    DetalleApoyoLogisticoSalud detalle1 = new DetalleApoyoLogisticoSalud();

                    detalle1 = db.DetalleApoyoLogisticoSalud.Find(id);
                    detalle1.IdTipoDocumento = model.IdTipoDocumento;
                    detalle1.IdAreaCovid = model.IdAreaCovid;
                    detalle1.IdDedicacion = model.IdDedicacion;
                    detalle1.IdCargo = model.IdCargo;
                    detalle1.UsuarioModificaId = model.UsuarioModificaId;
                    detalle1.FechaModifica = model.FechaModifica;
                    detalle1.IdIndicadorActualizacion = model.IdIndicadorActualizacion;
                    db.SaveChanges();
                    message = "Ok";
                }



            }catch{

                message = "Error";
            }


            return message; 

        } 
        
        }

        public string Eliminar(int id, DetallePersonalSalud detalle)
        {

            string message = "";
           
            using (var db = new AutogestionContext()) {


                try {

                    if (detalle.TipoRegistro == 2)
                    {

                           
                    DetallePersonalSalud personalsalud = new DetallePersonalSalud();
                    personalsalud= db.DetallePersonalSalud.Find(id);
                    personalsalud.FechaElimina = detalle.FechaElimina;
                    personalsalud.UsuarioEliminaId = detalle.UsuarioEliminaId;
                    personalsalud.IdIndicadorActualizacion = detalle.IdIndicadorActualizacion;
                    db.SaveChanges();
                    message = "Registro anulado correctamente";
                    }
                    else if (detalle.TipoRegistro == 3)
                    {
                        DetalleApoyoLogisticoSalud personalapoyo = new DetalleApoyoLogisticoSalud();

                        personalapoyo = db.DetalleApoyoLogisticoSalud.Find(id);
                        personalapoyo.FechaElimina = detalle.FechaElimina;
                        personalapoyo.UsuarioEliminaId = detalle.UsuarioEliminaId;
                        personalapoyo.IdIndicadorActualizacion = detalle.IdIndicadorActualizacion;
                        db.SaveChanges();
                        message = "Registro anulado correctamente";
                    }
                
                
                }
                catch (Exception ex){
                 
                    return ex.Message.ToString();
                   
                }
                    
                
                

                return message; 

            }
        }

      
    }


    
}