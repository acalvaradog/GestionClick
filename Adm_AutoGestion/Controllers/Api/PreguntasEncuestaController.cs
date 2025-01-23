using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Adm_AutoGestion.Services;
using Adm_AutoGestion.Models;
using System.Web.Http.Cors;
using Newtonsoft.Json;


namespace Adm_AutoGestion.Controllers.Api
{
       [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PreguntasEncuestaController : ApiController
    {

         private PreguntasEncRepository _repo;

            public PreguntasEncuestaController()
        {
            _repo = new PreguntasEncRepository();
        }



            // GET api/preguntasencuesta
            public string Get()
            {
                string grupo = "1";
                //return new string[] { "value1", "value2" };
                var model = _repo.ObtenerTodos2(grupo);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }

            // GET api/llamargrupo2
            [HttpGet]
            [Route("api/llamargrupo2")]
            public string llamargrupo2()
            {
                string grupo = "2";
                var model = _repo.ObtenerTodos2(grupo);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }

        [HttpGet]
        [Route("api/PreguntasGrupo2")]
        public IHttpActionResult PreguntasGrupo2()
        {
            string grupo = "2";
            var model = _repo.ObtenerTodos2(grupo);
            return Json(model);
        }



        // GET api/llamargrupo3
        [HttpGet]
            [Route("api/llamargrupo3")]
            public string llamargrupo3()
            {
                string grupo = "3";

                var model = _repo.ObtenerTodos2(grupo);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }


            // GET api/llamargrupo4
            [HttpGet]
            [Route("api/llamargrupo4")]
            public string llamargrupo4()
            {
                string grupo = "4";
                var model = _repo.ObtenerTodos2(grupo);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }

            // GET api/llamargrupo5
            [HttpGet]
            [Route("api/llamargrupo5")]
            public string llamargrupo5()
            {
                string grupo = "2";
                var model = _repo.ObtenerTodos2(grupo);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }

            // GET api/llamargrupo5
            [HttpGet]
            [Route("api/llamargrupo6/{Id}")]
            public string llamargrupo6(string Id)
            {
                //string grupo = "6";
                var model = _repo.ObtenerTodos2(Id);

                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(model);
                return json;
            }


            // GET api/DatosVacunas
            [HttpGet]
            [Route("api/DatosVacunas/{Cod}/{valor}")]
            public string DatosVacunas(string Cod, string valor)
            {
                string model = "";
                using (var db = new AutogestionContext())
                {

                    if (valor == "1")
                    {
                        model = _repo.ObtenerDatosVacunas(Cod);
                    }

                    if (valor == "2")
                    {
                        Empleado emp = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Cod);

                        model = emp.VacunaDosis1 + "," + emp.VacunaDosis2 + "," + emp.DosisRefuerzo + "," + emp.Correo;
                    }

                    if (valor == "3")
                    {
                        Empleado emp = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Cod);

                        model = emp.VacunaDosis1 + "," + emp.OtraD1 + "," + emp.FechaDosis1 + "," + emp.VacunaDosis2 + "," + emp.OtraD2 + "," + emp.FechaDosis2 + "," + emp.DosisRefuerzo + "," + emp.OtraR + "," + emp.FechaRefuerzo ;
                    }
                }
                //var jsonSerialiser = new JavaScriptSerializer();
                var json = JsonConvert.SerializeObject(model);
                return json;

            }

            // GET api/ValidarVacunasIngreso
            [HttpGet]
            [Route("api/ValidarVacunasIngreso/{Codigo}")]
            public string ValidarVacunasIngreso(string Cod)
            {
                string model = "";
                using (var db = new AutogestionContext())
                {

                    Empleado emp = db.Empleados.FirstOrDefault(e => e.NroEmpleado == Cod);

                    model = emp.VacunaDosis1 + "," + emp.VacunaDosis2 + "," + emp.DosisRefuerzo;

                }

                //return model;
                var json = JsonConvert.SerializeObject(model);
                return json;

            }



            [HttpGet]
            [Route("api/ConsultarFecha/{Id}")]
            public string ConsultarFecha(int Id)
            {
                var resp = "";
                using (var db = new AutogestionContext())
                {
                    var model1 = db.EncabezadoEncuesta.OrderByDescending(e => e.Id).FirstOrDefault(e => e.EmpleadoId == Id);
                    var model = db.Encuesta.FirstOrDefault(e => e.EncabezadoEncuesta_Id == model1.Id && e.NumeroPregunta == 15);

                    if (model != null)
                    {
                        resp = model.Respuesta;
                    }

                    var jsonSerialiser = new JavaScriptSerializer();
                    var json = jsonSerialiser.Serialize(resp);
                    return json;
                }

            }


