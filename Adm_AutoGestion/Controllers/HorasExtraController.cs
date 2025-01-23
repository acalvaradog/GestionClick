using Adm_AutoGestion.Models;
using Adm_AutoGestion.Services;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace Adm_AutoGestion.Controllers
{
    public class HorasExtraController : Controller
    {
        private AutogestionContext db = new AutogestionContext();
        private HorasExtraRepository _repo;
        public HorasExtraController()
        {
            _repo = new HorasExtraRepository();
        }


        public class InformeTotalHE
        {
            public int Id { get; set; }
            public int EmpleadoId { get; set; }
            public Empleado Empleado { get; set; }
            public HorasExtra HorasExtra { get; set; }
            public DateTime FechaHora { get; set; }
            public string HoraDesde { get; set; }
            public string HoraHasta { get; set; }
            public string ObservacionesMotivo { get; set; }
            public float? LiquidacionDiurna { get; set; }
            public float? LiquidacionNocturna { get; set; }
            public float? LiquidacionDiurnaFestivo { get; set; }
            public float? LiquidacionNocturnaFestivo { get; set; }
            public MotivoTrabajoHE MotivoTrabajoHE { get; set; }
            public float? TotalHoras { get; set; }
            public string MotivoNombre { get; set; }
            public DateTime FechadeRegistro { get; set; }
            //public float? TotalLiquidacionDiurna { get; set; }
            //public float? TotalLiquidacionNocturna { get; set; }
            //public float? TotalLiquidacionDiurnaFestivo { get; set; }
            //public float? TotalLiquidacionNocturnaFestivo { get; set; }
            public int Estado { get; set; }
            public DateTime? FechaPago { get; set; }

            public EstadosHorasExtra EstadosHorasExtra { get; set; }


        }



        [HttpPost]
        public ActionResult Create(HorasExtra model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    _repo.Crear(model);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                //Log del error
            }
            return View(model);
        }

        public ActionResult DescargarPDF(int Id)
        {
            Document doc = new Document(PageSize.LETTER);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            //FileStream file = new FileStream("archivo.pdf", FileMode.Create,FileAccess.ReadWrite);
            //PdfWriter writer = PdfWriter.GetInstance(doc, file);
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            doc.AddAuthor("Autogestión");
            doc.AddTitle("Archivo");
            doc.Open();

            BaseFont _titulo = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font titulo = new iTextSharp.text.Font(_titulo, 14f, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));

            BaseFont _subtitulo = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font subtitulo = new iTextSharp.text.Font(_subtitulo, 10f, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));

            BaseFont _parrafo = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(_parrafo, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            iTextSharp.text.Font negrita = new iTextSharp.text.Font(_subtitulo, 8f, iTextSharp.text.Font.BOLD, new BaseColor(0, 0, 0));

            BaseFont _parrafo2 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(_parrafo, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            doc.Add(Chunk.NEWLINE);



            //Chunk linea  = new Chunk( new iTextSharp.text.pdf.draw.LineSeparator(2f,100f,BaseColor.BLUE,Element.ALIGN_CENTER,0f));
            //doc.Add(linea);

            //iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance("~/Contents/assets/images/Logo.png");
            // logo.ScaleAbsoluteWidth(150);
            var tbl = new PdfPTable(new float[] { 50f, 50f }) { WidthPercentage = 100 };
            tbl.AddCell(new PdfPCell(new Phrase("Radiologos Especializados S.A.", titulo)) { Border = 0, Rowspan = 2, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase("REPORTE HORAS EXTRAS", titulo)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
            doc.Add(tbl);

            doc.Add(new Phrase(" "));


            doc.Add(Chunk.NEWLINE);

            Chunk barra = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(5f, 30f, new BaseColor(0, 69, 161), Element.ALIGN_LEFT, -1));
            doc.Add(barra);

            doc.Add(new Phrase(" "));

            var HorasExtra = db.HorasExtra
                             .Include(x => x.Empleado)
                             .FirstOrDefault(x => x.Id == Id);

            DateTime fechaNacimiento = Convert.ToDateTime(HorasExtra.Empleado.FechaNacimiento);

            DateTime now = DateTime.Today;
            int edad = DateTime.Today.Year - fechaNacimiento.Year;

            if (DateTime.Today < fechaNacimiento.AddYears(edad))
                --edad;
            string genero;

            if (HorasExtra.Empleado.Genero == "1")
            {
                genero = "MASCULINO";
            }
            else
            {

                genero = "FEMENINO";
            }

            tbl = new PdfPTable(new float[] { 20f, 25f, 10f, 20f, 10f, 15f }) { WidthPercentage = 100 };
            tbl.AddCell(new PdfPCell(new Phrase("NOMBRE EMPLEADO:", negrita)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(HorasExtra.Empleado.Nombres, parrafo)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase("CARGO:", negrita)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(HorasExtra.Empleado.Cargo, parrafo)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase("EDAD:", negrita)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(edad.ToString()+ " anos", parrafo)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            doc.Add(tbl);
            tbl = new PdfPTable(new float[] { 20f, 25f, 10f, 20f, 10f, 15f }) { WidthPercentage = 100 };
            tbl.AddCell(new PdfPCell(new Phrase("CODIGO EMPLEADO:", negrita)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(HorasExtra.Empleado.NroEmpleado, parrafo)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase("GENERO:", negrita)) { Border = 0, VerticalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(genero, parrafo)) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            tbl.AddCell(new PdfPCell(new Phrase(" ")) { Border = 0, HorizontalAlignment = Element.ALIGN_MIDDLE });
            doc.Add(tbl);
            doc.Add(new Phrase(" "));
            doc.Add(new Phrase(" "));
            tbl = new PdfPTable(new float[] { 12f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f }) { WidthPercentage = 100 };
            var c1 = new PdfPCell(new Phrase("FECHA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c2 = new PdfPCell(new Phrase("HORA DESDE", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c3 = new PdfPCell(new Phrase("HORA HASTA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c9 = new PdfPCell(new Phrase("MOTIVO", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c4 = new PdfPCell(new Phrase("DIURNA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c5 = new PdfPCell(new Phrase("NOCTURNA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c6 = new PdfPCell(new Phrase("DIURNA FESTIVA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c7 = new PdfPCell(new Phrase("NOCTURNA FESTIVA", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };
            var c8 = new PdfPCell(new Phrase("TOTAL", negrita)) { Border = 0, BorderWidthBottom = 2f, BorderColor = new BaseColor(0, 69, 161), Padding =4f };

            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c9);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);
            tbl.AddCell(c8);



            c1.Border = 0; c2.Border = 0; c3.Border = 0; c4.Border = 0; c5.Border = 0; c6.Border = 0; c7.Border = 0; c8.Border = 0; c9.Border = 0;

            var detalleHorasExtra = _repo.ObtenerDetallesHorasExtra(Id);

            foreach (var deta in detalleHorasExtra)
            {
                c1.Phrase = new Phrase(string.Format("{0:yyyy-MM-dd}", deta.Fecha), parrafo2);
                c2.Phrase = new Phrase(deta.HoraDesde.ToString(), parrafo2);
                c3.Phrase = new Phrase(deta.HoraHasta.ToString(), parrafo2);
                if (deta.ObservacionesMotivo is null)
                {
                    c9.Phrase = (new Phrase(""));
                }

                else
                {
                    c9.Phrase = new Phrase(deta.ObservacionesMotivo.ToString(), parrafo2);
                }
                c4.Phrase = new Phrase(deta.LiquidacionDiurna.ToString(), parrafo2);
                c5.Phrase = new Phrase(deta.LiquidacionNocturna.ToString(), parrafo2);
                c6.Phrase = new Phrase(deta.LiquidacionDiurnaFestivo.ToString(), parrafo2);
                c7.Phrase = new Phrase(deta.LiquidacionNocturnaFestivo.ToString(), parrafo2);
                c8.Phrase = new Phrase(deta.TotalHoras.ToString(), parrafo2);


                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c9);
                tbl.AddCell(c4);
                tbl.AddCell(c5);
                tbl.AddCell(c6);
                tbl.AddCell(c7);
                tbl.AddCell(c8);

            }

            c1.Colspan = 9;
            c1.Phrase = new Phrase("Total Horas Extras "+detalleHorasExtra.Sum(x => x.TotalHoras).ToString(), parrafo);
            c1.HorizontalAlignment =Element.ALIGN_RIGHT;
            tbl.AddCell(c1);

            doc.Add(tbl);
            doc.Add(new Phrase(" "));

            tbl = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };

            string imagenbase64 = HorasExtra.Firma.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(imagenbase64);

            Image pdfImage = Image.GetInstance(imageBytes);

            float ancho = pdfImage.Width;
            float alto = pdfImage.Height;
            float proporcion = ancho/ alto;

            pdfImage.ScaleAbsoluteWidth(150);
            pdfImage.ScaleAbsoluteHeight(150);

            tbl.AddCell(new PdfPCell(pdfImage));
            //  tbl.AddCell(new PdfPCell(new Phrase("Firma", subtitulo)) { Border = 0, VerticalAlignment = Element.ALIGN_CENTER });
            doc.Add(tbl);

            tbl = new PdfPTable(new float[] { 100f }) { WidthPercentage = 100 };
            tbl.AddCell(new PdfPCell(new Phrase("Firma", subtitulo)) { Border = 0, VerticalAlignment = Element.ALIGN_CENTER });
            doc.Add(tbl);
            // writer.Close();
            doc.Close();
            //file.Dispose();
            ////var pdf = new FileStream("archivo.pdf", FileMode.Open, FileAccess.Read);
            //ms.Seek(0, SeekOrigin.Begin);
            return File(ms.ToArray(), "application/pdf");




        }

        public ActionResult JefeDirectoHorasExtra(string JefeID, string TrabajadorS, string UnidadOrg, string Empresa, string Estado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HorasExtraJefeArea"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();
            using (var db = new AutogestionContext())
            {

                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;
                string NmrRegistro = "";

                Empleado empleado = new Empleado();

                var opcion = "JefeDirectoHorasExtra";
                var Empleadolog = Convert.ToString(Session["Empleado"]);


                int JefeID2 = 0;
                if (JefeID == null || JefeID == "")
                {
                    JefeID2 = Convert.ToInt32(Empleadolog);
                }

                var NroEmpLog = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);

                var Lider = db.Empleados.Where(e => e.Lider.Trim() == NroEmpLog.NroEmpleado.Trim() && e.Activo.Trim() == "SI").OrderBy(x => x.Nombres).ToList();
                var Jefe2 = db.Empleados.Where(e => e.Jefe.Trim() == NroEmpLog.NroEmpleado.Trim() && e.Activo.Trim()  == "SI").OrderBy(x => x.Nombres).ToList();

                if (Lider.Count != 0)
                {

                    empleado = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                    ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empleado.Empresa && f.Lider == empleado.NroEmpleado).OrderBy(x => x.Nombres).ToList();
                    var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == empleado.Empresa && f.Lider== empleado.NroEmpleado).GroupBy(b => b.AreaDescripcion).ToList();
                    ViewBag.AreaDescripcion = new List<SelectListItem>();
                    foreach (var x in areaDescripcionGroups)
                    {

                        Empleado Item = x.FirstOrDefault();
                        if (Item.UnidadOrganizativa != "00003103" || Item.UnidadOrganizativa != "00003105" || Item.UnidadOrganizativa != "00003109" ||  Item.UnidadOrganizativa != "00003110" ||  Item.UnidadOrganizativa != "00003117" || Item.UnidadOrganizativa != "00003625" || Item.UnidadOrganizativa != "00003700")
                        {

                            ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion });
                        }

                        // ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                    }


                }

                if (Jefe2.Count != 0)
                {
                    empleado = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                    ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empleado.Empresa && f.Jefe == empleado.NroEmpleado).OrderBy(x => x.Nombres).ToList();
                    var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == empleado.Empresa && f.Jefe == empleado.NroEmpleado).GroupBy(b => b.AreaDescripcion).ToList();
                    ViewBag.AreaDescripcion = new List<SelectListItem>();
                    foreach (var x in areaDescripcionGroups)
                    {
                        Empleado Item = x.FirstOrDefault();
                        if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                        {
                            ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion });
                            // ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                        }
                    }

                }
                //ViewBag.Sociedad = db.Sociedad.ToList();

                ViewBag.Empresa =  db.Sociedad.Where(x => x.Codigo== empleado.Empresa).ToList();

                ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.Where(x => x.Id == 1 || x.Id == 5).ToList();





                List<SelectListItem> lst = new List<SelectListItem>();

                if (Lider.Count != 0)
                {

                    ListadoFinal = db.HorasExtra
                             .Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado })
                             .Where(x => (x.Encabezado.Estado == 1|| x.Encabezado.Estado == 5)  && x.Encabezado.Empleado.Empresa == empleado.Empresa && x.Encabezado.Empleado.Lider == empleado.NroEmpleado
                             && (string.IsNullOrEmpty(TrabajadorS) || SqlFunctions.StringConvert((decimal)x.Encabezado.EmpleadoId).Contains(TrabajadorS)
                             && (string.IsNullOrEmpty(UnidadOrg) ||  x.Encabezado.Empleado.UnidadOrganizativa.Contains(UnidadOrg))))


                             .Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago,
                                        ObservacionesMotivo = x.Detallado.ObservacionesMotivo

                                    }).ToList();
                }


                if (Jefe2.Count != 0)
                {

                    ListadoFinal = db.HorasExtra
                             .Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado })
                             .Where(x => (x.Encabezado.Estado == 1|| x.Encabezado.Estado == 5)  && x.Encabezado.Empleado.Empresa == empleado.Empresa && x.Encabezado.Empleado.Jefe == empleado.NroEmpleado
                             && (string.IsNullOrEmpty(TrabajadorS) || SqlFunctions.StringConvert((decimal)x.Encabezado.EmpleadoId).Contains(TrabajadorS)
                             && (string.IsNullOrEmpty(UnidadOrg) ||  x.Encabezado.Empleado.UnidadOrganizativa.Contains(UnidadOrg))))


                             .Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago,
                                        ObservacionesMotivo = x.Detallado.ObservacionesMotivo

                                    }).ToList();
                }

                ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ToList();

                InformeTotalHE NewRTotal = new InformeTotalHE();



                foreach (InformeTotalHE Item in ListadoFinal.Reverse<InformeTotalHE>())
                {

                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);

                }





            }
            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ThenBy(x => x.FechaHora).ThenByDescending(x => x.Id).ToList();


            if (UnidadOrg != null && UnidadOrg != "")
            {
                List<InformeTotalHE> nuevaLista = new List<InformeTotalHE>();
                foreach (var item in ListadoFinal.Reverse<InformeTotalHE>())
                {
                    bool cumpleCriterio = true;
                    if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
                    {
                        cumpleCriterio = false;
                    }



                    if (cumpleCriterio)
                    {
                        nuevaLista.Add(item);
                    }
                }
                ListadoFinal = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();



            }


            return View(ListadoFinal);



        }

        public ActionResult EnviarAprobadosJefe(int[] ids, string fechap)
        {

            bool mensaje = true;
            int IdUsuarioM = 0;
            string modifica = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(modifica, out IdUsuarioM);
            var url = Url.Action("JefeDirectoHorasExtra");
            string message = null;
            var respuesta = "";
            var opcion = "JEFE";

            var idlider = db.Empleados.FirstOrDefault(x => x.Id == IdUsuarioM);
            var listlider = db.Empleados.Where(x => x.Lider == idlider.NroEmpleado).ToList();

            if (listlider.Count > 0)
            {

                opcion = "LIDER";

            }


            try
            {

                if (ids != null && ids.Length> 0)
                {



                    mensaje = _repo.EnviarAprobados(ids, IdUsuarioM, fechap, opcion);

                    foreach (int id2 in ids)
                    {
                        if (opcion == "JEFE")
                        {
                            var Emp = db.HorasExtra.Include(x => x.Empleado).Where(x => x.Id == id2).FirstOrDefault();
                            HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == id2).FirstOrDefault();

                            var Correo = db.Configuraciones.Where(x => x.Parametro == "TXTHECORREOGH").FirstOrDefault();
                            _repo.notificar_Solicitud(id2, Correo.Valor, Emp.EmpleadoId, horasExtra.FechaDeRegistro, "AprobadoJefe");

                        }
                    }

                    if (mensaje == true)
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Guardado Exitoso"
                        });




                    }
                    else
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Error al guardar Registro"
                        });
                    }

                }

                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }

        }


        public ActionResult EnviarRechazadosJefe(int[] ids, string observaciones)
        {

            bool mensaje = true;
            int IdUsuarioM = 0;
            string modifica = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(modifica, out IdUsuarioM);
            var url = Url.Action("JefeDirectoHorasExtra");
            string message = null;
            var respuesta = "";
            try
            {

                if (ids != null && ids.Length> 0)
                {



                    mensaje = _repo.EnviarRechazados(ids, IdUsuarioM, observaciones);
                    foreach (int id2 in ids)
                    {

                        var Emp = db.HorasExtra.Include(x => x.Empleado).Where(x => x.Id == id2).FirstOrDefault();
                        HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == id2).FirstOrDefault();


                        if (Emp.Empleado.Lider!= null)
                        {


                            var CorreoLider = db.Empleados.FirstOrDefault(x => x.NroEmpleado == Emp.Empleado.Lider);
                            _repo.notificar_Rechazado(id2, CorreoLider.Correo, Emp.EmpleadoId, horasExtra.FechaDeRegistro, observaciones, "Rechazado");

                        }

                        //CORREO JEFE

                        var CorreoJefe = db.Empleados.FirstOrDefault(x => x.NroEmpleado == Emp.Empleado.Jefe);
                        _repo.notificar_Rechazado(id2, CorreoJefe.Correo, Emp.EmpleadoId, horasExtra.FechaDeRegistro, observaciones, "Rechazado");

                        _repo.notificar_Rechazado(id2, Emp.Empleado.Correo, Emp.EmpleadoId, horasExtra.FechaDeRegistro, observaciones, "Rechazado");
                    }

                    if (mensaje == true)
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Guardado Exitoso"
                        });




                    }
                    else
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Error al guardar Registro"
                        });
                    }

                }

                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }

        }

        public ActionResult EnviarRechazadosGH(int[] ids, string observaciones)
        {

            bool mensaje = true;
            int IdUsuarioM = 0;
            string modifica = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(modifica, out IdUsuarioM);
            var url = Url.Action("GestionHumana");
            string message = null;
            var respuesta = "";
            try
            {

                if (ids != null && ids.Length> 0)
                {



                    mensaje = _repo.EnviarRechazados(ids, IdUsuarioM, observaciones);

                    foreach (int id2 in ids)
                    {

                        var Emp = db.HorasExtra.Include(x => x.Empleado).Where(x => x.Id == id2).FirstOrDefault();
                        HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == id2).FirstOrDefault();




                        _repo.notificar_Rechazado(id2, Emp.Empleado.Correo, Emp.EmpleadoId, horasExtra.FechaDeRegistro, observaciones, "Rechazado");
                    }

                    if (mensaje == true)
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Guardado Exitoso"
                        });




                    }
                    else
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Error al guardar Registro"
                        });
                    }

                }

                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }

        }


        public ActionResult EnviarAprobadosGH(int[] ids, string fechap)
        {

            bool mensaje = true;
            int IdUsuarioM = 0;
            string modifica = String.Format("{0}", Session["Empleado"]);
            Int32.TryParse(modifica, out IdUsuarioM);
            var url = Url.Action("GestionHumana");
            string message = null;
            var respuesta = "";
            var opcion = "GH";

            try
            {

                if (ids != null && ids.Length> 0)
                {



                    mensaje = _repo.EnviarAprobados(ids, IdUsuarioM, fechap, opcion);

                    foreach (int id2 in ids)
                    {

                        var Emp = db.HorasExtra.Include(x => x.Empleado).Where(x => x.Id == id2).FirstOrDefault();
                        Empleado Jefe = db.Empleados.Where(x => x.NroEmpleado == Emp.Empleado.Jefe).FirstOrDefault();
                        HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == id2).FirstOrDefault();
                        _repo.notificar_Cierre(id2, Emp.Empleado.Correo, Jefe.Nombres, Emp.EmpleadoId, horasExtra.FechaDeRegistro, "Cierre");

                    }

                    if (mensaje == true)
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Guardado Exitoso"
                        });




                    }
                    else
                    {

                        return Json(new
                        {
                            redirectUrl = url,
                            isRedirect = true,
                            respuesta = "Error al guardar Registro"
                        });
                    }

                }

                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });

            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }

        }


        public ActionResult GestionHumana(string TrabajadorS, string UnidadOrg, string Empresa)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("login", "login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HorasExtraGestionHumana"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");


            }

            int persona = 0;
            var Empleadolog = Convert.ToString(Session["Empleado"]);
            persona = Convert.ToInt32(Empleadolog);


            // var opcion = "GestionHumana";
            var Personal = db.Empleados.FirstOrDefault(e => e.Id == persona);
            ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo== Personal.Empresa).ToList();
            ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"   && f.Empresa ==Personal.Empresa).OrderBy(x => x.Nombres).ToList();
            ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.ToList();
            var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == Personal.Empresa).GroupBy(b => b.AreaDescripcion).ToList();
            ViewBag.AreaDescripcion = new List<SelectListItem>();
            foreach (var x in areaDescripcionGroups)
            {
                Empleado Item = x.FirstOrDefault();
                if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                {
                    ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion + " - " + Item.Empresa });
                    //ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });
                }
            }

            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();
            using (var db = new AutogestionContext())
            {

                ListadoFinal = db.HorasExtra
                            .Join(db.DetalleHorasExtras,
                               Encabezado => Encabezado.Id,
                               Detallado => Detallado.HorasExtraId,
                               (Encabezado, Detallado) => new { Encabezado, Detallado })
                            .Where(x => x.Encabezado.Estado == 2 && x.Encabezado.Empleado.Empresa == Personal.Empresa
                             && (string.IsNullOrEmpty(TrabajadorS) || SqlFunctions.StringConvert((decimal)x.Encabezado.EmpleadoId).Contains(TrabajadorS)

                            ))
                            .Select(x =>
                                   new InformeTotalHE
                                   {
                                       Id = x.Encabezado.Id,
                                       FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                       FechaHora = x.Detallado.Fecha,
                                       EmpleadoId = x.Encabezado.EmpleadoId,
                                       HoraDesde = x.Detallado.HoraDesde,
                                       HoraHasta = x.Detallado.HoraHasta,
                                       LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                       LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                       LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                       LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                       TotalHoras = x.Detallado.TotalHoras,
                                       Estado = x.Encabezado.Estado,
                                       ObservacionesMotivo = x.Detallado.ObservacionesMotivo

                                   }).ToList();
            }

            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ToList();
            //int NumeroGuia = 0;
            InformeTotalHE NewRTotal = new InformeTotalHE();



            foreach (InformeTotalHE Item in ListadoFinal.Reverse<InformeTotalHE>())
            {
                //int TotalRegistros = ListadoFinal.Where(e => e.EmpleadoId == Item.EmpleadoId).Count();
                Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                // var model = _repo.ObtenerHorasExtraAprobadasJefe(opcion, Empleadolog);


            }




            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ThenByDescending(x => x.Id).ToList();


            if (UnidadOrg != null && UnidadOrg != "")
            {
                List<InformeTotalHE> nuevaLista = new List<InformeTotalHE>();
                foreach (var item in ListadoFinal.Reverse<InformeTotalHE>())
                {
                    bool cumpleCriterio = true;
                    if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
                    {
                        cumpleCriterio = false;
                    }



                    if (cumpleCriterio)
                    {
                        nuevaLista.Add(item);
                    }
                }
                ListadoFinal = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();



            }

            return View(ListadoFinal);

        }



        public ActionResult DetalleHorasExtra(int Id)
        {
            using (var db = new AutogestionContext())
            {
                HorasExtra Items = new HorasExtra();
                var detalleHorasExtra = _repo.ObtenerDetallesHorasExtra(Id);
                ViewBag.DetalleHorasExtra = detalleHorasExtra;
                Items = db.HorasExtra.FirstOrDefault(e => e.Id == Id);

                // Pasar el Id a la vista
                ViewBag.HorasExtraId = Id;
                ViewBag.HorasExtra = Items;

                return PartialView("DetalleHorasExtra");
            }
        }
        public ActionResult DetalleHorasExtraGestion(int Id)
        {
            using (var db = new AutogestionContext())
            {
                HorasExtra Items = new HorasExtra();
                var detalleHorasExtra = _repo.ObtenerDetallesHorasExtra(Id);
                ViewBag.DetalleHorasExtra = detalleHorasExtra;
                Items = db.HorasExtra.FirstOrDefault(e => e.Id == Id);

                // Pasar el Id a la vista
                ViewBag.HorasExtraId = Id;
                ViewBag.HorasExtra = Items;
                return PartialView("DetalleHorasExtraGestion");
            }
        }
        public ActionResult HistorialHE(string Id)
        {
            using (var db = new AutogestionContext())
            {

                var HistorialHE = _repo.ObtenerTodos3(Id);
                ViewBag.HistorialHE = HistorialHE;


                // Pasar el Id a la vista
                ViewBag.HorasExtraId = Id;

                return PartialView("HistorialHE");
            }
        }
        public ActionResult DetalleHorasExtra2(int Id)
        {
            using (var db = new AutogestionContext())
            {

                var detalleHorasExtra = _repo.ObtenerDetallesHorasExtra(Id);
                ViewBag.DetalleHorasExtra = detalleHorasExtra;



                return PartialView("DetalleHorasExtraView");
            }
        }
        public ActionResult DetalleHorasExtraGestion2(int Id)
        {
            using (var db = new AutogestionContext())
            {
                HorasExtra Items = new HorasExtra();
                var detalleHorasExtra = _repo.ObtenerDetallesHorasExtra(Id);
                ViewBag.DetalleHorasExtra = detalleHorasExtra;
                Items = db.HorasExtra.FirstOrDefault(e => e.Id == Id);

                // Pasar el Id a la vista
                ViewBag.HorasExtraId = Id;
                ViewBag.HorasExtra = Items;


                return PartialView("DetalleHorasExtraGHView");
            }
        }



        public JsonResult RespuestajsonJefe()
        {

            int HorasExtraId = 0;
            int IdUsuarioM = 0;
            var IdSolicitudParam = HttpContext.Request.Params["HorasExtraId"];
            var Estado = HttpContext.Request.Params["Estado"];
            var Observaciones = HttpContext.Request.Params["Observaciones"];
            var FechaPago = HttpContext.Request.Params["FechaPago"];

            DetalleHorasExtra DetalleHorasExtra = new DetalleHorasExtra();
            DetalleHorasExtra = db.DetalleHorasExtras.FirstOrDefault(o => o.HorasExtraId == HorasExtraId);
            HorasExtra HorasExtra = new HorasExtra();

            var idUserlog = Convert.ToInt32(Session["Empleado"]);
            Empleado userlog = db.Empleados.Where(d => d.Id == idUserlog).FirstOrDefault();
            var Fechaactual = DateTime.Now;
            if (!int.TryParse(IdSolicitudParam, out HorasExtraId) || HorasExtraId == 0)
            {
                // Manejo de error: el valor no es un entero válido o es 0
                return Json(new
                {
                    respuesta = "Error: El valor de HorasExtraId no es válido."
                });
            }
            HorasExtra = db.HorasExtra.FirstOrDefault(o => o.Id == HorasExtraId);
            var respuesta = "";
            string message = null;
            try
            {
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                var url = Url.Action("JefeDirectoHorasExtra");
                using (var db = new AutogestionContext())
                {

                    if (HorasExtraId != 0)
                    {
                        if (Estado != "Seleccionado...")
                        {
                            if (Estado == "Aprobar")
                            {
                                Estado = "2";
                            }
                            if (Estado == "Denegar")
                            {
                                Estado = "4";
                            }

                            var a = _repo.Modificar(HorasExtraId, Estado, Observaciones, IdUsuarioM, FechaPago);
                            if (a == true)
                            {
                                var Opcion = "JefeDirectoHorasExtra";
                                HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == HorasExtraId).FirstOrDefault();

                                //h
                                Empleado Emp = db.Empleados.Where(x => x.Id == horasExtra.EmpleadoId).FirstOrDefault();

                                if (Estado == "2")
                                {
                                    var Correo = db.Configuraciones.Where(x => x.Parametro == "TXTHECORREOGH").FirstOrDefault();
                                    _repo.notificar_Solicitud(HorasExtraId, Correo.Valor, Emp.Id, HorasExtra.FechaDeRegistro, "AprobadoJefe");
                                    //_repo.NotificarARL(viaticos.Id, viaticos.EmpleadoId, viaticos.FechaInicio, viaticos.FechaFin);
                                    //if (viaticos.Hospedaje == true)
                                    //{ _repo.NotificarBienestar(viaticos.Id, viaticos.EmpleadoId, viaticos.FechaInicio, viaticos.FechaFin); }


                                }

                                if (Estado == "4")
                                {
                                    HistoricoHorasExtra Historico = db.HistoricoHorasExtra.Where(x => x.HorasExtraId == HorasExtraId).FirstOrDefault();
                                    Empleado Jefe = db.Empleados.Where(x => x.NroEmpleado == Emp.Jefe).FirstOrDefault();
                                    Empleado Lider = db.Empleados.Where(x => x.NroEmpleado == Emp.Lider).FirstOrDefault();
                                    if (Lider != null)
                                    {

                                        _repo.notificar_RechazadoGH(HorasExtraId, Emp.Correo, Lider.Correo, Emp.Id, HorasExtra.FechaDeRegistro, Historico.Observaciones, "Rechazado");

                                    }

                                    _repo.notificar_Rechazado(HorasExtraId, Emp.Correo, Emp.Id, HorasExtra.FechaDeRegistro, Historico.Observaciones, "Rechazado");
                                }
                            }
                            else
                            {

                                return Json(new
                                {
                                    redirectUrl = url,
                                    isRedirect = true,
                                    respuesta = "Guardado Exitoso"
                                });
                            }

                        }

                    }

                }
                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }
        }
        //public ActionResult HistorialHE(string Id)
        //{

        //    var model = _repo.ObtenerTodos3(Id);

        //    return View(model);
        //}
        public JsonResult RespuestajsonGestion()
        {

            int HorasExtraId = 0;
            int IdUsuarioM = 0;
            var IdSolicitudParam = HttpContext.Request.Params["HorasExtraId"];
            var Estado = HttpContext.Request.Params["Estado"];
            var Observaciones = HttpContext.Request.Params["Observaciones"];
            var FechaPago = (HttpContext.Request.Params["FechaPago"]);

            DetalleHorasExtra DetalleHorasExtra = new DetalleHorasExtra();
            DetalleHorasExtra = db.DetalleHorasExtras.FirstOrDefault(o => o.HorasExtraId == HorasExtraId);
            HorasExtra HorasExtra = new HorasExtra();

            var idUserlog = Convert.ToInt32(Session["Empleado"]);
            Empleado userlog = db.Empleados.Where(d => d.Id == idUserlog).FirstOrDefault();
            var Fechaactual = DateTime.Now;
            if (!int.TryParse(IdSolicitudParam, out HorasExtraId) || HorasExtraId == 0)
            {
                // Manejo de error: el valor no es un entero válido o es 0
                return Json(new
                {
                    respuesta = "Error: El valor de HorasExtraId no es válido."
                });
            }
            HorasExtra = db.HorasExtra.FirstOrDefault(o => o.Id == HorasExtraId);
            var respuesta = "";
            string message = null;
            try
            {
                string modifica = String.Format("{0}", Session["Empleado"]);
                Int32.TryParse(modifica, out IdUsuarioM);
                var url = Url.Action("GestionHumana");
                using (var db = new AutogestionContext())
                {

                    if (HorasExtraId != 0)
                    {
                        if (Estado != "Seleccionado...")
                        {
                            if (Estado == "Aprobar")
                            {
                                Estado = "3";
                            }
                            if (Estado == "Denegar")
                            {
                                Estado = "4";
                            }

                            var a = _repo.Modificar(HorasExtraId, Estado, Observaciones, IdUsuarioM, FechaPago);
                            if (a == true)
                            {
                                var Opcion = "GestionHumana";
                                HorasExtra horasExtra = db.HorasExtra.Where(x => x.Id == HorasExtraId).FirstOrDefault();

                                //h
                                Empleado Emp = db.Empleados.Where(x => x.Id == horasExtra.EmpleadoId).FirstOrDefault();
                                if (Estado == "4")
                                {
                                    Empleado Jefe = db.Empleados.Where(x => x.NroEmpleado == Emp.Jefe).FirstOrDefault();
                                    HistoricoHorasExtra Historico = db.HistoricoHorasExtra.Where(x => x.HorasExtraId == HorasExtraId).FirstOrDefault();
                                    Empleado Lider = db.Empleados.Where(x => x.NroEmpleado == Emp.Lider).FirstOrDefault();

                                    if (Lider != null)
                                    {

                                        _repo.notificar_RechazadoGH(HorasExtraId, Emp.Correo, Lider.Correo, Emp.Id, HorasExtra.FechaDeRegistro, Historico.Observaciones, "Rechazado");

                                    }

                                    _repo.notificar_RechazadoGH(HorasExtraId, Emp.Correo, Jefe.Correo, Emp.Id, HorasExtra.FechaDeRegistro, Historico.Observaciones, "Rechazado");
                                }
                                if (Estado == "3")
                                {
                                    Empleado Jefe = db.Empleados.Where(x => x.NroEmpleado == Emp.Jefe).FirstOrDefault();
                                    _repo.notificar_Cierre(HorasExtraId, Emp.Correo, Jefe.Nombres, Emp.Id, HorasExtra.FechaDeRegistro, "Cierre");
                                }

                            }
                            else
                            {

                                return Json(new
                                {
                                    redirectUrl = url,
                                    isRedirect = true,
                                    respuesta = "Guardado Exitoso"
                                });
                            }

                        }

                    }

                }
                return Json(new
                {
                    redirectUrl = url,
                    isRedirect = true,
                    respuesta = "Guardado Exitoso"
                });
            }
            catch (SystemException ex)
            {
                message = String.Format("Se genero un error. {0}", ex.Message);
                Session["message"] = message;
                respuesta = "Error durante el guardado";
                return Json(new { respuesta });
            }
        }
        public ActionResult InformeHorasExtra(string JefeID, string TrabajadorS, string FechaI, string FechaF, string Estado, string NmrRegistro, string UnidadOrg, string Empresa, string Documento, string NroEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);


            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HorasExtraJefeArea"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }
            Empleado empleado = new Empleado();
            var Empleadolog = Session["Empleado"];
            var EstadoId = 0;
            int JefeID2 = 0;
            if (JefeID == null || JefeID == "")
            {
                JefeID2 = Convert.ToInt32(Empleadolog);
            }
            //List<HorasExtra> Proceso = new List<HorasExtra>();
            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();

            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//


                var NroEmpLog = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);

                var Lider = db.Empleados.Where(e => e.Lider.Trim() == NroEmpLog.NroEmpleado.Trim() && e.Activo.Trim() == "SI").ToList();
                var Jefe2 = db.Empleados.Where(e => e.Jefe.Trim() == NroEmpLog.NroEmpleado.Trim() && e.Activo.Trim()  == "SI").ToList();

                if (Lider.Count != 0)
                {

                    empleado = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                    ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empleado.Empresa && f.Lider == empleado.NroEmpleado).OrderBy(x => x.Nombres).ToList();
                    var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == empleado.Empresa && f.Lider== empleado.NroEmpleado).GroupBy(b => b.AreaDescripcion).ToList();
                    ViewBag.AreaDescripcion = new List<SelectListItem>();
                    foreach (var x in areaDescripcionGroups)
                    {
                        Empleado Item = x.FirstOrDefault();
                        if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                        {
                            ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion });
                            // ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });

                        }
                    }
                }

                if (Jefe2.Count != 0)
                {
                    empleado = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                    ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO"  && f.Empresa == empleado.Empresa && f.Jefe == empleado.NroEmpleado).OrderBy(x => x.Nombres).ToList();
                    var areaDescripcionGroups = db.Empleados.Where(f => f.Activo != "NO" && f.AreaDescripcion != null && f.AreaDescripcion != "" && f.Empresa == empleado.Empresa && f.Jefe == empleado.NroEmpleado).GroupBy(b => b.AreaDescripcion).ToList();
                    ViewBag.AreaDescripcion = new List<SelectListItem>();
                    foreach (var x in areaDescripcionGroups)
                    {


                        Empleado Item = x.FirstOrDefault();
                        if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                        {
                            ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion });
                            // ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.AreaDescripcion, Text = Item.AreaDescripcion });

                        }
                    }



                }

                // ViewBag.Sociedad = db.Sociedad.ToList();

                ViewBag.Empresa =  db.Sociedad.Where(x => x.Codigo== empleado.Empresa).ToList();
                ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.ToList();


                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                if (Estado != null && Estado != "") { EstadoId = Convert.ToInt32(Estado); }
                Int32 IdProceso = new Int32();
                if (NmrRegistro != "")
                { IdProceso = Convert.ToInt32(NmrRegistro); }
                if (!DateTime.TryParse(FechaI, out Fecha1))
                { Fecha1 = new DateTime(); }
                if (!DateTime.TryParse(FechaF, out Fecha2))
                { Fecha2 = DateTime.Now; }

                if (Lider.Count != 0)
                {

                    if (TrabajadorS == "")
                    {
                        ListadoFinal = db.HorasExtra
                                 .Join(db.DetalleHorasExtras,
                                    Encabezado => Encabezado.Id,
                                    Detallado => Detallado.HorasExtraId,
                                    (Encabezado, Detallado) => new { Encabezado, Detallado }
                                 ).Where(Union => DbFunctions.TruncateTime(Union.Detallado.Fecha) >= Fecha1 &&
                                   DbFunctions.TruncateTime(Union.Detallado.Fecha) <= Fecha2
                                   &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                    &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)
                                    && Union.Encabezado.Empleado.Lider == empleado.NroEmpleado
                                 ).Select(x =>
                                        new InformeTotalHE
                                        {
                                            Id = x.Encabezado.Id,
                                            FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                            FechaHora = x.Detallado.Fecha,
                                            EmpleadoId = x.Encabezado.EmpleadoId,
                                            HoraDesde = x.Detallado.HoraDesde,
                                            HoraHasta = x.Detallado.HoraHasta,
                                            LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                            LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                            LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                            LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                            TotalHoras = x.Detallado.TotalHoras,
                                            Estado = x.Encabezado.Estado,
                                            FechaPago = x.Encabezado.FechaPago
                                        }).ToList();
                    }

                    if (TrabajadorS != "")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);


                        int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                        ListadoFinal = db.HorasExtra.Join(db.DetalleHorasExtras,
                                    Encabezado => Encabezado.Id,
                                    Detallado => Detallado.HorasExtraId,
                                    (Encabezado, Detallado) => new { Encabezado, Detallado }
                                 ).Where(Union =>
                                        Ids.Contains(Union.Encabezado.Id) && Union.Encabezado.FechaDeRegistro >= Fecha1 && Union.Encabezado.FechaDeRegistro <= Fecha2
                                         &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)&&
                                        SqlFunctions.StringConvert((decimal)Union.Encabezado.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                        ).Select(x =>
                                        new InformeTotalHE
                                        {
                                            Id = x.Encabezado.Id,
                                            FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                            FechaHora = x.Detallado.Fecha,
                                            EmpleadoId = x.Encabezado.EmpleadoId,
                                            HoraDesde = x.Detallado.HoraDesde,
                                            HoraHasta = x.Detallado.HoraHasta,
                                            LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                            LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                            LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                            LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                            TotalHoras = x.Detallado.TotalHoras,
                                            Estado = x.Encabezado.Estado,
                                            FechaPago = x.Encabezado.FechaPago
                                        }).ToList();
                    }

                }


                if (Jefe2.Count != 0)
                {

                    if (TrabajadorS == "")
                    {
                        ListadoFinal = db.HorasExtra
                                 .Join(db.DetalleHorasExtras,
                                    Encabezado => Encabezado.Id,
                                    Detallado => Detallado.HorasExtraId,
                                    (Encabezado, Detallado) => new { Encabezado, Detallado }
                                 ).Where(Union => DbFunctions.TruncateTime(Union.Encabezado.FechaDeRegistro) >= Fecha1 &&
                                   DbFunctions.TruncateTime(Union.Encabezado.FechaDeRegistro) <= Fecha2
                                   &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                    &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)
                                    && Union.Encabezado.Empleado.Jefe == empleado.NroEmpleado
                                 ).Select(x =>
                                        new InformeTotalHE
                                        {
                                            Id = x.Encabezado.Id,
                                            FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                            FechaHora = x.Detallado.Fecha,
                                            EmpleadoId = x.Encabezado.EmpleadoId,
                                            HoraDesde = x.Detallado.HoraDesde,
                                            HoraHasta = x.Detallado.HoraHasta,
                                            LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                            LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                            LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                            LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                            TotalHoras = x.Detallado.TotalHoras,
                                            Estado = x.Encabezado.Estado,
                                            FechaPago = x.Encabezado.FechaPago
                                        }).ToList();
                    }

                    if (TrabajadorS != "")
                    {
                        int id = -1;
                        Int32.TryParse(TrabajadorS, out id);


                        int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                        ListadoFinal = db.HorasExtra.Join(db.DetalleHorasExtras,
                                    Encabezado => Encabezado.Id,
                                    Detallado => Detallado.HorasExtraId,
                                    (Encabezado, Detallado) => new { Encabezado, Detallado }
                                 ).Where(Union =>
                                        Ids.Contains(Union.Encabezado.Id) && Union.Encabezado.FechaDeRegistro >= Fecha1 && Union.Encabezado.FechaDeRegistro <= Fecha2
                                         &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)&&
                                        SqlFunctions.StringConvert((decimal)Union.Encabezado.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                        ).Select(x =>
                                        new InformeTotalHE
                                        {
                                            Id = x.Encabezado.Id,
                                            FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                            FechaHora = x.Detallado.Fecha,
                                            EmpleadoId = x.Encabezado.EmpleadoId,
                                            HoraDesde = x.Detallado.HoraDesde,
                                            HoraHasta = x.Detallado.HoraHasta,
                                            LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                            LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                            LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                            LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                            TotalHoras = x.Detallado.TotalHoras,
                                            Estado = x.Encabezado.Estado,
                                            FechaPago = x.Encabezado.FechaPago
                                        }).ToList();
                    }

                }

                ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ToList();
                //int NumeroGuia = 0;
                InformeTotalHE NewRTotal = new InformeTotalHE();



                foreach (InformeTotalHE Item in ListadoFinal.Reverse<InformeTotalHE>())
                {
                    int TotalRegistros = ListadoFinal.Where(e => e.EmpleadoId == Item.EmpleadoId).Count();
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                    NewRTotal.Empleado = Item.Empleado;
                    NewRTotal.EstadosHorasExtra = Item.EstadosHorasExtra;

                    //NumeroGuia++;

                    //if (NewRTotal.TotalHoras == null || NewRTotal.LiquidacionDiurna == null ||
                    //    NewRTotal.LiquidacionDiurnaFestivo == null || NewRTotal.LiquidacionNocturna == null ||
                    //    NewRTotal.LiquidacionNocturnaFestivo == null)
                    //{
                    //    //INICIALIZA CAMPOS PARA PODER HACER OPERACIONES
                    //    NewRTotal.TotalHoras = 0;
                    //    NewRTotal.LiquidacionDiurna = 0;
                    //    NewRTotal.LiquidacionDiurnaFestivo = 0;
                    //    NewRTotal.LiquidacionNocturna = 0;
                    //    NewRTotal.LiquidacionNocturnaFestivo = 0;
                    //}
                    //NewRTotal.EmpleadoId = Item.EmpleadoId;
                    //NewRTotal.Empleado = Item.Empleado;
                    //NewRTotal.Estado = Item.Estado;
                    //NewRTotal.LiquidacionDiurna = NewRTotal.LiquidacionDiurna + Item.LiquidacionDiurna;
                    //NewRTotal.LiquidacionDiurnaFestivo = NewRTotal.LiquidacionDiurnaFestivo + Item.LiquidacionDiurnaFestivo;
                    //NewRTotal.LiquidacionNocturna = NewRTotal.LiquidacionNocturna + Item.LiquidacionNocturna;
                    //NewRTotal.LiquidacionNocturnaFestivo = NewRTotal.LiquidacionNocturnaFestivo + Item.LiquidacionNocturnaFestivo;
                    //NewRTotal.TotalHoras = NewRTotal.TotalHoras + Item.TotalHoras;
                    //if (NumeroGuia == TotalRegistros)
                    //{
                    //    NewRTotal.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                    //    NumeroGuia = 0;
                    //    ListadoFinal.Add(NewRTotal);
                    //    NewRTotal = new InformeTotalHE();
                    //}
                }
                if (UnidadOrg != null && UnidadOrg != "" || (Empresa != null && Empresa != "") || (Documento != null && Documento != "") || (NroEmpleado != null && NroEmpleado != ""))
                {
                    List<InformeTotalHE> nuevaLista = new List<InformeTotalHE>();
                    foreach (var item in ListadoFinal.Reverse<InformeTotalHE>())
                    {
                        bool cumpleCriterio = true;
                        if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Documento != Documento && !string.IsNullOrEmpty(Documento))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.NroEmpleado != NroEmpleado && !string.IsNullOrEmpty(NroEmpleado))
                        {
                            cumpleCriterio = false;
                        }


                        if (cumpleCriterio)
                        {
                            nuevaLista.Add(item);
                        }
                    }
                    ListadoFinal = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();

                }
                if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
                {
                    return View(new List<InformeTotalHE>());
                }

            }
            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ThenBy(x => x.FechaHora).ThenByDescending(x => x.Id).ToList();
            return View(ListadoFinal);
        }



        public ActionResult InformeHorasExtraGH(string JefeID, string TrabajadorS, string FechaI, string FechaF, string Estado, string NmrRegistro, string UnidadOrg, string Empresa, string Documento, string NroEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            Empleado empleado = new Empleado();

            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HorasExtraGestionHumanaInforme"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            var Empleadolog = Session["Empleado"];
            var EstadoId = 0;
            int JefeID2 = 0;
            if (JefeID == null || JefeID == "")
            {
                JefeID2 = Convert.ToInt32(Empleadolog);
            }
            List<HorasExtra> Proceso = new List<HorasExtra>();
            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();


            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                //ViewBag.Sociedad = db.Sociedad.ToList();
                empleado = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Sociedad =  db.Sociedad.Where(x => x.Codigo== empleado.Empresa).ToList();
                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Empresa ==Jefe.Empresa).OrderBy(x => x.Nombres).ToList();
                ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.ToList();
                var areaDescripcionGroups = db.Empleados.Where(x => x.Activo == "SI" && (x.AreaDescripcion != null && x.AreaDescripcion != "") && x.Empresa ==Jefe.Empresa).GroupBy(b => b.UnidadOrganizativa).ToList();
                ViewBag.AreaDescripcion = new List<SelectListItem>();
                foreach (var x in areaDescripcionGroups)
                {
                    Empleado Item = x.FirstOrDefault();
                    if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                    {
                        ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = ""+ Item.AreaDescripcion +" - "+ Item.Empresa });
                    }
                }
                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                if (Estado != null && Estado != "") { EstadoId = Convert.ToInt32(Estado); }
                Int32 IdProceso = new Int32();
                if (NmrRegistro != "")
                { IdProceso = Convert.ToInt32(NmrRegistro); }
                if (!DateTime.TryParse(FechaI, out Fecha1))
                { Fecha1 = new DateTime(); }
                if (!DateTime.TryParse(FechaF, out Fecha2))
                { Fecha2 = DateTime.Now; }
                if (TrabajadorS == "")
                {
                    ListadoFinal = db.HorasExtra
                             .Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado }
                             ).Where(Union => DbFunctions.TruncateTime(Union.Detallado.Fecha) >= Fecha1 &&
                               DbFunctions.TruncateTime(Union.Detallado.Fecha) <= Fecha2
                               &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                               &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)
                               && Union.Encabezado.Empleado.Empresa == empleado.Empresa
                             ).Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago
                                    }).ToList();
                }

                if (TrabajadorS != "")
                {
                    int id = -1;
                    Int32.TryParse(TrabajadorS, out id);


                    int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                    ListadoFinal = db.HorasExtra.Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado }
                             ).Where(Union =>
                                    Ids.Contains(Union.Encabezado.Id) && Union.Detallado.Fecha >= Fecha1 && Union.Detallado.Fecha <= Fecha2
                                     &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Estado).Contains(Estado)&&
                                    SqlFunctions.StringConvert((decimal)Union.Encabezado.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                    && Union.Encabezado.Empleado.Empresa == empleado.Empresa
                                    ).Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago
                                    }).ToList();
                }

                ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ToList();
                //int NumeroGuia = 0;
                InformeTotalHE NewRTotal = new InformeTotalHE();



                foreach (InformeTotalHE Item in ListadoFinal.Reverse<InformeTotalHE>())
                {
                    int TotalRegistros = ListadoFinal.Where(e => e.EmpleadoId == Item.EmpleadoId).Count();
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                    NewRTotal.Empleado = Item.Empleado;
                    NewRTotal.EstadosHorasExtra = Item.EstadosHorasExtra;

                    //NumeroGuia++;

                    //if (NewRTotal.TotalHoras == null || NewRTotal.LiquidacionDiurna == null ||
                    //    NewRTotal.LiquidacionDiurnaFestivo == null || NewRTotal.LiquidacionNocturna == null ||
                    //    NewRTotal.LiquidacionNocturnaFestivo == null)
                    //{
                    //    //INICIALIZA CAMPOS PARA PODER HACER OPERACIONES
                    //    NewRTotal.TotalHoras = 0;
                    //    NewRTotal.LiquidacionDiurna = 0;
                    //    NewRTotal.LiquidacionDiurnaFestivo = 0;
                    //    NewRTotal.LiquidacionNocturna = 0;
                    //    NewRTotal.LiquidacionNocturnaFestivo = 0;
                    //}
                    //NewRTotal.EmpleadoId = Item.EmpleadoId;
                    //NewRTotal.Empleado = Item.Empleado;
                    //NewRTotal.Estado = Item.Estado;
                    //NewRTotal.LiquidacionDiurna = NewRTotal.LiquidacionDiurna + Item.LiquidacionDiurna;
                    //NewRTotal.LiquidacionDiurnaFestivo = NewRTotal.LiquidacionDiurnaFestivo + Item.LiquidacionDiurnaFestivo;
                    //NewRTotal.LiquidacionNocturna = NewRTotal.LiquidacionNocturna + Item.LiquidacionNocturna;
                    //NewRTotal.LiquidacionNocturnaFestivo = NewRTotal.LiquidacionNocturnaFestivo + Item.LiquidacionNocturnaFestivo;
                    //NewRTotal.TotalHoras = NewRTotal.TotalHoras + Item.TotalHoras;
                    //if (NumeroGuia == TotalRegistros)
                    //{
                    //    NewRTotal.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                    //    NumeroGuia = 0;
                    //    ListadoFinal.Add(NewRTotal);
                    //    NewRTotal = new InformeTotalHE();
                    //}
                }
                if (UnidadOrg != null && UnidadOrg != "" || (Empresa != null && Empresa != "") || (Documento != null && Documento != "") || (NroEmpleado != null && NroEmpleado != ""))
                {
                    List<InformeTotalHE> nuevaLista = new List<InformeTotalHE>();
                    foreach (var item in ListadoFinal.Reverse<InformeTotalHE>())
                    {
                        bool cumpleCriterio = true;
                        if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Documento != Documento && !string.IsNullOrEmpty(Documento))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.NroEmpleado != NroEmpleado && !string.IsNullOrEmpty(NroEmpleado))
                        {
                            cumpleCriterio = false;
                        }


                        if (cumpleCriterio)
                        {
                            nuevaLista.Add(item);
                        }
                    }
                    ListadoFinal = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();

                }
                if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
                {
                    return View(new List<InformeTotalHE>());
                }

            }
            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ThenBy(x => x.FechaHora).ThenByDescending(x => x.Id).ToList();
            return View(ListadoFinal);

        }

        public ActionResult InformeHorasExtraGHTotalesxEmpleado(string JefeID, string TrabajadorS, string FechaI, string FechaF, string NmrRegistro, string UnidadOrg, string Empresa, string Documento, string NroEmpleado)
        {
            List<string> funciones = Acceso.Validar(Session["Empleado"]);
            if (Acceso.EsAnonimo)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (!Acceso.EsAnonimo && !funciones.Contains("HorasExtraGestionHumanaInformeTotales"))
            {
                Session.Add("ErrorAutorizacion", "No cuenta con autorización para la opción seleccionada");
                return RedirectToAction("Index", "Login");
            }

            Empleado empleado = new Empleado();

            var Empleadolog = Session["Empleado"];
            int JefeID2 = 0;


            JefeID2 = Convert.ToInt32(Empleadolog);



            List<InformeTotalHE> ListadoFinal = new List<InformeTotalHE>();
            using (var db = new AutogestionContext())
            {

                //----------------List Empleados------------------------------//
                // ViewBag.Sociedad = db.Sociedad.ToList();

                var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
                ViewBag.Empresa = db.Sociedad.Where(x => x.Codigo== Jefe.Empresa).ToList();
                ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO" && f.Empresa ==Jefe.Empresa).OrderBy(x => x.Nombres).ToList();
                ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.ToList();
                var areaDescripcionGroups = db.Empleados.Where(x => x.Activo == "SI" && (x.AreaDescripcion != null && x.AreaDescripcion != "") && x.Empresa ==Jefe.Empresa).GroupBy(b => b.UnidadOrganizativa).ToList();
                ViewBag.AreaDescripcion = new List<SelectListItem>();
                foreach (var x in areaDescripcionGroups)
                {
                    Empleado Item = x.FirstOrDefault();
                    if (Item.UnidadOrganizativa != "00003103" && Item.UnidadOrganizativa != "00003105" && Item.UnidadOrganizativa != "00003109" &&  Item.UnidadOrganizativa != "00003110" &&  Item.UnidadOrganizativa != "00003117" && Item.UnidadOrganizativa != "00003625" && Item.UnidadOrganizativa != "00003700")
                    {
                        ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion + " - " + Item.Empresa });
                    }
                }
                DateTime Fecha1 = DateTime.Now;
                DateTime Fecha2 = DateTime.Now;

                List<SelectListItem> lst = new List<SelectListItem>();
                Int32 IdProceso = new Int32();
                if (NmrRegistro != "")
                { IdProceso = Convert.ToInt32(NmrRegistro); }
                if (!DateTime.TryParse(FechaI, out Fecha1))
                { Fecha1 = new DateTime(); }
                if (!DateTime.TryParse(FechaF, out Fecha2))
                { Fecha2 = DateTime.Now; }
                if (TrabajadorS == "")
                {
                    ListadoFinal = db.HorasExtra
                             .Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado }
                             ).Where(Union => DbFunctions.TruncateTime(Union.Detallado.Fecha) >= Fecha1 &&
                               DbFunctions.TruncateTime(Union.Detallado.Fecha) <= Fecha2
                               && Union.Encabezado.Estado == 3 &&  SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                               && Union.Encabezado.Empleado.Empresa == Jefe.Empresa
                             ).Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago
                                    }).ToList();
                }

                if (TrabajadorS != "")
                {
                    int id = -1;
                    Int32.TryParse(TrabajadorS, out id);


                    int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

                    ListadoFinal = db.HorasExtra.Join(db.DetalleHorasExtras,
                                Encabezado => Encabezado.Id,
                                Detallado => Detallado.HorasExtraId,
                                (Encabezado, Detallado) => new { Encabezado, Detallado }
                             ).Where(Union =>
                                    Ids.Contains(Union.Encabezado.Id) && Union.Detallado.Fecha >= Fecha1 && Union.Detallado.Fecha <= Fecha2
                                    && Union.Encabezado.Estado == 3 &&
                                    SqlFunctions.StringConvert((decimal)Union.Encabezado.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)Union.Encabezado.Id).Contains(NmrRegistro)
                                     && Union.Encabezado.Empleado.Empresa == empleado.Empresa
                                    ).Select(x =>
                                    new InformeTotalHE
                                    {
                                        Id = x.Encabezado.Id,
                                        FechadeRegistro = x.Encabezado.FechaDeRegistro,
                                        FechaHora = x.Detallado.Fecha,
                                        EmpleadoId = x.Encabezado.EmpleadoId,
                                        HoraDesde = x.Detallado.HoraDesde,
                                        HoraHasta = x.Detallado.HoraHasta,
                                        LiquidacionDiurna = x.Detallado.LiquidacionDiurna,
                                        LiquidacionDiurnaFestivo = x.Detallado.LiquidacionDiurnaFestivo,
                                        LiquidacionNocturna = x.Detallado.LiquidacionNocturna,
                                        LiquidacionNocturnaFestivo = x.Detallado.LiquidacionNocturnaFestivo,
                                        TotalHoras = x.Detallado.TotalHoras,
                                        Estado = x.Encabezado.Estado,
                                        FechaPago = x.Encabezado.FechaPago
                                    }).ToList();
                }

                ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ToList();
                int NumeroGuia = 0;
                InformeTotalHE NewRTotal = new InformeTotalHE();



                foreach (InformeTotalHE Item in ListadoFinal.Reverse<InformeTotalHE>())
                {
                    int TotalRegistros = ListadoFinal.Where(e => e.EmpleadoId == Item.EmpleadoId).Count();
                    Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
                    Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                    NewRTotal.Empleado = Item.Empleado;
                    NewRTotal.EstadosHorasExtra = Item.EstadosHorasExtra;

                    NumeroGuia++;

                    if (NewRTotal.TotalHoras == null || NewRTotal.LiquidacionDiurna == null ||
                        NewRTotal.LiquidacionDiurnaFestivo == null || NewRTotal.LiquidacionNocturna == null ||
                        NewRTotal.LiquidacionNocturnaFestivo == null)
                    {
                        //INICIALIZA CAMPOS PARA PODER HACER OPERACIONES
                        NewRTotal.TotalHoras = 0;
                        NewRTotal.LiquidacionDiurna = 0;
                        NewRTotal.LiquidacionDiurnaFestivo = 0;
                        NewRTotal.LiquidacionNocturna = 0;
                        NewRTotal.LiquidacionNocturnaFestivo = 0;
                    }
                    NewRTotal.EmpleadoId = Item.EmpleadoId;
                    NewRTotal.Empleado = Item.Empleado;
                    NewRTotal.Estado = Item.Estado;
                    NewRTotal.LiquidacionDiurna = NewRTotal.LiquidacionDiurna + Item.LiquidacionDiurna;
                    NewRTotal.LiquidacionDiurnaFestivo = NewRTotal.LiquidacionDiurnaFestivo + Item.LiquidacionDiurnaFestivo;
                    NewRTotal.LiquidacionNocturna = NewRTotal.LiquidacionNocturna + Item.LiquidacionNocturna;
                    NewRTotal.LiquidacionNocturnaFestivo = NewRTotal.LiquidacionNocturnaFestivo + Item.LiquidacionNocturnaFestivo;
                    NewRTotal.TotalHoras = NewRTotal.TotalHoras + Item.TotalHoras;
                    if (NumeroGuia == TotalRegistros)
                    {
                        NewRTotal.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
                        NumeroGuia = 0;
                        ListadoFinal.Add(NewRTotal);
                        NewRTotal = new InformeTotalHE();
                    }
                }
                if (UnidadOrg != null && UnidadOrg != "" || (Empresa != null && Empresa != "") || (Documento != null && Documento != "") || (NroEmpleado != null && NroEmpleado != ""))
                {
                    List<InformeTotalHE> nuevaLista = new List<InformeTotalHE>();
                    foreach (var item in ListadoFinal.Reverse<InformeTotalHE>())
                    {
                        bool cumpleCriterio = true;
                        if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.Documento != Documento && !string.IsNullOrEmpty(Documento))
                        {
                            cumpleCriterio = false;
                        }

                        if (item.Empleado.NroEmpleado != NroEmpleado && !string.IsNullOrEmpty(NroEmpleado))
                        {
                            cumpleCriterio = false;
                        }

                        if (cumpleCriterio)
                        {
                            nuevaLista.Add(item);
                        }
                    }
                    ListadoFinal = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();

                }
                if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
                {
                    return View(new List<InformeTotalHE>());
                }

            }
            ListadoFinal = ListadoFinal.OrderBy(x => x.EmpleadoId).ThenBy(x => x.FechaHora).ThenByDescending(x => x.Id).ToList();
            return View(ListadoFinal);

        }

        [HttpGet]
        public async Task<FileResult> ExportaExcel(List<HorasExtra> model)
        {

            var nombrearchivo = $"InformeProcesos_Horas_Extra.xlsx";
            return GenerarExcel(nombrearchivo, model);

        }



        //    var nom = "hola"; 
        //    var nombrearchivo = $"InformeProcesos_Horas_Extra.xlsx";

        //   return GenerarExcel(nombrearchivo, proceso);

        //}


        private FileResult GenerarExcel(string nombreArchivo, List<HorasExtra> HorasExtras)
        {

            DataTable dt = new DataTable("HorasExtra");
            dt.Columns.AddRange(new DataColumn[] {

                new DataColumn("Id"),
                new DataColumn("EmpleadoId"),


            });


            foreach (var horas in HorasExtras)
            {

                dt.Rows.Add(horas.Id,
                            horas.EmpleadoId);
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);

                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);

                    return File(ms.ToArray(),

                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);


                }

            }


        }



        //private Task<List<HorasExtra>>Consultar(string JefeID, string TrabajadorS, string FechaI, string FechaF, string Estado, string NmrRegistro, string UnidadOrg, string Empresa, string Documento, string NroEmpleado) {

        //    var Empleadolog = Session["Empleado"];
        //    var EstadoId = 0;
        //    int JefeID2 = 0;
        //    if (JefeID == null || JefeID == "")
        //    {
        //        JefeID2 = Convert.ToInt32(Empleadolog);
        //    }
        //    List<HorasExtra> Proceso = new List<HorasExtra>();


        //    using (var db = new AutogestionContext())
        //    {

        //        //----------------List Empleados------------------------------//
        //        ViewBag.Sociedad = db.Sociedad.ToList();
        //        var Jefe = db.Empleados.FirstOrDefault(e => e.Id == JefeID2);
        //        ViewBag.Empleado = db.Empleados.Where(f => f.Activo != "NO").ToList();
        //        ViewBag.EstadosHorasExtra = db.EstadosHorasExtra.ToList();
        //        var areaDescripcionGroups = db.Empleados.Where(x => x.Activo == "SI" && (x.AreaDescripcion != null && x.AreaDescripcion != "")).GroupBy(b => b.UnidadOrganizativa).ToList();
        //        ViewBag.AreaDescripcion = new List<SelectListItem>();
        //        foreach (var x in areaDescripcionGroups)
        //        {
        //            Empleado Item = x.FirstOrDefault();
        //            ViewBag.AreaDescripcion.Add(new SelectListItem() { Value = Item.UnidadOrganizativa, Text = "" + Item.AreaDescripcion + " - " + Item.Empresa });
        //        }
        //        DateTime Fecha1 = DateTime.Now;
        //        DateTime Fecha2 = DateTime.Now;

        //        List<SelectListItem> lst = new List<SelectListItem>();
        //        Int32 IdProceso = new Int32();

        //        //------------Denifir variable EstadoId------------------------//
        //        //if (Estado == "Solicitado") { EstadoId = 1; }
        //        //else if (Estado == "Aprobado Jefe directo") { EstadoId = 2; }
        //        //else if (Estado == "Cerrado") { EstadoId = 3; }
        //        //else if (Estado == "Rechazado") { EstadoId = 4; };
        //        if (Estado != null && Estado != "") { EstadoId = Convert.ToInt32(Estado); }


        //        if (NmrRegistro != "")
        //        {
        //            IdProceso = Convert.ToInt32(NmrRegistro);
        //        }

        //        if (!DateTime.TryParse(FechaI, out Fecha1))
        //        {
        //            Fecha1 = new DateTime();
        //        }

        //        if (!DateTime.TryParse(FechaF, out Fecha2))
        //        {
        //            Fecha2 = DateTime.Now;
        //        }

        //        if (TrabajadorS == "")
        //        {
        //            if (Estado == "")
        //            {
        //                Proceso = db.HorasExtra.Where(e => DbFunctions.TruncateTime(e.FechaDeRegistro) >= Fecha1 &&
        //                                  DbFunctions.TruncateTime(e.FechaDeRegistro) <= Fecha2

        //                                  && e.Estado == 3 &&  SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
        //                                  ).ToList();
        //            }
        //            else
        //            {
        //                Proceso = db.HorasExtra.Where(e => DbFunctions.TruncateTime(e.FechaDeRegistro) >= Fecha1
        //                             && e.Estado == 3 &&
        //                             DbFunctions.TruncateTime(e.FechaDeRegistro) <= Fecha2
        //                             && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
        //                             ).ToList();
        //            }

        //        }
        //        if (TrabajadorS != "")
        //        {

        //            if (Estado == "")
        //            {
        //                int id = -1;
        //                Int32.TryParse(TrabajadorS, out id);


        //                int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

        //                Proceso = db.HorasExtra.Where(e =>
        //                            Ids.Contains(e.Id) && e.FechaDeRegistro >= Fecha1 && e.FechaDeRegistro <= Fecha2
        //                            && e.Estado == 3 &&
        //                            SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)).ToList();

        //            }
        //            else
        //            {
        //                int id = -1;
        //                Int32.TryParse(TrabajadorS, out id);
        //                int[] Ids = (from item in db.HorasExtra where item.EmpleadoId.Equals(id) select item.Id).ToArray();

        //                Proceso = db.HorasExtra.Where(e =>
        //                            Ids.Contains(e.Id) && e.FechaDeRegistro >= Fecha1 && e.FechaDeRegistro <= Fecha2
        //                            && e.Estado == 3 &&
        //                            SqlFunctions.StringConvert((decimal)e.EmpleadoId).Contains(TrabajadorS) && SqlFunctions.StringConvert((decimal)e.Id).Contains(NmrRegistro)
        //                        ).ToList();



        //            }

        //        }
        //        List<Empleado> Empleados_Jefe = db.Empleados.Where(x => x.Jefe == Jefe.NroEmpleado && x.Activo == "SI").ToList();
        //        Proceso = Proceso.OrderBy(x => x.EmpleadoId).ToList();
        //        int NumeroGuia = 0;
        //        HorasExtra NewRTotal = new HorasExtra();

        //        foreach (HorasExtra Item in Proceso.Reverse<HorasExtra>())
        //        {
        //            int TotalRegistros = Proceso.Where(e => e.EmpleadoId == Item.EmpleadoId).Count();
        //            Item.Empleado = db.Empleados.FirstOrDefault(e => e.Id == Item.EmpleadoId);
        //            Item.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
        //            NumeroGuia++;

        //            if (NewRTotal.TotalHoras == null || NewRTotal.TotalLiquidacionDiurna == null ||
        //                NewRTotal.TotalLiquidacionDiurnaFestivo == null || NewRTotal.TotalLiquidacionNocturna == null ||
        //                NewRTotal.TotalLiquidacionNocturnaFestivo == null)
        //            {
        //                //INICIALIZA CAMPOS PARA PODER HACER OPERACIONES
        //                NewRTotal.TotalHoras = 0;
        //                NewRTotal.TotalLiquidacionDiurna = 0;
        //                NewRTotal.TotalLiquidacionDiurnaFestivo = 0;
        //                NewRTotal.TotalLiquidacionNocturna = 0;
        //                NewRTotal.TotalLiquidacionNocturnaFestivo = 0;
        //            }
        //            NewRTotal.EmpleadoId = Item.EmpleadoId;
        //            NewRTotal.Empleado = Item.Empleado;
        //            NewRTotal.Estado = Item.Estado;
        //            NewRTotal.TotalLiquidacionDiurna = NewRTotal.TotalLiquidacionDiurna + Item.TotalLiquidacionDiurna;
        //            NewRTotal.TotalLiquidacionDiurnaFestivo = NewRTotal.TotalLiquidacionDiurnaFestivo + Item.TotalLiquidacionDiurnaFestivo;
        //            NewRTotal.TotalLiquidacionNocturna = NewRTotal.TotalLiquidacionNocturna + Item.TotalLiquidacionNocturna;
        //            NewRTotal.TotalLiquidacionNocturnaFestivo = NewRTotal.TotalLiquidacionNocturnaFestivo + Item.TotalLiquidacionNocturnaFestivo;
        //            NewRTotal.TotalHoras = NewRTotal.TotalHoras + Item.TotalHoras;
        //            if (NumeroGuia == TotalRegistros)
        //            {
        //                NewRTotal.EstadosHorasExtra = db.EstadosHorasExtra.FirstOrDefault(e => e.Id == Item.Estado);
        //                NumeroGuia = 0;
        //                Proceso.Add(NewRTotal);
        //                NewRTotal = new HorasExtra();
        //            }
        //        }
        //        if (UnidadOrg != null && UnidadOrg != "" || (Empresa != null && Empresa != "") || (Documento != null && Documento != "") || (NroEmpleado != null && NroEmpleado != ""))
        //        {
        //            List<HorasExtra> nuevaLista = new List<HorasExtra>();
        //            foreach (var item in Proceso.Reverse<HorasExtra>())
        //            {
        //                bool cumpleCriterio = true;
        //                if (item.Empleado.UnidadOrganizativa != UnidadOrg && !string.IsNullOrEmpty(UnidadOrg))
        //                {
        //                    cumpleCriterio = false;
        //                }

        //                if (item.Empleado.Empresa != Empresa && !string.IsNullOrEmpty(Empresa))
        //                {
        //                    cumpleCriterio = false;
        //                }

        //                if (item.Empleado.Documento != Documento && !string.IsNullOrEmpty(Documento))
        //                {
        //                    cumpleCriterio = false;
        //                }

        //                if (item.Empleado.NroEmpleado != NroEmpleado && !string.IsNullOrEmpty(NroEmpleado))
        //                {
        //                    cumpleCriterio = false;
        //                }

        //                if (cumpleCriterio)
        //                {
        //                    nuevaLista.Add(item);
        //                }
        //            }
        //            Proceso = nuevaLista.OrderBy(x => x.EmpleadoId).ToList();

        //        }
        //        if (string.IsNullOrWhiteSpace(FechaI) || string.IsNullOrWhiteSpace(FechaF) || !DateTime.TryParse(FechaI, out Fecha1) || !DateTime.TryParse(FechaF, out Fecha2))
        //        {
        //            return(new List<HorasExtra>());
        //        }

        //    }
        //    Proceso = Proceso.OrderBy(x => x.EmpleadoId).ThenByDescending(x => x.Id).ToList();


        //}



    }
}

