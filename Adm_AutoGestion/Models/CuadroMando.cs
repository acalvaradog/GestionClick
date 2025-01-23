using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class SummaryReport
    {
        public List<TablaEdadesEmpleados> TablaEdadesEmpleados { get; set; }
        public List<TablaTiempoPermanencia> TablaTiempoPermanencia { get; set; }
        public List<TablaGeneroEmpleados> TablaGeneroEmpleados { get; set; }
        public List<TablaAreaPersonal> TablaAreaPersonal { get; set; }
        public List<TablaCargoEmpleado> TablaCargoEmpleado { get; set; }
        public List<TablaIngresosMes> TablaIngresosMes { get; set; }
        public List<TablaGeneroEmpleadosEgreso> TablaGeneroEmpleadosEgreso { get; set; }
        public List<TablaEdadesEmpleadosEgreso> TablaEdadesEmpleadosEgreso { get; set; }
        public List<TablaTiempoPermanenciaEgreso> TablaTiempoPermanenciaEgreso { get; set; }
        public List<TablaMotivoRenuncia> TablaMotivoRenuncia { get; set; }
        public List<TablaIndicadorRotacionEgresosPorArea> TablaIndicadorRotacionEgresosPorArea { get; set; }
        public List<TablaRotacionPorCargo> TablaRotacionPorCargo { get; set; }
        public List<TablaIndicadorRotacionEgresosPorCargo> TablaIndicadorRotacionEgresosPorCargo { get; set; }
        public List<TablaIndicadorRotacionEgresosPorAreaFosunab> TablaIndicadorRotacionEgresosPorAreaFosunab { get; set; }
        public List<TablaRotacionPorCargoFosunab> TablaRotacionPorCargoFosunab { get; set; }
        public List<TablaIndicadorRotacionEgresosPorCargoFosunab> TablaIndicadorRotacionEgresosPorCargoFosunab { get; set; }
    }

    public class TablaEdadesEmpleados
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaTiempoPermanencia
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaGeneroEmpleados
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaAreaPersonal
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaCargoEmpleado
    {
        public string Cargo { get; set; }
        public int FOS { get; set;}
        public int FOSUNAB { get; set; }
    }

    public class TablaIngresosMes
    {
        public int FOS { get; set; }
        public int FOSUNAB { get; set; }
        public string Mes { get; set; }
    }

    public class TablaGeneroEmpleadosEgreso
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaEdadesEmpleadosEgreso
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaTiempoPermanenciaEgreso
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaMotivoRenuncia
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Porcentaje { get; set; }
        public int Total { get; set; }
    }

    public class TablaIndicadorRotacionEgresosPorArea
    {
        public string Area { get; set; }
        public string Key { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Septiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
        public string Total { get; set; }
    }
    public class TablaRotacionPorCargo
    {
        public string Area { get; set; }
        public string Cargo { get; set; }
        public int NroPersonas { get; set; }
        public int Retiros { get; set; }
        public string Porcentaje { get; set; }
    }

    public class TablaIndicadorRotacionEgresosPorCargo
    {
        public string Cargo { get; set; }
        public string Area { get; set; }
        public string Key { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Septiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
        public string Total { get; set; }
    }

    public class TablaIndicadorRotacionEgresosPorAreaFosunab
    {
        public string Area { get; set; }
        public string Key { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Septiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
        public string Total { get; set; }
    }
    public class TablaRotacionPorCargoFosunab
    {
        public string Area { get; set; }
        public string Cargo { get; set; }
        public int NroPersonas { get; set; }
        public int Retiros { get; set; }
        public string Porcentaje { get; set; }
    }

    public class TablaIndicadorRotacionEgresosPorCargoFosunab
    {
        public string Cargo { get; set; }
        public string Area { get; set; }
        public string Key { get; set; }
        public string Enero { get; set; }
        public string Febrero { get; set; }
        public string Marzo { get; set; }
        public string Abril { get; set; }
        public string Mayo { get; set; }
        public string Junio { get; set; }
        public string Julio { get; set; }
        public string Agosto { get; set; }
        public string Septiembre { get; set; }
        public string Octubre { get; set; }
        public string Noviembre { get; set; }
        public string Diciembre { get; set; }
        public string Total { get; set; }
    }
}