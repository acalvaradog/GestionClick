using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.IO;
using System.Threading.Tasks;
using System.Data.Entity;
namespace Adm_AutoGestion.Controllers
{
    public class NoticiaController : Controller
    {

        private NoticiaRepository _repo;

        public NoticiaController()
        { 
        _repo = new NoticiaRepository();

        }
        // GET: /Noticia/

        public ActionResult Index()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("Noticia"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            var model= _repo.ObtenerTodos().Result;
            return View(model);
        }

        //
        // GET: /Noticia/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Noticia/Create

        public ActionResult Create()
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("NoticiaCrear"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /Noticia/Create

        [HttpPost]
        public ActionResult Create(Noticia model)
        {
            try
            {
               
                var respuesta = "";
                // TODO: Add insert logic here
                if (model.Titulo == null || model.Titulo== "") 
                {
                    respuesta = respuesta + " </br> El título es necesario ";
                }
                if (model.Contenido == null || model.Contenido == "")
                {
                    respuesta = respuesta + "</br> El Contenido es necesario";
                }
                if (model.Publicacion == null || model.Publicacion == new DateTime() )
                {
                    respuesta = respuesta + "</br> La Fecha de Publicación es necesaria";
                }

                if (respuesta != "") 
                {
                    Session.Add("message", "");
                    return View();
                }
                else 
                {
                    model.EmpleadoId = Convert.ToInt32(Session["Empleado"]);
                    _repo.Crear(model);
                    Session.Add("Success", "Guardado Exitoso");
                    return RedirectToAction("Index");
                }
                
                
               
            }
            catch (Exception ex)
            {
                Session.Add("message", ex);
                return View();
            }
      
        }

        //
        // GET: /Noticia/Edit/5

        public ActionResult Edit(int id)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("NoticiaEdit"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        //
        // POST: /Noticia/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Noticia/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Noticia/Delete/5

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


        public ActionResult pruebas()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearNoticia(Noticia model, IEnumerable<HttpPostedFileBase> Imagenes)
        {
            model.EmpleadoId = Convert.ToInt32(Session["Empleado"]);
            model.Publicacion = DateTime.Now;
            // Validación del modelo
            if (ModelState.IsValid)
            {
                string rutaCarpeta = Server.MapPath("~/ImagenesNoticias");
                // Asegurarse de que la carpeta exista
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                // Llamar al repositorio para guardar la noticia y procesar las imágenes
                _repo.CrearNoticia(model, Imagenes, rutaCarpeta);

                TempData["MensajeExito"] = "La noticia se guardó correctamente.";
                return RedirectToAction("Index"); ;
            }
            else
            {
                TempData["MensajeError"] = "Hubo un problema al guardar la noticia.";
                return View();
            }
        }

        public async Task<ActionResult> CrearNoticia()
        {
            return View();
        }

        public async Task<ActionResult> EditarNoticia(int id)
        {
            var noticia = await _repo.ObtenerNoticia(id);
            if (noticia == null)
            {
                return HttpNotFound();
            }

            return View(noticia);

        }
            // Acción para actualizar la noticia
            [HttpPost]
        public ActionResult EditarNoticia(Noticia model, IEnumerable<HttpPostedFileBase> Imagenes)
        {
            if (ModelState.IsValid)
            {
                string rutaCarpeta = Server.MapPath("~/ImagenesNoticias");
                _repo.ActualizarNoticia(model, Imagenes, rutaCarpeta);

                TempData["MensajeExito"] = "La noticia se actualizó correctamente.";
                return RedirectToAction("Index");
            }

            TempData["MensajeError"] = "Hubo un problema al actualizar la noticia.";
            return View(model);
        }
    }


}
