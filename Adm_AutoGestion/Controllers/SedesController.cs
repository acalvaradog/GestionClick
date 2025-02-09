using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Adm_AutoGestion.Models;

namespace Adm_AutoGestion.Controllers
{
    public class SedesController : Controller
    {
        private AutogestionContext db = new AutogestionContext();

        // GET: Sedes
        public async Task<ActionResult> Index()
        {
            var sede = db.Sede.Include(s => s.Empresa);
            return View(await sede.Include(x=> x.Empresa).ToListAsync());
        }

        // GET: Sedes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sede sede = await db.Sede.FindAsync(id);
            if (sede == null)
            {
                return HttpNotFound();
            }
            return View(sede);
        }

        // GET: Sedes/Create
        public ActionResult Create()
        {
            ViewBag.SociedadId = new SelectList(db.Sociedad, "Id", "Descripcion");
            return View();
        }

        // POST: Sedes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nombre,Estado,SociedadId")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                db.Sede.Add(sede);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.SociedadId = new SelectList(db.Sociedad, "Id", "Descripcion", sede.SociedadId);
            return View(sede);
        }

        // GET: Sedes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sede sede = await db.Sede.FindAsync(id);
            if (sede == null)
            {
                return HttpNotFound();
            }
            ViewBag.SociedadId = new SelectList(db.Sociedad, "Id", "Descripcion", sede.SociedadId);
            return View(sede);
        }

        // POST: Sedes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nombre,Estado,SociedadId")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sede).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.SociedadId = new SelectList(db.Sociedad, "Id", "Descripcion", sede.SociedadId);
            return View(sede);
        }

        // GET: Sedes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sede sede = await db.Sede.FindAsync(id);
            if (sede == null)
            {
                return HttpNotFound();
            }
            return View(sede);
        }

        // POST: Sedes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Sede sede = await db.Sede.FindAsync(id);
            db.Sede.Remove(sede);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
