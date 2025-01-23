using Adm_AutoGestion.Controllers;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaDesempeno;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace Adm_AutoGestion.Services
{
    public class AArea
    {
        public string Area { get; set; }
        public string Area2 { get; set; }
    }
    public class DatosPorArea
    {
        public string Area { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string Empresa { get; set; }
        public string Activo { get; set; }
        public string Cargo { get; set; }
    }
    public class CuadroMandoRepository
    {
        AutogestionContext db = new AutogestionContext();

        public async Task<SummaryReport> ObtenerDatosGrafica(string Id, string FInicio, string FFin, bool EsJefe, string Area, string Cargo)
        {
            SummaryReport sr = new SummaryReport();

            sr.TablaEdadesEmpleados = await ObtenerEdadesEmpleados(Id, EsJefe, Area, Cargo);
            sr.TablaTiempoPermanencia = await ObtenerTiempoPermanencia(Id, EsJefe, Area, Cargo);
            sr.TablaGeneroEmpleados = await ObtenerGeneroEmpleados(FInicio, FFin, Id, EsJefe, Area, Cargo);
            sr.TablaAreaPersonal = await ObtenerAreaPersonal(Id, EsJefe, Area, Cargo);
            sr.TablaCargoEmpleado = await ObtenerCargoEmpleado(Id, EsJefe, Area, Cargo);
            sr.TablaIngresosMes = await ObtenerIngresosMes(FInicio, FFin, Id, EsJefe, Area, Cargo);
            sr.TablaGeneroEmpleadosEgreso = await ObtenerGeneroEmpleadosEgreso(FInicio, FFin, Id, EsJefe, Area, Cargo);
            sr.TablaEdadesEmpleadosEgreso = await ObtenerEdadesEmpleadosEgreso(Id, EsJefe, Area, Cargo);
            sr.TablaTiempoPermanenciaEgreso = await ObtenerTiempoPermanenciaEgreso(Id, EsJefe, Area, Cargo);
            sr.TablaMotivoRenuncia = await ObtenerMotivoRenuncia(FInicio, FFin, Id, EsJefe, Area, Cargo);
            sr.TablaIndicadorRotacionEgresosPorArea = await ObtenerIndicadorRotacionEgresosPorArea(FInicio, FFin, Id, EsJefe, Area);
            sr.TablaRotacionPorCargo = await ObtenerRotacionPorCargo(Id, EsJefe, Area, Cargo);
            sr.TablaIndicadorRotacionEgresosPorCargo = await ObtenerIndicadorRotacionEgresosPorCargo(FInicio, FFin, Id, EsJefe, Area, Cargo);
            sr.TablaIndicadorRotacionEgresosPorAreaFosunab = await ObtenerIndicadorRotacionEgresosPorAreaFosunab(FInicio, FFin, Id, EsJefe, Area);
            sr.TablaRotacionPorCargoFosunab = await ObtenerRotacionPorCargoFosunab(Id, EsJefe, Area, Cargo);
            sr.TablaIndicadorRotacionEgresosPorCargoFosunab = await ObtenerIndicadorRotacionEgresosPorCargoFosunab(FInicio, FFin, Id, EsJefe, Area, Cargo);
            return sr;
        }

        public async Task<List<TablaEdadesEmpleados>> ObtenerEdadesEmpleados(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaEdadesEmpleados> tabla = new List<TablaEdadesEmpleados>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "SI" && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                t = await db.Empleados.Where(x => x.Activo == "SI").ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int rango1 = 0;
            int rango2 = 0;
            int rango3 = 0;
            int rango4 = 0;
            int rango5 = 0;
            //Fosunab
            int rango6 = 0;
            int rango7 = 0;
            int rango8 = 0;
            int rango9 = 0;
            int rango10 = 0;

            foreach (var item in datos)
            {
                DateTime FechaNac = Convert.ToDateTime(item.FechaNacimiento);
                var Nac = int.Parse(FechaNac.ToString("yyyyMMdd"));
                var FechaActual = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                var Edad = (FechaActual - Nac) / 10000;
                if (item.Empresa == "1000")
                {
                    if (Edad >= 18 && Edad <= 25)
                    {
                        rango1 += 1;
                    }
                    else if (Edad >= 26 && Edad <= 35)
                    {
                        rango2 += 1;
                    }
                    else if (Edad >= 36 && Edad <= 45)
                    {
                        rango3 += 1;
                    }
                    else if (Edad >= 46 && Edad <= 55)
                    {
                        rango4 += 1;
                    }
                    else if (Edad >= 56 && Edad <= 120)
                    {
                        rango5 += 1;
                    }
                }
                else if (item.Empresa == "2000")
                {
                    if (Edad >= 18 && Edad <= 25)
                    {
                        rango6 += 1;
                    }
                    else if (Edad >= 26 && Edad <= 35)
                    {
                        rango7 += 1;
                    }
                    else if (Edad >= 36 && Edad <= 45)
                    {
                        rango8 += 1;
                    }
                    else if (Edad >= 46 && Edad <= 55)
                    {
                        rango9 += 1;
                    }
                    else if (Edad >= 56 && Edad <= 120)
                    {
                        rango10 += 1;
                    }
                }
            }
            int Total = rango1 + rango2 + rango3 + rango4 + rango5;
            Rangos.Add(new KeyValuePair<string, int>("18 a 25 años", rango1));
            Rangos.Add(new KeyValuePair<string, int>("26 a 35 años", rango2));
            Rangos.Add(new KeyValuePair<string, int>("36 a 45 años", rango3));
            Rangos.Add(new KeyValuePair<string, int>("46 a 55 años", rango4));
            Rangos.Add(new KeyValuePair<string, int>("Mayores 56 años", rango5));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaEdadesEmpleados { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = rango6 + rango7 + rango8 + rango9 + rango10;
            Rangos2.Add(new KeyValuePair<string, int>("18 a 25 años", rango6));
            Rangos2.Add(new KeyValuePair<string, int>("26 a 35 años", rango7));
            Rangos2.Add(new KeyValuePair<string, int>("36 a 45 años", rango8));
            Rangos2.Add(new KeyValuePair<string, int>("46 a 55 años", rango9));
            Rangos2.Add(new KeyValuePair<string, int>("Mayores 56 años", rango10));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaEdadesEmpleados { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaTiempoPermanencia>> ObtenerTiempoPermanencia(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaTiempoPermanencia> tabla = new List<TablaTiempoPermanencia>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "SI" && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                t = await db.Empleados.Where(x => x.Activo == "SI").ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int rango1 = 0;
            int rango2 = 0;
            int rango3 = 0;
            int rango4 = 0;
            int rango5 = 0;
            int rango6 = 0;
            //Fosunab
            int rango7 = 0;
            int rango8 = 0;
            int rango9 = 0;
            int rango10 = 0;
            int rango11 = 0;
            int rango12 = 0;

            foreach (var item in datos)
            {
                DateTime FechaIngreso = Convert.ToDateTime(item.FechaIngreso);
                var Ingreso = int.Parse(FechaIngreso.ToString("yyyyMMdd"));
                var FechaActual = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                float Mes = (float)(FechaActual - Ingreso) / 10000;
                int Edad = (FechaActual - Ingreso) / 10000;

                if (item.Empresa == "1000")
                {
                    if (Mes <= 0.04)
                    {
                        rango1 += 1;
                    }
                    else if (Mes > 0.04 && Mes < 1)
                    {
                        rango2 += 1;
                    }
                    else if (Edad >= 1 && Edad <= 2)
                    {
                        rango3 += 1;
                    }
                    else if (Edad >= 3 && Edad <= 5)
                    {
                        rango4 += 1;
                    }
                    else if (Edad >= 6 && Edad <= 9)
                    {
                        rango5 += 1;
                    }
                    else if (Edad >= 10)
                    {
                        rango6 += 1;
                    }
                }else if(item.Empresa == "2000")
                {
                    if (Mes <= 0.04)
                    {
                        rango7 += 1;
                    }
                    else if (Mes > 0.04 && Mes < 1)
                    {
                        rango8 += 1;
                    }
                    else if (Edad >= 1 && Edad <= 2)
                    {
                        rango9 += 1;
                    }
                    else if (Edad >= 3 && Edad <= 5)
                    {
                        rango10 += 1;
                    }
                    else if (Edad >= 6 && Edad <= 9)
                    {
                        rango11 += 1;
                    }
                    else if (Edad >= 10)
                    {
                        rango12 += 1;
                    }
                }
            }
            int Total = rango1 + rango2 + rango3 + rango4 + rango5 + rango6;
            Rangos.Add(new KeyValuePair<string, int>("Menor o Igual a 4 meses", rango1));
            Rangos.Add(new KeyValuePair<string, int>("De 4 a 12 Meses", rango2));
            Rangos.Add(new KeyValuePair<string, int>("De 1 a 2 años", rango3));
            Rangos.Add(new KeyValuePair<string, int>("De 3 a 5 años", rango4));
            Rangos.Add(new KeyValuePair<string, int>("De 6 a 9 años", rango5));
            Rangos.Add(new KeyValuePair<string, int>("Mayor o igual a 10 años", rango6));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaTiempoPermanencia { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = rango7 + rango8 + rango9 + rango10 + rango11 + rango12;
            Rangos2.Add(new KeyValuePair<string, int>("Menor o Igual a 4 meses", rango7));
            Rangos2.Add(new KeyValuePair<string, int>("De 4 a 12 Meses", rango8));
            Rangos2.Add(new KeyValuePair<string, int>("De 1 a 2 años", rango9));
            Rangos2.Add(new KeyValuePair<string, int>("De 3 a 5 años", rango10));
            Rangos2.Add(new KeyValuePair<string, int>("De 6 a 9 años", rango11));
            Rangos2.Add(new KeyValuePair<string, int>("Mayor o igual a 10 años", rango12));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaTiempoPermanencia { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaGeneroEmpleados>> ObtenerGeneroEmpleados(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaGeneroEmpleados> tabla = new List<TablaGeneroEmpleados>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            DateTime? FI = null;
            DateTime? FF = null;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(e => e.Activo == "SI" && e.FechaIngreso >= FI && e.FechaIngreso <= FF && e.Jefe == empleado.NroEmpleado).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Activo == "SI" && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(e => e.Activo == "SI" && e.FechaIngreso >= FI && e.FechaIngreso <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Activo == "SI").ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int M = 0;
            int F = 0;
            //Fosunab
            int M2 = 0;
            int F2 = 0;
            foreach (var item in datos)
            {
                if (item.Empresa == "1000")
                {
                    if (item.Genero == "1")
                    {
                        M += 1;
                    }
                    else if (item.Genero == "2")
                    {
                        F += 1;
                    }
                }else if (item.Empresa == "2000")
                {
                    if (item.Genero == "1")
                    {
                        M2 += 1;
                    }
                    else if (item.Genero == "2")
                    {
                        F2 += 1;
                    }
                }
            }
            int Total = M + F;
            Rangos.Add(new KeyValuePair<string, int>("Femenino", F));
            Rangos.Add(new KeyValuePair<string, int>("Masculino", M));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaGeneroEmpleados { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = M2 + F2;
            Rangos2.Add(new KeyValuePair<string, int>("Femenino", F2));
            Rangos2.Add(new KeyValuePair<string, int>("Masculino", M2));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaGeneroEmpleados { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaAreaPersonal>> ObtenerAreaPersonal(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaAreaPersonal> tabla = new List<TablaAreaPersonal>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "SI" && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                t = await db.Empleados.Where(x => x.Activo == "SI").ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            var adm = 0;
            var asi = 0;
            //Fosunab
            var adm2 = 0;
            var asi2 = 0;
            foreach (var item in datos)
            {
                if (item.Empresa == "1000")
                {
                    if (item.TipoArea == "Administrativos CO")
                    {
                        adm += 1;
                    }
                    else if (item.TipoArea == "Asistenciales CO")
                    {
                        asi += 1;
                    }
                }else if (item.Empresa == "2000")
                {
                    if (item.TipoArea == "Administrativos CO")
                    {
                        adm2 += 1;
                    }
                    else if (item.TipoArea == "Asistenciales CO")
                    {
                        asi2 += 1;
                    }
                }
            }
            int Total = adm + asi;
            Rangos.Add(new KeyValuePair<string, int>("Asistencial", asi));
            Rangos.Add(new KeyValuePair<string, int>("Administrativa", adm));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaAreaPersonal { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = adm2 + asi2;
            Rangos2.Add(new KeyValuePair<string, int>("Asistencial", asi2));
            Rangos2.Add(new KeyValuePair<string, int>("Administrativa", adm2));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaAreaPersonal { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaCargoEmpleado>> ObtenerCargoEmpleado(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaCargoEmpleado> tabla = new List<TablaCargoEmpleado>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe  == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "SI" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "SI" && x.Area != null).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }
            List<IGrouping<string, AArea>> CargosDelArea = new List<IGrouping<string, AArea>>();

            if (AreasJefe.Count != 0)
            {
                foreach (var i in AreasJefe)
                {
                    if (Cargo != null && Cargo != "")
                    {
                        var Cargos = db.Empleados.Where(x => x.Area != null && x.Cargo == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                    else
                    {
                        var Cargos = db.Empleados.Where(x => x.Cargo != null && x.Area == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                }
            }
            else
            {
                var Cargos = db.Empleados.Where(x => x.Cargo == Cargo).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                if (Cargo != "")
                {
                    CargosDelArea.AddRange(Cargos);
                }
            }

            foreach (var i in CargosDelArea)
            {
                int fos = 0;
                int fosunab = 0;
                foreach (var item in datos)
                {
                    if (item.Empresa == "1000" && item.Cargo == i.Key)
                    {
                        fos += 1;
                    }
                    else if (item.Empresa == "2000" && item.Cargo == i.Key)
                    {
                        fosunab += 1;
                    }
                }
                tabla.Add(new TablaCargoEmpleado { Cargo = i.Key, FOS = fos, FOSUNAB = fosunab });
            }
            return tabla;
        }

        public async Task<List<TablaIngresosMes>> ObtenerIngresosMes(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaIngresosMes> tabla = new List<TablaIngresosMes>();
            DateTime? FechaFin;
            DateTime? FechaInicio;
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            DateTime? FI = null;
            DateTime? FF = null;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    FechaInicio = FI;
                    FechaFin = FF;
                    t = await db.Empleados.Where(e => e.FechaIngreso >= FechaInicio && e.FechaIngreso <= FechaFin && e.Jefe == empleado.NroEmpleado).ToListAsync();
                }
                else
                {
                    var MesActual = DateTime.Now.Month;
                    FechaInicio = Convert.ToDateTime($"1-{MesActual}-2023");
                    FechaFin = Convert.ToDateTime($"30-{MesActual}-2023");
                    t = await db.Empleados.Where(e => e.FechaIngreso >= FechaInicio && e.FechaIngreso <= FechaFin && e.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    FechaInicio = FI;
                    FechaFin = FF;
                    t = await db.Empleados.Where(e => e.FechaIngreso >= FechaInicio && e.FechaIngreso <= FechaFin).ToListAsync();
                }
                else
                {
                    var MesActual = DateTime.Now.Month;
                    FechaInicio = Convert.ToDateTime($"1-{MesActual}-2023");
                    FechaFin = Convert.ToDateTime($"30-{MesActual}-2023");
                    t = await db.Empleados.Where(e => e.FechaIngreso >= FechaInicio && e.FechaIngreso <= FechaFin).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            int fos = 0;
            int fosunab = 0;
            var Mes = "";

            if (FI != null && FF != null)
            {
                var fosEne = 0;
                var fosFeb = 0;
                var fosMar = 0;
                var fosAbr = 0;
                var fosMay = 0;
                var fosJun = 0;
                var fosJul = 0;
                var fosAgo = 0;
                var fosSep = 0;
                var fosOct = 0;
                var fosNov = 0;
                var fosDic = 0;

                var fosunabEne = 0;
                var fosunabFeb = 0;
                var fosunabMar = 0;
                var fosunabAbr = 0;
                var fosunabMay = 0;
                var fosunabJun = 0;
                var fosunabJul = 0;
                var fosunabAgo = 0;
                var fosunabSep = 0;
                var fosunabOct = 0;
                var fosunabNov = 0;
                var fosunabDic = 0;
                foreach (var item in datos)
                {
                    if (item.Empresa == "1000")
                    {
                        if (item.FechaIngreso.Value.Month == 1)
                        {
                            fosEne += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 2)
                        {
                            fosFeb += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 3)
                        {
                            fosMar += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 4)
                        {
                            fosAbr += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 5)
                        {
                            fosMay += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 6)
                        {
                            fosJun += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 7)
                        {
                            fosJul += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 8)
                        {
                            fosAgo += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 9)
                        {
                            fosSep += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 10)
                        {
                            fosOct += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 11)
                        {
                            fosNov += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 12)
                        {
                            fosDic += 1;
                        }
                    }
                    else if (item.Empresa == "2000")
                    {
                        if (item.FechaIngreso.Value.Month == 1)
                        {
                            fosunabEne += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 2)
                        {
                            fosunabFeb += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 3)
                        {
                            fosunabMar += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 4)
                        {
                            fosunabAbr += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 5)
                        {
                            fosunabMay += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 6)
                        {
                            fosunabJun += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 7)
                        {
                            fosunabJul += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 8)
                        {
                            fosunabAgo += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 9)
                        {
                            fosunabSep += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 10)
                        {
                            fosunabOct += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 11)
                        {
                            fosunabNov += 1;
                        }
                        else if (item.FechaIngreso.Value.Month == 12)
                        {
                            fosunabDic += 1;
                        }
                    }
                }
                
                if (fosEne > 0 || fosunabEne > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosEne, FOSUNAB = fosunabEne, Mes = "Enero" });
                }
                if (fosFeb > 0 || fosunabFeb > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosFeb, FOSUNAB = fosunabFeb, Mes = "Febrero" });
                }
                if (fosMar > 0 || fosunabMar > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosMar, FOSUNAB = fosunabMar, Mes = "Marzo" });
                }
                if (fosAbr > 0 || fosunabAbr > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosAbr, FOSUNAB = fosunabAbr, Mes = "Abril" });
                }
                if (fosMay > 0 || fosunabMay > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosMay, FOSUNAB = fosunabMay, Mes = "Mayo" });
                }
                if (fosJun > 0 || fosunabJun > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosJun, FOSUNAB = fosunabJun, Mes = "Junio" });
                }
                if (fosJul > 0 || fosunabJul > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosJul, FOSUNAB = fosunabJul, Mes = "Julio" });
                }
                if (fosAgo > 0 || fosunabAgo > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosAgo, FOSUNAB = fosunabAgo, Mes = "Agosto" });
                }
                if (fosSep > 0 || fosunabSep > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosSep, FOSUNAB = fosunabSep, Mes = "Septiembre" });
                }
                if (fosOct > 0 || fosunabOct > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosOct, FOSUNAB = fosunabOct, Mes = "Octubre" });
                }
                if (fosNov > 0 || fosunabNov > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosNov, FOSUNAB = fosunabNov, Mes = "Noviembre" });
                }
                if (fosDic > 0 || fosunabDic > 0)
                {
                    tabla.Add(new TablaIngresosMes { FOS = fosDic, FOSUNAB = fosunabDic, Mes = "Diciembre" });
                }
            }
            else
            {
                foreach (var item in datos)
                {
                    if (item.Empresa == "1000")
                    {
                        fos += 1;
                    }
                    else if (item.Empresa == "2000")
                    {
                        fosunab += 1;
                    }
                }
                Mes = Convert.ToDateTime(FechaInicio).ToString("MMMM");
                tabla.Add(new TablaIngresosMes { FOS = fos, FOSUNAB = fosunab, Mes = Mes });
            }
            return tabla;
        }

        public async Task<List<TablaIndicadorRotacionEgresosPorArea>> ObtenerIndicadorRotacionEgresosPorArea(string FInicio, string FFin,string Id, bool EsJefe, string Area)
        {
            List<TablaIndicadorRotacionEgresosPorArea> tabla = new List<TablaIndicadorRotacionEgresosPorArea>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            DateTime? FI = null;
            DateTime? FF = null;
            var AñoActual = DateTime.Now.Year;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
                AñoActual = FF.Value.Year;
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }

                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.Jefe == empleado.NroEmpleado && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null).ToListAsync();

                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Activo = item.Activo, Empresa = item.Empresa, FechaFin = item.FechaFin, FechaIngreso = item.FechaIngreso });
                    }
                }
            }

            foreach (var i in AreasJefe)
            {
                //Egresos por Area
                int enero = 0;
                int febrero = 0;
                int marzo = 0;
                int abril = 0;
                int mayo = 0;
                int junio = 0;
                int julio = 0;
                int agosto = 0;
                int septiembre = 0;
                int octubre = 0;
                int noviembre = 0;
                int diciembre = 0;
                foreach (var item in DatosPorArea)
                {
                    if (item.Activo == "NO" && item.Area == i.Key)
                        {
                            if (item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-01-{AñoActual}"))
                            {
                                enero += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-02-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"28-02-{AñoActual}"))
                            {
                                febrero += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-03-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-03-{AñoActual}"))
                            {
                                marzo += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-04-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-04-{AñoActual}"))
                            {
                                abril += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-05-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-05-{AñoActual}"))
                            {
                                mayo += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-06-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-06-{AñoActual}"))
                            {
                                junio += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-07-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-07-{AñoActual}"))
                            {
                                julio += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-08-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-08-{AñoActual}"))
                            {
                                agosto += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-09-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-09-{AñoActual}"))
                            {
                                septiembre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-10-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-10-{AñoActual}"))
                            {
                                octubre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-11-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-11-{AñoActual}"))
                            {
                                noviembre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-12-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-12-{AñoActual}"))
                            {
                                diciembre += 1;
                            }
                        }
                }
                int Total = enero + febrero + marzo + abril + mayo + junio + julio + agosto + septiembre + octubre + noviembre + diciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorArea { Area = i.Key, Key = "EGRESOS DEL AREA A CARGO", Enero = enero.ToString(), Febrero = febrero.ToString(), Marzo = marzo.ToString(), Abril = abril.ToString(), Mayo = mayo.ToString(), Junio = junio.ToString(), Julio = julio.ToString(), Agosto = agosto.ToString(), Septiembre = septiembre.ToString(), Octubre = octubre.ToString(), Noviembre = noviembre.ToString(), Diciembre = diciembre.ToString(), Total = Total.ToString() });
                      
                //Personal Activo por area
                int Aenero = 0;
                int Afebrero = 0;
                int Amarzo = 0;
                int Aabril = 0;
                int Amayo = 0;
                int Ajunio = 0;
                int Ajulio = 0;
                int Aagosto = 0;
                int Aseptiembre = 0;
                int Aoctubre = 0;
                int Anoviembre = 0;
                int Adiciembre = 0;
                foreach (var item in DatosPorArea)
                {
                    if (item.Activo == "SI" && item.Area == i.Key)
                    {
                        if (/*item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            Aenero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-02-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            Afebrero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-03-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            Amarzo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-04-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            Aabril += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-05-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            Amayo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-06-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            Ajunio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-07-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            Ajulio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-08-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            Aagosto += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-09-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            Aseptiembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-10-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            Aoctubre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-11-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            Anoviembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-12-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            Adiciembre += 1;
                        }
                    }
                }
                Total = Aenero + Afebrero + Amarzo + Aabril + Amayo + Ajunio + Ajulio + Aagosto + Aseptiembre + Aoctubre + Anoviembre + Adiciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorArea { Area = "", Key = "PERSONAL ACTIVO DEL AREA A CARGO", Enero = Aenero.ToString(), Febrero = Afebrero.ToString(), Marzo = Amarzo.ToString(), Abril = Aabril.ToString(), Mayo = Amayo.ToString(), Junio = Ajunio.ToString(), Julio = Ajulio.ToString(), Agosto = Aagosto.ToString(), Septiembre = Aseptiembre.ToString(), Octubre = Aoctubre.ToString(), Noviembre = Anoviembre.ToString(), Diciembre = Adiciembre.ToString(), Total = Total.ToString() });

                //Indicador de Rotacion
                float Cenero = (float)enero * 100 / Aenero;
                var Penero = Cenero.ToString(format: "0.00");

                float Cfebrero = (float)febrero * 100 / Afebrero;
                var Pfebrero = Cfebrero.ToString(format: "0.00");

                float Cmarzo = (float)marzo * 100 / Amarzo;
                var Pmarzo = Cmarzo.ToString(format: "0.00");

                float Cabril = (float)abril * 100 / Aabril;
                var Pabril = Cabril.ToString(format: "0.00");

                float Cmayo = (float)mayo * 100 / Amayo;
                var Pmayo = Cmayo.ToString(format: "0.00");

                float Cjunio = (float)junio * 100 / Ajunio;
                var Pjunio = Cjunio.ToString(format: "0.00");

                float Cjulio = (float)julio * 100 / Ajulio;
                var Pjulio = Cjulio.ToString(format: "0.00");

                float Cagosto = (float)agosto * 100 / Aagosto;
                var Pagosto = Cagosto.ToString(format: "0.00");

                float Cseptiembre = (float)septiembre * 100 / Aseptiembre;
                var Pseptiembre = Cseptiembre.ToString(format: "0.00");

                float Coctubre = (float)octubre * 100 / Aoctubre;
                var Poctubre = Coctubre.ToString(format: "0.00");

                float Cnoviembre = (float)noviembre * 100 / Anoviembre;
                var Pnoviembre = Cnoviembre.ToString(format: "0.00");

                float Cdiciembre = (float)diciembre * 100 / Adiciembre;
                var Pdiciembre = Cdiciembre.ToString(format: "0.00");

                //decimal suma = Convert.ToDecimal(Penero) + Convert.ToDecimal(Pfebrero) + Convert.ToDecimal(Pmarzo) + Convert.ToDecimal(Pabril) + Convert.ToDecimal(Pmayo) + Convert.ToDecimal(Pjunio) + Convert.ToDecimal(Pjulio) + Convert.ToDecimal(Pagosto) + Convert.ToDecimal(Pseptiembre) + Convert.ToDecimal(Pnoviembre) + Convert.ToDecimal(Pdiciembre);
                tabla.Add(new TablaIndicadorRotacionEgresosPorArea { Area = "", Key = "INDICADOR DE ROTACION DEL AREA A CARGO", Enero = Penero, Febrero = Pfebrero.ToString(), Marzo = Pmarzo.ToString(), Abril = Pabril.ToString(), Mayo = Pmayo.ToString(), Junio = Pjunio.ToString(), Julio = Pjulio.ToString(), Agosto = Pagosto.ToString(), Septiembre = Pseptiembre.ToString(), Octubre = Poctubre.ToString(), Noviembre = Pnoviembre.ToString(), Diciembre = Pdiciembre.ToString(), Total = "" });
            }
            return tabla;
        }

        public async Task<List<TablaIndicadorRotacionEgresosPorCargo>> ObtenerIndicadorRotacionEgresosPorCargo(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaIndicadorRotacionEgresosPorCargo> tabla = new List<TablaIndicadorRotacionEgresosPorCargo>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            DateTime? FI = null;
            DateTime? FF = null;
            var AñoActual = DateTime.Now.Year;

            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
                AñoActual = FF.Value.Year;
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);

            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }

                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.Jefe == empleado.NroEmpleado && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                            }
                        }
                    }
                }
            }
            else
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                    }
                }
            }

            List<IGrouping<string, AArea>> CargosDelArea = new List<IGrouping<string, AArea>>();

            if (AreasJefe.Count != 0)
            {
                foreach (var i in AreasJefe)
                {
                    if (Cargo != null && Cargo != "")
                    {
                        var Cargos = db.Empleados.Where(x => x.Area != null && x.Cargo == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                    else
                    {
                        var Cargos = db.Empleados.Where(x => x.Cargo != null && x.Area == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                }
            }
            else
            {
                var Cargos = db.Empleados.Where(x => x.Cargo == Cargo).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                if (Cargo != "")
                {
                    CargosDelArea.AddRange(Cargos);
                }
            }

            foreach (var i in CargosDelArea)
            {
                int enero = 0;
                int febrero = 0;
                int marzo = 0;
                int abril = 0;
                int mayo = 0;
                int junio = 0;
                int julio = 0;
                int agosto = 0;
                int septiembre = 0;
                int octubre = 0;
                int noviembre = 0;
                int diciembre = 0;
                string cargo = i.Key;
                if (i.Key == "" || i.Key == null)
                {
                    cargo = "CARGO DESCONOCIDO";
                }
                foreach (var item in DatosPorArea)
                {
                    if (item.Cargo == i.Key && item.Activo == "NO")
                        {
                            if (item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-01-{AñoActual}"))
                            {
                                enero += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-02-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"28-02-{AñoActual}"))
                            {
                                febrero += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-03-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-03-{AñoActual}"))
                            {
                                marzo += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-04-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-04-{AñoActual}"))
                            {
                                abril += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-05-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-05-{AñoActual}"))
                            {
                                mayo += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-06-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-06-{AñoActual}"))
                            {
                                junio += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-07-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-07-{AñoActual}"))
                            {
                                julio += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-08-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-08-{AñoActual}"))
                            {
                                agosto += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-09-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-09-{AñoActual}"))
                            {
                                septiembre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-10-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-10-{AñoActual}"))
                            {
                                octubre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-11-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-11-{AñoActual}"))
                            {
                                noviembre += 1;
                            }
                            else if (item.FechaFin >= Convert.ToDateTime($"1-12-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-12-{AñoActual}"))
                            {
                                diciembre += 1;
                            }
                        }
                }
                int Total = enero + febrero + marzo + abril + mayo + junio + julio + agosto + septiembre + octubre + noviembre + diciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargo { Cargo = cargo, Area = "TESTING", Key = "EGRESOS POR CARGO DEL AREA A CARGO", Enero = enero.ToString(), Febrero = febrero.ToString(), Marzo = marzo.ToString(), Abril = abril.ToString(), Mayo = mayo.ToString(), Junio = junio.ToString(), Julio = julio.ToString(), Agosto = agosto.ToString(), Septiembre = septiembre.ToString(), Octubre = octubre.ToString(), Noviembre = noviembre.ToString(), Diciembre = diciembre.ToString(), Total = Total.ToString() });

                //Personal Activo por area
                int Aenero = 0;
                int Afebrero = 0;
                int Amarzo = 0;
                int Aabril = 0;
                int Amayo = 0;
                int Ajunio = 0;
                int Ajulio = 0;
                int Aagosto = 0;
                int Aseptiembre = 0;
                int Aoctubre = 0;
                int Anoviembre = 0;
                int Adiciembre = 0;

                foreach (var item in DatosPorArea)
                {
                    if (item.Cargo == i.Key && item.Activo == "SI")
                    {
                        if (/*item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            Aenero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-02-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            Afebrero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-03-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            Amarzo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-04-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            Aabril += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-05-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            Amayo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-06-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            Ajunio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-07-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            Ajulio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-08-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            Aagosto += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-09-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            Aseptiembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-10-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            Aoctubre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-11-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            Anoviembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-12-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            Adiciembre += 1;
                        }
                    }
                }
                Total = Aenero + Afebrero + Amarzo + Aabril + Amayo + Ajunio + Ajulio + Aagosto + Aseptiembre + Aoctubre + Anoviembre + Adiciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargo { Cargo = "", Area = "TESTING", Key = "PERSONAL ACTIVO DEL AREA A CARGO", Enero = Aenero.ToString(), Febrero = Afebrero.ToString(), Marzo = Amarzo.ToString(), Abril = Aabril.ToString(), Mayo = Amayo.ToString(), Junio = Ajunio.ToString(), Julio = Ajulio.ToString(), Agosto = Aagosto.ToString(), Septiembre = Aseptiembre.ToString(), Octubre = Aoctubre.ToString(), Noviembre = Anoviembre.ToString(), Diciembre = Adiciembre.ToString(), Total = Total.ToString() });

                //Indicador de Rotacion
                float Cenero = (float)enero * 100 / Aenero;
                var Penero = Cenero.ToString(format: "0.00");

                float Cfebrero = (float)febrero * 100 / Afebrero;
                var Pfebrero = Cfebrero.ToString(format: "0.00");

                float Cmarzo = (float)marzo * 100 / Amarzo;
                var Pmarzo = Cmarzo.ToString(format: "0.00");

                float Cabril = (float)abril * 100 / Aabril;
                var Pabril = Cabril.ToString(format: "0.00");

                float Cmayo = (float)mayo * 100 / Amayo;
                var Pmayo = Cmayo.ToString(format: "0.00");

                float Cjunio = (float)junio * 100 / Ajunio;
                var Pjunio = Cjunio.ToString(format: "0.00");

                float Cjulio = (float)julio * 100 / Ajulio;
                var Pjulio = Cjulio.ToString(format: "0.00");

                float Cagosto = (float)agosto * 100 / Aagosto;
                var Pagosto = Cagosto.ToString(format: "0.00");

                float Cseptiembre = (float)septiembre * 100 / Aseptiembre;
                var Pseptiembre = Cseptiembre.ToString(format: "0.00");

                float Coctubre = (float)octubre * 100 / Aoctubre;
                var Poctubre = Coctubre.ToString(format: "0.00");

                float Cnoviembre = (float)noviembre * 100 / Anoviembre;
                var Pnoviembre = Cnoviembre.ToString(format: "0.00");

                float Cdiciembre = (float)diciembre * 100 / Adiciembre;
                var Pdiciembre = Cdiciembre.ToString(format: "0.00");

                //decimal suma = Convert.ToDecimal(Penero) + Convert.ToDecimal(Pfebrero) + Convert.ToDecimal(Pmarzo) + Convert.ToDecimal(Pabril) + Convert.ToDecimal(Pmayo) + Convert.ToDecimal(Pjunio) + Convert.ToDecimal(Pjulio) + Convert.ToDecimal(Pagosto) + Convert.ToDecimal(Pseptiembre) + Convert.ToDecimal(Pnoviembre) + Convert.ToDecimal(Pdiciembre);
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargo { Cargo = "", Area = "TESTING", Key = "INDICADOR DE ROTACIÓN POR CARGO DEL AREA A CARGO", Enero = Penero, Febrero = Pfebrero.ToString(), Marzo = Pmarzo.ToString(), Abril = Pabril.ToString(), Mayo = Pmayo.ToString(), Junio = Pjunio.ToString(), Julio = Pjulio.ToString(), Agosto = Pagosto.ToString(), Septiembre = Pseptiembre.ToString(), Octubre = Poctubre.ToString(), Noviembre = Pnoviembre.ToString(), Diciembre = Pdiciembre.ToString(), Total = "" });
            }
            return tabla;
        }

        public async Task<List<TablaRotacionPorCargo>> ObtenerRotacionPorCargo(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaRotacionPorCargo> tabla = new List<TablaRotacionPorCargo>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);

            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                            }
                        }
                    }
                }
            }
            else
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Empresa == "1000" && x.Area != null).ToListAsync();

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                    }
                }
            }

            List<IGrouping<string, AArea>> CargosDelArea = new List<IGrouping<string, AArea>>();

            if (AreasJefe.Count != 0)
            {
                foreach (var i in AreasJefe)
                {
                    if (Cargo != null && Cargo != "")
                    {
                        var Cargos = db.Empleados.Where(x => x.Area != null && x.Cargo == i.Key).Select(x => new AArea { Area = x.Cargo, Area2 = i.Key }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                    else
                    {
                        var Cargos = db.Empleados.Where(x => x.Cargo != null && x.Area == i.Key).Select(x => new AArea { Area = x.Cargo, Area2 = i.Key }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                }
            }
            else
            {
                var Cargos = db.Empleados.Where(x => x.Cargo == Cargo).Select(x => new AArea { Area = x.Cargo, Area2 = x.Area }).GroupBy(b => b.Area).ToList();
                if (Cargo != "")
                {
                    CargosDelArea.AddRange(Cargos);
                }
            }

            foreach (var item in CargosDelArea)
            {
                int nro = 0;
                int retiros = 0;

                foreach (var i in DatosPorArea)
                {
                    if (i.Cargo == item.Key && i.Activo == "SI")
                    {
                        nro += 1;
                    }
                    else if (i.Cargo == item.Key && i.Activo == "NO")
                    {
                        retiros += 1;
                    }
                }
                float calculo = (float)retiros * 100 / nro;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaRotacionPorCargo { Cargo = item.Key, Area = item.First().Area2, NroPersonas = nro, Retiros = retiros, Porcentaje = Porcentaje });
            }
            return tabla;
        }

        public async Task<List<TablaIndicadorRotacionEgresosPorAreaFosunab>> ObtenerIndicadorRotacionEgresosPorAreaFosunab(string FInicio, string FFin, string Id, bool EsJefe, string Area)
        {
            List<TablaIndicadorRotacionEgresosPorAreaFosunab> tabla = new List<TablaIndicadorRotacionEgresosPorAreaFosunab>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            DateTime? FI = null;
            DateTime? FF = null;
            var AñoActual = DateTime.Now.Year;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
                AñoActual = FF.Value.Year;
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }

                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.Jefe == empleado.NroEmpleado && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null).ToListAsync();

                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Activo = item.Activo, Empresa = item.Empresa, FechaFin = item.FechaFin, FechaIngreso = item.FechaIngreso });
                    }
                }
            }

            foreach (var i in AreasJefe)
            {
                //Egresos por Area
                int enero = 0;
                int febrero = 0;
                int marzo = 0;
                int abril = 0;
                int mayo = 0;
                int junio = 0;
                int julio = 0;
                int agosto = 0;
                int septiembre = 0;
                int octubre = 0;
                int noviembre = 0;
                int diciembre = 0;
                foreach (var item in DatosPorArea)
                {
                    if (item.Activo == "NO" && item.Area == i.Key)
                    {
                        if (item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            enero += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-02-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            febrero += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-03-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            marzo += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-04-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            abril += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-05-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            mayo += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-06-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            junio += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-07-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            julio += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-08-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            agosto += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-09-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            septiembre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-10-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            octubre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-11-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            noviembre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-12-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            diciembre += 1;
                        }
                    }
                }
                int Total = enero + febrero + marzo + abril + mayo + junio + julio + agosto + septiembre + octubre + noviembre + diciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorAreaFosunab { Area = i.Key, Key = "EGRESOS DEL AREA A CARGO", Enero = enero.ToString(), Febrero = febrero.ToString(), Marzo = marzo.ToString(), Abril = abril.ToString(), Mayo = mayo.ToString(), Junio = junio.ToString(), Julio = julio.ToString(), Agosto = agosto.ToString(), Septiembre = septiembre.ToString(), Octubre = octubre.ToString(), Noviembre = noviembre.ToString(), Diciembre = diciembre.ToString(), Total = Total.ToString() });

                //Personal Activo por area
                int Aenero = 0;
                int Afebrero = 0;
                int Amarzo = 0;
                int Aabril = 0;
                int Amayo = 0;
                int Ajunio = 0;
                int Ajulio = 0;
                int Aagosto = 0;
                int Aseptiembre = 0;
                int Aoctubre = 0;
                int Anoviembre = 0;
                int Adiciembre = 0;
                foreach (var item in DatosPorArea)
                {
                    if (item.Activo == "SI" && item.Area == item.Area)
                    {
                        if (/*item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            Aenero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-02-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            Afebrero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-03-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            Amarzo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-04-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            Aabril += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-05-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            Amayo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-06-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            Ajunio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-07-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            Ajulio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-08-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            Aagosto += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-09-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            Aseptiembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-10-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            Aoctubre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-11-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            Anoviembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-12-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            Adiciembre += 1;
                        }
                    }
                }
                Total = Aenero + Afebrero + Amarzo + Aabril + Amayo + Ajunio + Ajulio + Aagosto + Aseptiembre + Aoctubre + Anoviembre + Adiciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorAreaFosunab { Area = "", Key = "PERSONAL ACTIVO DEL AREA A CARGO", Enero = Aenero.ToString(), Febrero = Afebrero.ToString(), Marzo = Amarzo.ToString(), Abril = Aabril.ToString(), Mayo = Amayo.ToString(), Junio = Ajunio.ToString(), Julio = Ajulio.ToString(), Agosto = Aagosto.ToString(), Septiembre = Aseptiembre.ToString(), Octubre = Aoctubre.ToString(), Noviembre = Anoviembre.ToString(), Diciembre = Adiciembre.ToString(), Total = Total.ToString() });

                //Indicador de Rotacion
                float Cenero = (float)enero * 100 / Aenero;
                var Penero = Cenero.ToString(format: "0.00");

                float Cfebrero = (float)febrero * 100 / Afebrero;
                var Pfebrero = Cfebrero.ToString(format: "0.00");

                float Cmarzo = (float)marzo * 100 / Amarzo;
                var Pmarzo = Cmarzo.ToString(format: "0.00");

                float Cabril = (float)abril * 100 / Aabril;
                var Pabril = Cabril.ToString(format: "0.00");

                float Cmayo = (float)mayo * 100 / Amayo;
                var Pmayo = Cmayo.ToString(format: "0.00");

                float Cjunio = (float)junio * 100 / Ajunio;
                var Pjunio = Cjunio.ToString(format: "0.00");

                float Cjulio = (float)julio * 100 / Ajulio;
                var Pjulio = Cjulio.ToString(format: "0.00");

                float Cagosto = (float)agosto * 100 / Aagosto;
                var Pagosto = Cagosto.ToString(format: "0.00");

                float Cseptiembre = (float)septiembre * 100 / Aseptiembre;
                var Pseptiembre = Cseptiembre.ToString(format: "0.00");

                float Coctubre = (float)octubre * 100 / Aoctubre;
                var Poctubre = Coctubre.ToString(format: "0.00");

                float Cnoviembre = (float)noviembre * 100 / Anoviembre;
                var Pnoviembre = Cnoviembre.ToString(format: "0.00");

                float Cdiciembre = (float)diciembre * 100 / Adiciembre;
                var Pdiciembre = Cdiciembre.ToString(format: "0.00");

                //decimal suma = Convert.ToDecimal(Penero) + Convert.ToDecimal(Pfebrero) + Convert.ToDecimal(Pmarzo) + Convert.ToDecimal(Pabril) + Convert.ToDecimal(Pmayo) + Convert.ToDecimal(Pjunio) + Convert.ToDecimal(Pjulio) + Convert.ToDecimal(Pagosto) + Convert.ToDecimal(Pseptiembre) + Convert.ToDecimal(Pnoviembre) + Convert.ToDecimal(Pdiciembre);
                tabla.Add(new TablaIndicadorRotacionEgresosPorAreaFosunab { Area = "", Key = "INDICADOR DE ROTACION DEL AREA A CARGO", Enero = Penero, Febrero = Pfebrero.ToString(), Marzo = Pmarzo.ToString(), Abril = Pabril.ToString(), Mayo = Pmayo.ToString(), Junio = Pjunio.ToString(), Julio = Pjulio.ToString(), Agosto = Pagosto.ToString(), Septiembre = Pseptiembre.ToString(), Octubre = Poctubre.ToString(), Noviembre = Pnoviembre.ToString(), Diciembre = Pdiciembre.ToString(), Total = "" });
            }
            return tabla;
        }

        public async Task<List<TablaIndicadorRotacionEgresosPorCargoFosunab>> ObtenerIndicadorRotacionEgresosPorCargoFosunab(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaIndicadorRotacionEgresosPorCargoFosunab> tabla = new List<TablaIndicadorRotacionEgresosPorCargoFosunab>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            DateTime? FI = null;
            DateTime? FF = null;
            var AñoActual = DateTime.Now.Year;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
                AñoActual = FF.Value.Year;
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);

            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }

                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.Jefe == empleado.NroEmpleado && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                            }
                        }
                    }
                }
            }
            else
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.FechaFin >= FI && x.FechaFin <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                    }
                }
            }

            List<IGrouping<string, AArea>> CargosDelArea = new List<IGrouping<string, AArea>>();

            if (AreasJefe.Count != 0)
            {
                foreach (var i in AreasJefe)
                {
                    if (Cargo != null && Cargo != "")
                    {
                        var Cargos = db.Empleados.Where(x => x.Area != null && x.Cargo == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                    else
                    {
                        var Cargos = db.Empleados.Where(x => x.Cargo != null && x.Area == i.Key).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                }
            }
            else
            {
                var Cargos = db.Empleados.Where(x => x.Cargo == Cargo).Select(x => new AArea { Area = x.Cargo }).GroupBy(b => b.Area).ToList();
                if (Cargo != "")
                {
                    CargosDelArea.AddRange(Cargos);
                }
            }


            foreach (var i in CargosDelArea)
            {
                int enero = 0;
                int febrero = 0;
                int marzo = 0;
                int abril = 0;
                int mayo = 0;
                int junio = 0;
                int julio = 0;
                int agosto = 0;
                int septiembre = 0;
                int octubre = 0;
                int noviembre = 0;
                int diciembre = 0;
                string cargo = i.Key;
                if (i.Key == "" || i.Key == null)
                {
                    cargo = "CARGO DESCONOCIDO";
                }
                foreach (var item in DatosPorArea)
                {
                    if (item.Cargo == i.Key && item.Activo == "NO")
                    {
                        if (item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            enero += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-02-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            febrero += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-03-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            marzo += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-04-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            abril += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-05-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            mayo += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-06-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            junio += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-07-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            julio += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-08-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            agosto += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-09-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            septiembre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-10-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            octubre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-11-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            noviembre += 1;
                        }
                        else if (item.FechaFin >= Convert.ToDateTime($"1-12-{AñoActual}") && item.FechaFin <= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            diciembre += 1;
                        }
                    }
                }
                int Total = enero + febrero + marzo + abril + mayo + junio + julio + agosto + septiembre + octubre + noviembre + diciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargoFosunab { Cargo = cargo, Area = "TESTING", Key = "EGRESOS POR CARGO DEL AREA A CARGO", Enero = enero.ToString(), Febrero = febrero.ToString(), Marzo = marzo.ToString(), Abril = abril.ToString(), Mayo = mayo.ToString(), Junio = junio.ToString(), Julio = julio.ToString(), Agosto = agosto.ToString(), Septiembre = septiembre.ToString(), Octubre = octubre.ToString(), Noviembre = noviembre.ToString(), Diciembre = diciembre.ToString(), Total = Total.ToString() });

                //Personal Activo por area
                int Aenero = 0;
                int Afebrero = 0;
                int Amarzo = 0;
                int Aabril = 0;
                int Amayo = 0;
                int Ajunio = 0;
                int Ajulio = 0;
                int Aagosto = 0;
                int Aseptiembre = 0;
                int Aoctubre = 0;
                int Anoviembre = 0;
                int Adiciembre = 0;

                foreach (var item in DatosPorArea)
                {
                    if (item.Cargo == i.Key && item.Activo == "SI")
                    {
                        if (/*item.FechaFin >= Convert.ToDateTime($"1-01-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-01-{AñoActual}"))
                        {
                            Aenero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-02-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"28-02-{AñoActual}"))
                        {
                            Afebrero += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-03-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-03-{AñoActual}"))
                        {
                            Amarzo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-04-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-04-{AñoActual}"))
                        {
                            Aabril += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-05-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-05-{AñoActual}"))
                        {
                            Amayo += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-06-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-06-{AñoActual}"))
                        {
                            Ajunio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-07-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-07-{AñoActual}"))
                        {
                            Ajulio += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-08-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-08-{AñoActual}"))
                        {
                            Aagosto += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-09-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-09-{AñoActual}"))
                        {
                            Aseptiembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-10-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-10-{AñoActual}"))
                        {
                            Aoctubre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-11-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"30-11-{AñoActual}"))
                        {
                            Anoviembre += 1;
                        }
                        if (/*item.FechaIngreso >= Convert.ToDateTime($"1-12-{AñoActual}") &&*/ item.FechaFin >= Convert.ToDateTime($"31-12-{AñoActual}"))
                        {
                            Adiciembre += 1;
                        }
                    }
                }
                Total = Aenero + Afebrero + Amarzo + Aabril + Amayo + Ajunio + Ajulio + Aagosto + Aseptiembre + Aoctubre + Anoviembre + Adiciembre;
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargoFosunab { Cargo = "", Area = "TESTING", Key = "PERSONAL ACTIVO DEL AREA A CARGO", Enero = Aenero.ToString(), Febrero = Afebrero.ToString(), Marzo = Amarzo.ToString(), Abril = Aabril.ToString(), Mayo = Amayo.ToString(), Junio = Ajunio.ToString(), Julio = Ajulio.ToString(), Agosto = Aagosto.ToString(), Septiembre = Aseptiembre.ToString(), Octubre = Aoctubre.ToString(), Noviembre = Anoviembre.ToString(), Diciembre = Adiciembre.ToString(), Total = Total.ToString() });

                //Indicador de Rotacion
                float Cenero = (float)enero * 100 / Aenero;
                var Penero = Cenero.ToString(format: "0.00");

                float Cfebrero = (float)febrero * 100 / Afebrero;
                var Pfebrero = Cfebrero.ToString(format: "0.00");

                float Cmarzo = (float)marzo * 100 / Amarzo;
                var Pmarzo = Cmarzo.ToString(format: "0.00");

                float Cabril = (float)abril * 100 / Aabril;
                var Pabril = Cabril.ToString(format: "0.00");

                float Cmayo = (float)mayo * 100 / Amayo;
                var Pmayo = Cmayo.ToString(format: "0.00");

                float Cjunio = (float)junio * 100 / Ajunio;
                var Pjunio = Cjunio.ToString(format: "0.00");

                float Cjulio = (float)julio * 100 / Ajulio;
                var Pjulio = Cjulio.ToString(format: "0.00");

                float Cagosto = (float)agosto * 100 / Aagosto;
                var Pagosto = Cagosto.ToString(format: "0.00");

                float Cseptiembre = (float)septiembre * 100 / Aseptiembre;
                var Pseptiembre = Cseptiembre.ToString(format: "0.00");

                float Coctubre = (float)octubre * 100 / Aoctubre;
                var Poctubre = Coctubre.ToString(format: "0.00");

                float Cnoviembre = (float)noviembre * 100 / Anoviembre;
                var Pnoviembre = Cnoviembre.ToString(format: "0.00");

                float Cdiciembre = (float)diciembre * 100 / Adiciembre;
                var Pdiciembre = Cdiciembre.ToString(format: "0.00");

                //decimal suma = Convert.ToDecimal(Penero) + Convert.ToDecimal(Pfebrero) + Convert.ToDecimal(Pmarzo) + Convert.ToDecimal(Pabril) + Convert.ToDecimal(Pmayo) + Convert.ToDecimal(Pjunio) + Convert.ToDecimal(Pjulio) + Convert.ToDecimal(Pagosto) + Convert.ToDecimal(Pseptiembre) + Convert.ToDecimal(Pnoviembre) + Convert.ToDecimal(Pdiciembre);
                tabla.Add(new TablaIndicadorRotacionEgresosPorCargoFosunab { Cargo = "", Area = "TESTING", Key = "INDICADOR DE ROTACIÓN POR CARGO DEL AREA A CARGO", Enero = Penero, Febrero = Pfebrero.ToString(), Marzo = Pmarzo.ToString(), Abril = Pabril.ToString(), Mayo = Pmayo.ToString(), Junio = Pjunio.ToString(), Julio = Pjulio.ToString(), Agosto = Pagosto.ToString(), Septiembre = Pseptiembre.ToString(), Octubre = Poctubre.ToString(), Noviembre = Pnoviembre.ToString(), Diciembre = Pdiciembre.ToString(), Total = "" });
            }
            return tabla;
        }

        public async Task<List<TablaRotacionPorCargoFosunab>> ObtenerRotacionPorCargoFosunab(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaRotacionPorCargoFosunab> tabla = new List<TablaRotacionPorCargoFosunab>();
            List<Empleado> t;
            List<DatosPorArea> DatosPorArea = new List<DatosPorArea>();

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);

            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();
            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                DatosPorArea.Add(new DatosPorArea { Area = i.Key, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                            }
                        }
                    }
                }
            }
            else
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Empresa == "2000" && x.Area != null).ToListAsync();

                foreach (var item in t)
                {
                    if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else if (Cargo != null && Cargo != "")
                    {
                        if (item.Cargo == Cargo)
                        {
                            DatosPorArea.Add(new DatosPorArea { Area = Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                        }
                    }
                    else
                    {
                        DatosPorArea.Add(new DatosPorArea { Area = item.Area, Empresa = item.Empresa, FechaFin = item.FechaFin, Activo = item.Activo, FechaIngreso = item.FechaIngreso, Cargo = item.Cargo });
                    }
                }
            }

            List<IGrouping<string, AArea>> CargosDelArea = new List<IGrouping<string, AArea>>();

            if (AreasJefe.Count != 0)
            {
                foreach (var i in AreasJefe)
                {
                    if (Cargo != null && Cargo != "")
                    {
                        var Cargos = db.Empleados.Where(x => x.Area != null && x.Cargo == i.Key).Select(x => new AArea { Area = x.Cargo, Area2 = i.Key }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                    else
                    {
                        var Cargos = db.Empleados.Where(x => x.Cargo != null && x.Area == i.Key).Select(x => new AArea { Area = x.Cargo, Area2 = i.Key }).GroupBy(b => b.Area).ToList();
                        if (Cargos != null)
                        {
                            CargosDelArea.AddRange(Cargos);
                        }
                    }
                }
            }
            else
            {
                var Cargos = db.Empleados.Where(x => x.Cargo == Cargo).Select(x => new AArea { Area = x.Cargo, Area2 = x.Area }).GroupBy(b => b.Area).ToList();
                if (Cargo != "")
                {
                    CargosDelArea.AddRange(Cargos);
                }
            }

            foreach (var item in CargosDelArea)
            {
                int nro = 0;
                int retiros = 0;

                foreach (var i in DatosPorArea)
                {
                    if (i.Cargo == item.Key && i.Activo == "SI")
                    {
                        nro += 1;
                    }
                    else if (i.Cargo == item.Key && i.Activo == "NO")
                    {
                        retiros += 1;
                    }
                }
                float calculo = (float)retiros * 100 / nro;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaRotacionPorCargoFosunab { Cargo = item.Key, Area = item.First().Area2, NroPersonas = nro, Retiros = retiros, Porcentaje = Porcentaje });
            }
            return tabla;
        }

        public async Task<List<TablaGeneroEmpleadosEgreso>> ObtenerGeneroEmpleadosEgreso(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaGeneroEmpleadosEgreso> tabla = new List<TablaGeneroEmpleadosEgreso>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            DateTime? FI = null;
            DateTime? FF = null;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(e => e.Activo == "SI" && e.FechaIngreso >= FI && e.FechaIngreso <= FF && e.Jefe == empleado.NroEmpleado).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Activo == "SI" && x.Jefe == empleado.NroEmpleado).ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    t = await db.Empleados.Where(e => e.Activo == "SI" && e.FechaIngreso >= FI && e.FechaIngreso <= FF).ToListAsync();
                }
                else
                {
                    t = await db.Empleados.Where(x => x.Activo == "SI").ToListAsync();
                }

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int M = 0;
            int F = 0;
            //Fosunab
            int M2 = 0;
            int F2 = 0;
            foreach (var item in datos)
            {
                if (item.Empresa == "1000")
                {
                    if (item.Genero == "1")
                    {
                        M += 1;
                    }
                    else if (item.Genero == "2")
                    {
                        F += 1;
                    }
                }
                else if (item.Empresa == "2000")
                {
                    if (item.Genero == "1")
                    {
                        M2 += 1;
                    }
                    else if (item.Genero == "2")
                    {
                        F2 += 1;
                    }
                }
            }
            int Total = M + F;
            Rangos.Add(new KeyValuePair<string, int>("Femenino", F));
            Rangos.Add(new KeyValuePair<string, int>("Masculino", M));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));

            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaGeneroEmpleadosEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = M2 + F2;
            Rangos2.Add(new KeyValuePair<string, int>("Femenino", F2));
            Rangos2.Add(new KeyValuePair<string, int>("Masculino", M2));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaGeneroEmpleadosEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaEdadesEmpleadosEgreso>> ObtenerEdadesEmpleadosEgreso(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaEdadesEmpleadosEgreso> tabla = new List<TablaEdadesEmpleadosEgreso>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "NO" && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                t = await db.Empleados.Where(x => x.Activo == "NO").ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int rango1 = 0;
            int rango2 = 0;
            int rango3 = 0;
            int rango4 = 0;
            int rango5 = 0;
            //Fosunab
            int rango6 = 0;
            int rango7 = 0;
            int rango8 = 0;
            int rango9 = 0;
            int rango10 = 0;

            foreach (var item in datos)
            {
                DateTime FechaNac = Convert.ToDateTime(item.FechaNacimiento);
                var Nac = int.Parse(FechaNac.ToString("yyyyMMdd"));
                var FechaActual = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                var Edad = (FechaActual - Nac) / 10000;
                if (item.Empresa == "1000")
                {
                    if (Edad >= 18 && Edad <= 25)
                    {
                        rango1 += 1;
                    }
                    else if (Edad >= 26 && Edad <= 35)
                    {
                        rango2 += 1;
                    }
                    else if (Edad >= 36 && Edad <= 45)
                    {
                        rango3 += 1;
                    }
                    else if (Edad >= 46 && Edad <= 55)
                    {
                        rango4 += 1;
                    }
                    else if (Edad >= 56 && Edad <= 120)
                    {
                        rango5 += 1;
                    }
                }
                else if (item.Empresa == "2000")
                {
                    if (Edad >= 18 && Edad <= 25)
                    {
                        rango6 += 1;
                    }
                    else if (Edad >= 26 && Edad <= 35)
                    {
                        rango7 += 1;
                    }
                    else if (Edad >= 36 && Edad <= 45)
                    {
                        rango8 += 1;
                    }
                    else if (Edad >= 46 && Edad <= 55)
                    {
                        rango9 += 1;
                    }
                    else if (Edad >= 56 && Edad <= 120)
                    {
                        rango10 += 1;
                    }
                }
            }
            int Total = rango1 + rango2 + rango3 + rango4 + rango5;
            Rangos.Add(new KeyValuePair<string, int>("18 a 25 años", rango1));
            Rangos.Add(new KeyValuePair<string, int>("26 a 35 años", rango2));
            Rangos.Add(new KeyValuePair<string, int>("36 a 45 años", rango3));
            Rangos.Add(new KeyValuePair<string, int>("46 a 55 años", rango4));
            Rangos.Add(new KeyValuePair<string, int>("Mayores 56 años", rango5));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaEdadesEmpleadosEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = rango6 + rango7 + rango8 + rango9 + rango10;
            Rangos2.Add(new KeyValuePair<string, int>("18 a 25 años", rango6));
            Rangos2.Add(new KeyValuePair<string, int>("26 a 35 años", rango7));
            Rangos2.Add(new KeyValuePair<string, int>("36 a 45 años", rango8));
            Rangos2.Add(new KeyValuePair<string, int>("46 a 55 años", rango9));
            Rangos2.Add(new KeyValuePair<string, int>("Mayores 56 años", rango10));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaEdadesEmpleadosEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaTiempoPermanenciaEgreso>> ObtenerTiempoPermanenciaEgreso(string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaTiempoPermanenciaEgreso> tabla = new List<TablaTiempoPermanenciaEgreso>();
            List<Empleado> t;
            List<Empleado> datos = new List<Empleado>();
            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }
            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                t = await db.Empleados.Where(x => x.Activo == "NO" && x.Jefe == empleado.NroEmpleado).ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var i in AreasJefe)
                        {
                            if (item.Area == i.Key)
                            {
                                datos.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                t = await db.Empleados.Where(x => x.Activo == "NO").ToListAsync();

                foreach (var item in t)
                {
                    if (Cargo != "" && Cargo != null)
                    {
                        if (item.Cargo == Cargo)
                        {
                            datos.Add(item);
                        }
                    }
                    else if (Area != null && Area != "")
                    {
                        if (item.Area == Area)
                        {
                            datos.Add(item);
                        }
                    }
                    else
                    {
                        datos.Add(item);
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int rango1 = 0;
            int rango2 = 0;
            int rango3 = 0;
            int rango4 = 0;
            int rango5 = 0;
            int rango6 = 0;
            //Fosunab
            int rango7 = 0;
            int rango8 = 0;
            int rango9 = 0;
            int rango10 = 0;
            int rango11 = 0;
            int rango12 = 0;

            foreach (var item in datos)
            {
                DateTime FechaIngreso = Convert.ToDateTime(item.FechaIngreso);
                var Ingreso = int.Parse(FechaIngreso.ToString("yyyyMMdd"));
                DateTime FechaEgreso = Convert.ToDateTime(item.FechaFin);
                var Egreso = int.Parse(FechaEgreso.ToString("yyyyMMdd"));
                float Mes = (float)(Egreso - Ingreso) / 10000;
                int Edad = (Egreso - Ingreso) / 10000;

                if (item.Empresa == "1000")
                {
                    if (Mes <= 0.04)
                    {
                        rango1 += 1;
                    }
                    else if (Mes > 0.04 && Mes < 1)
                    {
                        rango2 += 1;
                    }
                    else if (Edad >= 1 && Edad <= 2)
                    {
                        rango3 += 1;
                    }
                    else if (Edad >= 3 && Edad <= 5)
                    {
                        rango4 += 1;
                    }
                    else if (Edad >= 6 && Edad <= 9)
                    {
                        rango5 += 1;
                    }
                    else if (Edad >= 10)
                    {
                        rango6 += 1;
                    }
                }
                else if (item.Empresa == "2000")
                {
                    if (Mes <= 0.04)
                    {
                        rango7 += 1;
                    }
                    else if (Mes > 0.04 && Mes < 1)
                    {
                        rango8 += 1;
                    }
                    else if (Edad >= 1 && Edad <= 2)
                    {
                        rango9 += 1;
                    }
                    else if (Edad >= 3 && Edad <= 5)
                    {
                        rango10 += 1;
                    }
                    else if (Edad >= 6 && Edad <= 9)
                    {
                        rango11 += 1;
                    }
                    else if (Edad >= 10)
                    {
                        rango12 += 1;
                    }
                }
            }
            int Total = rango1 + rango2 + rango3 + rango4 + rango5 + rango6;
            Rangos.Add(new KeyValuePair<string, int>("Menor o Igual a 4 meses", rango1));
            Rangos.Add(new KeyValuePair<string, int>("De 4 a 12 Meses", rango2));
            Rangos.Add(new KeyValuePair<string, int>("De 1 a 2 años", rango3));
            Rangos.Add(new KeyValuePair<string, int>("De 3 a 5 años", rango4));
            Rangos.Add(new KeyValuePair<string, int>("De 6 a 9 años", rango5));
            Rangos.Add(new KeyValuePair<string, int>("Mayor o igual a 10 años", rango6));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));

            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaTiempoPermanenciaEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = rango7 + rango8 + rango9 + rango10 + rango11 + rango12;
            Rangos2.Add(new KeyValuePair<string, int>("Menor o Igual a 4 meses", rango7));
            Rangos2.Add(new KeyValuePair<string, int>("De 4 a 12 Meses", rango8));
            Rangos2.Add(new KeyValuePair<string, int>("De 1 a 2 años", rango9));
            Rangos2.Add(new KeyValuePair<string, int>("De 3 a 5 años", rango10));
            Rangos2.Add(new KeyValuePair<string, int>("De 6 a 9 años", rango11));
            Rangos2.Add(new KeyValuePair<string, int>("Mayor o igual a 10 años", rango12));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaTiempoPermanenciaEgreso { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }

        public async Task<List<TablaMotivoRenuncia>> ObtenerMotivoRenuncia(string FInicio, string FFin, string Id, bool EsJefe, string Area, string Cargo)
        {
            List<TablaMotivoRenuncia> tabla = new List<TablaMotivoRenuncia>();
            List<Retiros> p;
            List<Empleado> t;
            List<Retiros> datos = new List<Retiros>();
            DateTime? FI = null;
            DateTime? FF = null;
            if (FInicio != "" && FFin != "" && FInicio != null && FFin != null)
            {
                FI = Convert.ToDateTime(FInicio);
                FF = Convert.ToDateTime(FFin);
            }

            int.TryParse(Id, out int id);
            var empleado = db.Empleados.Find(id);
            List<IGrouping<string, AArea>> AreasJefe = new List<IGrouping<string, AArea>>();

            if (Area != null && Area != "")
            {
                AreasJefe = new List<IGrouping<string, AArea>>();
                var AreaFiltro = new AArea { Area = Area };
                var grupo = new List<AArea> { AreaFiltro }.GroupBy(a => Area).FirstOrDefault();
                AreasJefe.Add(grupo);
            }

            if (EsJefe == true)
            {
                if (Area == null || Area == "" && Cargo == null || Area == "" && Cargo == "")
                {
                    AreasJefe = db.Empleados.Where(x => x.Area != null && x.Jefe == empleado.NroEmpleado).Select(x => new AArea { Area = x.Area }).GroupBy(b => b.Area).ToList();
                }
                if (FI != null && FF != null)
                {
                    p = await db.Retiros.Where(e => e.Fecha >= FI && e.Fecha <= FF).ToListAsync();
                }
                else
                {
                    p = await db.Retiros.ToListAsync();
                }

                foreach (var dato in p)
                {
                    t = await db.Empleados.Where(x => x.Id == dato.IdEmpleado && x.Activo == "NO" && x.Jefe == empleado.NroEmpleado).ToListAsync();
                    foreach (var item in t)
                    {
                        if (Cargo != "" && Cargo != null)
                        {
                            if (item.Cargo == Cargo)
                            {
                                datos.Add(dato);
                            }
                        }
                        else if (Area != null && Area != "")
                        {
                            if (item.Area == Area)
                            {
                                datos.Add(dato);
                            }
                        }
                        else
                        {
                            foreach (var i in AreasJefe)
                            {
                                if (item.Area == i.Key)
                                {
                                    datos.Add(dato);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (FI != null && FF != null)
                {
                    p = await db.Retiros.Where(e => e.Fecha >= FI && e.Fecha <= FF).ToListAsync();
                }
                else
                {
                    p = await db.Retiros.ToListAsync();
                }

                foreach (var dato in p)
                {
                    t = await db.Empleados.Where(x => x.Id == dato.IdEmpleado && x.Activo == "NO").ToListAsync();
                    foreach (var item in t)
                    {
                        if (Cargo != "" && Cargo != null)
                        {
                            if (item.Cargo == Cargo)
                            {
                                datos.Add(dato);
                            }
                        }
                        else if (Area != null && Area != "")
                        {
                            if (item.Area == Area)
                            {
                                datos.Add(dato);
                            }
                        }
                        else
                        {
                            datos.Add(dato);
                        }
                    }
                }
            }

            var Rangos = new List<KeyValuePair<string, int>>();
            var Rangos2 = new List<KeyValuePair<string, int>>();
            //Foscal
            int rango1 = 0;
            int rango2 = 0;
            int rango3 = 0;
            int rango4 = 0;
            int rango5 = 0;
            int rango6 = 0;
            int rango7 = 0;
            //Fosunab
            int rango8 = 0;
            int rango9 = 0;
            int rango10 = 0;
            int rango11 = 0;
            int rango12 = 0;
            int rango13 = 0;
            int rango14 = 0;

            foreach (var item in datos)
            {
                if (item.Empresa == "1000")
                {
                    if (item.MotivoCancelacion == 1)
                    {
                        rango1 += 1;
                    }
                    else if (item.MotivoCancelacion == 2)
                    {
                        rango2 += 1;
                    }
                    else if (item.MotivoCancelacion == 3)
                    {
                        rango3 += 1;
                    }
                    else if (item.MotivoCancelacion == 4)
                    {
                        rango4 += 1;
                    }
                    else if (item.MotivoCancelacion == 5)
                    {
                        rango5 += 1;
                    }
                    else if (item.MotivoCancelacion == 6)
                    {
                        rango6 += 1;
                    }
                    else if (item.MotivoCancelacion == 7)
                    {
                        rango7 += 1;
                    }
                }else if (item.Empresa == "2000")
                {
                    if (item.MotivoCancelacion == 1)
                    {
                        rango8 += 1;
                    }
                    else if (item.MotivoCancelacion == 2)
                    {
                        rango9 += 1;
                    }
                    else if (item.MotivoCancelacion == 3)
                    {
                        rango10 += 1;
                    }
                    else if (item.MotivoCancelacion == 4)
                    {
                        rango11 += 1;
                    }
                    else if (item.MotivoCancelacion == 5)
                    {
                        rango12 += 1;
                    }
                    else if (item.MotivoCancelacion == 6)
                    {
                        rango13 += 1;
                    }
                    else if (item.MotivoCancelacion == 7)
                    {
                        rango14 += 1;
                    }
                }
            }
            int Total = rango1 + rango2 + rango3 + rango4 + rango5 + rango6 + rango7;
            Rangos.Add(new KeyValuePair<string, int>("Vencimiento de contrato", rango1));
            Rangos.Add(new KeyValuePair<string, int>("Renuncia voluntaria", rango2));
            Rangos.Add(new KeyValuePair<string, int>("Terminación justa causa", rango3));
            Rangos.Add(new KeyValuePair<string, int>("Muerte", rango4));
            Rangos.Add(new KeyValuePair<string, int>("Mutuo Acuerdo", rango5));
            Rangos.Add(new KeyValuePair<string, int>("Terminación sin justa causa", rango6));
            Rangos.Add(new KeyValuePair<string, int>("En periodo de prueba", rango7));
            Rangos.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaMotivoRenuncia { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            Total = rango8 + rango9 + rango10 + rango11 + rango12 + rango13 + rango14;
            Rangos2.Add(new KeyValuePair<string, int>("Vencimiento de contrato", rango8));
            Rangos2.Add(new KeyValuePair<string, int>("Renuncia voluntaria", rango9));
            Rangos2.Add(new KeyValuePair<string, int>("Terminación justa causa", rango10));
            Rangos2.Add(new KeyValuePair<string, int>("Muerte", rango11));
            Rangos2.Add(new KeyValuePair<string, int>("Mutuo Acuerdo", rango12));
            Rangos2.Add(new KeyValuePair<string, int>("Terminación sin justa causa", rango13));
            Rangos2.Add(new KeyValuePair<string, int>("En periodo de prueba", rango14));
            Rangos2.Add(new KeyValuePair<string, int>("Total", Total));
            foreach (var i in Rangos2)
            {
                float calculo = (float)i.Value * 100 / Total;
                var Porcentaje = calculo.ToString(format: "0.00");
                tabla.Add(new TablaMotivoRenuncia { Key = i.Key, Value = i.Value, Porcentaje = Porcentaje, Total = Total });
            }
            return tabla;
        }
    }
}