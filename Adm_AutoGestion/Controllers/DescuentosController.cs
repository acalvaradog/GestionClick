﻿using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualBasic;


using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.Reporting.WebForms;
//using Microsoft.Reporting.WebForms;



namespace Adm_AutoGestion.Controllers
{

    public class datos
    {
        string nombre { get; set; }
        string documento { get; set; }
    }

    public class DescuentosController : Controller
    {
        //
        // GET: /Descuentos/
        private DescuentosRepository _repo;

        public DescuentosController()
        {
            _repo = new DescuentosRepository();

        }





        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Descuentoslista"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }



            var model = _repo.ObtenerTodos();
           
            return View(model);
        }

        //
        // GET: /Descuentos/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Descuentos/Create

        public ActionResult Create(string Codigo, string Documento)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DescuentosCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Session.Remove("Nombres");
            Session.Remove("Documento");
            Session.Remove("EmpleadoId");
            Descuentos model = new Descuentos();
            string message = "";
            int empleado = 0;
            using (var db = new AutogestionContext())
            {

                model.ListadoServicios = db.Servicios.Where(e => e.Activo == true).ToList();


                if (Codigo != null && Documento != null)
                {


                    var datos = db.TopeDescuento.FirstOrDefault(e => e.CodigoEmpleado == Codigo || e.NroDocumento == Documento);

                    if (datos != null)
                    {

                        //Int32.TryParse(datos.CodigoEmpleado, out empleado);

                        var emp = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Codigo || e.Documento == Documento);
                        Session["Nombres"] = emp.Nombres;
                        Session["Documento"] = emp.Documento;
                        Session["Impresion"] = "Vacio";
                        Session["EmpleadoId"] = emp.Id;
                        model.EmpleadoId = empleado;
                        model.TopeMaximo = datos.Valor;

                    }
                    else
                    {
                        message = "El empleado no cuenta con descuentos activos.";
                    }

                }

            }

            Session["message"] = message;
            return View(model);
        }

        //
        // POST: /Descuentos/Create

        [HttpPost]
        public ActionResult Create(Descuentos model)
        {
            string message = "";
            var message1 = "";
            

            try
            {
                if (model.ValorDescuento < model.TopeMaximo)
                {
                    // TODO: Add insert logic here
                    if (ModelState.IsValid)
                    {

                        int empleadoid = Convert.ToInt32(Session["EmpleadoId"]);
                        model.EmpleadoId = empleadoid;
                        var res = _repo.Crear(model);
                        if (res == "Ok")
                        {

                            message1 = "Los datos fueron registrados correctamente";
                            string nombres = string.Format("{0}", Session["Nombres"]);
                            string documento = string.Format("{0}", Session["Documento"]);
                            ViewBag.Nombres = nombres;
                            ViewBag.Documento = documento;
                            //Impresion(nombres, documento);
                            //Session.Remove("Nombres");
                            //Session.Remove("Documento");
                            //Session.Remove("EmpleadoId");
                            Session["Impresion"] = "Ok";
                            Session["message1"] = message1;
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            message = "Se presento un error al guardar los datos.";
                        }

                    }
                }
                else
                {
                    using (var db = new AutogestionContext())
                    {

                        model.ListadoServicios = db.Servicios.Where(e => e.Activo == true).ToList();
                    }
                    message = "El valor del descuento no puede superar el tope Maximo.";
                }

            }
            catch
            {

            }
            Session["message1"] = message1;
            Session["message"] = message;
            using (var db = new AutogestionContext())
            {

                model.ListadoServicios = db.Servicios.Where(e => e.Activo == true).ToList();
            }
            return View(model);

        }

        //
        // GET: /Descuentos/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DescuentosEditar"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model = new Descuentos();



            using (var db = new AutogestionContext())
            {
                model = db.Descuentos.Find(id);
                model.ListadoServicios = db.Servicios.Where(e => e.Activo == true).ToList();
                var emp = db.Empleados.FirstOrDefault(e => e.Id == model.EmpleadoId);
                Session["Nombres"] = emp.Nombres;
                Session["Documento"] = emp.Documento;
                ViewBag.Estado = model.Activo;
            }



            return View(model);
        }

        //
        // POST: /Descuentos/Edit/5
        public void EnvioAimpresion(int Id)
        {

            using (var db = new AutogestionContext())
            {

                var emp = db.Empleados.FirstOrDefault(e => e.Id == Id);

                Impresion(emp.Nombres, emp.Documento);
            }

        }

        public String GenerarImpresion(datos model)
        {

            //string nombre, string documento
            var nombres = "mayra";
            var documentos = "46";

            var Data = new DataSet1();
            var ReportViewer = new Microsoft.Reporting.WebForms.ReportViewer();

            Data.Tables[0].Rows.Add(new object[] { nombres, documentos, DateTime.Now });

            ReportViewer.LocalReport.DataSources.Clear();
            ReportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", Data.Tables[0]));
            ReportViewer.LocalReport.ReportEmbeddedResource = "Report2.rdlc";
            ReportViewer.LocalReport.ReportPath = "Report2.rdlc";
            ReportViewer.LocalReport.Refresh();

            string File = string.Format("{0}\\{1}-Carta Autorización Descuento.pdf", Server.MapPath("~/AnexosPermisos"), Session["Empleado"]);
            byte[] Buffer = ReportViewer.LocalReport.Render("PDF");
            System.IO.File.WriteAllBytes(File, Buffer);

            return File;

            //return File(Buffer, "application/pdf", "Carta Autorización Descuento");
            //return System.Convert.ToBase64String(ReportViewer.LocalReport.Render("PDF"));

        }

        [HttpGet]
        public ActionResult Impresion(string nombre, string documento)
        {


            Session.Remove("Nombres");
            Session.Remove("Documento");
            Session.Remove("Impresion");
            var Data = new DataSet1();
            var ReportViewer = new Microsoft.Reporting.WebForms.ReportViewer();

            Data.Tables[0].Rows.Add(new object[] { nombre, documento, DateTime.Now });

            ReportViewer.LocalReport.DataSources.Clear();
            ReportViewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", Data.Tables[0]));
            ReportViewer.LocalReport.ReportEmbeddedResource = "Report2.rdlc";
            ReportViewer.LocalReport.ReportPath = "Report2.rdlc";
            ReportViewer.LocalReport.Refresh();


            var buffer = ReportViewer.LocalReport.Render("PDF");
            //string File = string.Format("{0}-Soporte-Descuento.pdf", Session["Empleado"]);
            //System.IO.File.WriteAllBytes(System.IO.Path.Combine(Server.MapPath("~/AnexosPermisos"),File), buffer);

            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = String.Empty;
            Response.AddHeader("content-disposition", "attachment; filename=Report.pdf");
            Response.BinaryWrite(buffer);
            Response.Flush();
            Response.End();
            return Json("ok");
            //return buffer;
            //return Server.MapPath("~/AnexosPermisos" + File);            
        }


        public ActionResult CargarArchivoPlano()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DescuentosCargaPlano"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }




            DescuentosReportados model = new DescuentosReportados();
            using (var db = new AutogestionContext())
            {

                model.ListadoServicios = db.Servicios.Where(e => e.Activo == true).ToList();

            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CargarArchivoPlano(DescuentosReportados model, HttpPostedFileBase file)
        {

            var ruta = "";
            var contlin = 0;
            var message = "";
            var message1 = "";

            try
            {

                if (HttpContext.Request.Files.AllKeys.Any())
                {
                    //Get the uploaded image from the Files collection
                    var httpPostedFile = HttpContext.Request.Files[0];

                    if (httpPostedFile != null)
                    {
                        // Validate the uploaded image(optional)

                        // Get the complete file path
                        DateTime date1 = DateTime.Now;
                        var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + httpPostedFile.FileName;
                        var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosPermisos"), nombrearchivo);
                        ruta = fileSavePath;


                        // Save the uploaded file to "UploadedFiles" folder
                        httpPostedFile.SaveAs(fileSavePath);
                    }



                    DescuentosReportados descuentos = new DescuentosReportados();
                    string EmpleadoId = String.Format("{0}", Session["Empleado"]);
                    int Empleado = 0;
                    Int32.TryParse(EmpleadoId, out Empleado);
                    TextReader Leer = new StreamReader(ruta);
                    //string linea = Leer.ReadLine();
                    string datos = Leer.ReadToEnd();
                    var filas = datos.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var lineas in filas)
                    {
                        var linea1 = lineas.Split(';');
                        descuentos.Mes = model.Mes;
                        descuentos.anio = model.anio;
                        descuentos.ServicioId = model.ServicioId;
                        descuentos.UsuarioId = Empleado;
                        descuentos.NroDocumento = linea1[0];
                        descuentos.ValorDescuento = linea1[1];
                        descuentos.Fecha = DateTime.Now;
                        contlin = contlin + 1;
                        using (var db = new AutogestionContext())
                        {
                            db.DescuentosReportados.Add(descuentos);
                            db.SaveChanges();
                        }

                    }

                    message = "Se cargaron " + contlin + " lineas con exito.";

                    Leer.Close();
                    System.IO.File.Delete(ruta);



                }
            }
            catch (SystemException ex)
            {
                message1 = "Ocurrio un error. " + ex.Message;
            }

            Session["message"] = message;
            Session["message1"] = message1;
            return RedirectToAction("Index");
        }


        public ActionResult GenerarCSV()
        {

            List<string> funciones = Acceso.Validar(Session["Empleado"]);

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("DescuentosDescargarPlano"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpPost]
        public FileResult GenerarCSV(string mes, string anio)
        {
            string fichero = "";
            string ruta = "";
            List<DescuentosReportados> datos = new List<DescuentosReportados>();
            using (var db = new AutogestionContext())
            {
                datos = db.DescuentosReportados.Where(e => e.Mes == mes && e.anio == anio).ToList();

                List<string> strFiles = Directory.GetFiles(Path.Combine(HttpContext.Server.MapPath("~/AnexosPermisos/temp")), "*", SearchOption.AllDirectories).ToList();

                foreach (string ficheros in strFiles)
                {
                    System.IO.File.Delete(ficheros);
                }
                //var ficherosCarpeta = Path.Combine(HttpContext.Server.MapPath("~/AnexosPermisos/temp"));
                //System.IO.File.Delete(ficherosCarpeta);

                var hora = DateTime.Now.ToString("hh:mm:ss");
                DateTime fecha = DateTime.Now; // Instancio un objeto DateTime
                fichero = fecha.Day.ToString() + "-" + fecha.Month.ToString() + "-" + fecha.Year.ToString() + "-" + fecha.Hour.ToString() + "-" + fecha.Minute.ToString() + "-" + fecha.Second.ToString() + ".csv"; // Usando el objeto DateTime consigo dia-mes-año y lo monto en un string
                var fileSavePath = Path.Combine(HttpContext.Server.MapPath("~/AnexosPermisos/temp"), fichero);
                ruta = fileSavePath;
                System.IO.File.Create(fileSavePath).Close();

                TextWriter Escribir = new StreamWriter(fileSavePath);

                foreach (DescuentosReportados Item in datos)
                {
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Documento == Item.NroDocumento);
                    Escribir.WriteLine(Convert.ToDateTime(Item.Fecha).ToString("dd/MM/yyyy") + ";" + Item.Empleado.NroEmpleado + ";" + "2T62" + ";" + Item.ValorDescuento + ";" + "01;" + ";");
                }
                Escribir.Close();
            }
            var fileBytes = Server.MapPath(@"..\AnexosPermisos\temp\" + fichero);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fichero);


        }




        [HttpPost]
        public ActionResult Edit(int id, Descuentos model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {

                    _repo.Editar(model);

                    return RedirectToAction("Index");
                }
            }
            catch
            {

            }
            return View();
        }

        //
        // GET: /Descuentos/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Descuentos/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}