            // POST api/preguntasencuesta
            [HttpPost]
            public IHttpActionResult Post([FromBody]Empleado Empleado)
            {
                var idempleado = Empleado.Id;
                var dosis1 = Empleado.VacunaDosis1;
                var otrad1 = Empleado.OtraD1;
                var fechad1 = Empleado.FechaDosis1;
                var dosis2 = Empleado.VacunaDosis2;
                var otrad2 = Empleado.OtraD2;
                var fechad2 = Empleado.FechaDosis2;
                var dosisr = Empleado.DosisRefuerzo;
                var otrar = Empleado.OtraR;
                var fechadr = Empleado.FechaRefuerzo;
                var dosisr2 = Empleado.DosisRefuerzo2;
                var otrar2 = Empleado.OtraR2;
                var fechadr2 = Empleado.FechaRefuerzo2;
                string respuesta = "";

                using (var db = new AutogestionContext())
                {


                    var empleado = db.Empleados.FirstOrDefault(x => x.Id == idempleado);

                    if (Object.ReferenceEquals(null, empleado))
                    {
                        respuesta = "3";
                    }
                    else
                    {
                        empleado.VacunaDosis1 = dosis1;
                        empleado.OtraD1 = otrad1;
                        empleado.FechaDosis1 = fechad1;
                        empleado.VacunaDosis2 = dosis2;
                        empleado.OtraD2 = otrad2;
                        empleado.FechaDosis2 = fechad2;
                        empleado.DosisRefuerzo = dosisr;
                        empleado.OtraR = otrar;
                        empleado.FechaRefuerzo = fechadr;
                        empleado.DosisRefuerzo2 = dosisr2;
                        empleado.OtraR2 = otrar2;
                        empleado.FechaRefuerzo2 = fechadr2;
                        db.SaveChanges();
                        respuesta = "1";

                    }

                }

                return Json(respuesta);

            }




            // GET api/preguntasencuesta/5
            public string Get(int id)
            {
                return "value";
            }

            // POST api/preguntasencuesta
            //public void Post([FromBody]string value)
            //{
            //}

            // PUT api/preguntasencuesta/5
            public void Put(int id, [FromBody]string value)
            {
            }

            // DELETE api/preguntasencuesta/5
            public void Delete(int id)
            {
            }

/////////////////////////////////////**************************  productivo ****************************//////////////////////////////////////////////////

            ////GET api/preguntasencuesta
            //public string Get()
            //{
            //    string grupo = "1";
            //    //return new string[] { "value1", "value2" };
            //    var model = _repo.ObtenerTodos2(grupo);

            //    var jsonSerialiser = new JavaScriptSerializer();
            //    var json = jsonSerialiser.Serialize(model);
            //    return json;
            //}

            //// GET api/llamargrupo2
            //[HttpGet]
            //[Route("api/llamargrupo2")]
            //public string llamargrupo2()
            //{
            //    string grupo = "2";
            //    var model = _repo.ObtenerTodos2(grupo);

            //    var jsonSerialiser = new JavaScriptSerializer();
            //    var json = jsonSerialiser.Serialize(model);
            //    return json;
            //}

            //// GET api/llamargrupo3
            //[HttpGet]
            //[Route("api/llamargrupo3")]
            //public string llamargrupo3()
            //{
            //    string grupo = "3";
            //    var model = _repo.ObtenerTodos2(grupo);

            //    var jsonSerialiser = new JavaScriptSerializer();
            //    var json = jsonSerialiser.Serialize(model);
            //    return json;
            //}


            //// GET api/llamargrupo4
            //[HttpGet]
            //[Route("api/llamargrupo4")]
            //public string llamargrupo4()
            //{
            //    string grupo = "4";
            //    var model = _repo.ObtenerTodos2(grupo);

            //    var jsonSerialiser = new JavaScriptSerializer();
            //    var json = jsonSerialiser.Serialize(model);
            //    return json;
            //}

            //// GET api/llamargrupo5
            //[HttpGet]
            //[Route("api/llamargrupo5")]
            //public string llamargrupo5()
            //{
            //    string grupo = "2";
            //    var model = _repo.ObtenerTodos2(grupo);

            //    var jsonSerialiser = new JavaScriptSerializer();
            //    var json = jsonSerialiser.Serialize(model);
            //    return json;
            //}




            //// GET api/preguntasencuesta/5
            //public string Get(int id)
            //{
            //    return "value";
            //}

            //// POST api/preguntasencuesta
            //public void Post([FromBody]string value)
            //{
            //}

            //// PUT api/preguntasencuesta/5
            //public void Put(int id, [FromBody]string value)
            //{
            //}

            //// DELETE api/preguntasencuesta/5
            //public void Delete(int id)
            //{
            //}
        /////////////////////////////////////**************************  productivo ****************************//////////////////////////////////////////////////
    }
}
