using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public class ListaGraficasEducacion
    {
        public List<IndicadorAccionesEjecutadas> IndicadorAccionesEjecutadasGeneral { get; set; }
        public List<IndicadorAccionesEjecutadasAreaFoscal> IndicadorAccionesEjecutadasAreaFoscal { get; set; }
        public List<IndicadorAccionesEjecutadasAreasFoscal> IndicadorAccionesEjecutadasAreasFoscal { get; set; }
        public List<IndicadorAccionesEjecutadasAreaFosunab> IndicadorAccionesEjecutadasAreaFosunab { get; set; }
        public List<IndicadorAccionesEjecutadasAreasFosunab> IndicadorAccionesEjecutadasAreasFosunab { get; set; }
        public List<IndicadorAccionesEjecutadasPICO> IndicadorAccionesEjecutadasPICO { get; set; }
        public List<IndicadorAccionesEjecutadasPICOs> IndicadorAccionesEjecutadasPICOs { get; set; }
        public List<IndicadorAccionesEjecutadasDI> IndicadorAccionesEjecutadasDI { get; set; }
        public List<IndicadorAccionesEjecutadasSUH> IndicadorAccionesEjecutadasSUH { get; set; }

        public List<IndicadorAccionesEjecutadasAnual> IndicadorAccionesEjecutadasGeneralAnual { get; set; }
        public List<IndicadorAccionesEjecutadasAreaFoscalAnual> IndicadorAccionesEjecutadasAreaFoscalAnual { get; set; }
        public List<IndicadorAccionesEjecutadasAreasFoscalAnual> IndicadorAccionesEjecutadasAreasFoscalAnual { get; set; }
        public List<IndicadorAccionesEjecutadasAreaFosunabAnual> IndicadorAccionesEjecutadasAreaFosunabAnual { get; set; }
        public List<IndicadorAccionesEjecutadasAreasFosunabAnual> IndicadorAccionesEjecutadasAreasFosunabAnual { get; set; }
        public List<IndicadorAccionesEjecutadasPICOAnual> IndicadorAccionesEjecutadasPICOAnual { get; set; }
        public List<IndicadorAccionesEjecutadasPICOsAnual> IndicadorAccionesEjecutadasPICOsAnual { get; set; }
        public List<IndicadorAccionesEjecutadasDIAnual> IndicadorAccionesEjecutadasDIAnual { get; set; }
        public List<IndicadorAccionesEjecutadasSUHAnual> IndicadorAccionesEjecutadasSUHAnual { get; set; }


        public List<IndicadorAsistentesGeneral> IndicadorAsistentesGeneral { get; set; }
        public List<IndicadorAsistentesAreaFoscal> IndicadorAsistentesAreaFoscal { get; set; }
        public List<IndicadorAsistentesAreasFoscal> IndicadorAsistentesAreasFoscal { get; set; }
        public List<IndicadorAsistentesAreaFosunab> IndicadorAsistentesAreaFosunab { get; set; }
        public List<IndicadorAsistentesAreasFosunab> IndicadorAsistentesAreasFosunab { get; set; }
        public List<IndicadorAsistentesPICO> IndicadorAsistentesPICO { get; set; }
        public List<IndicadorAsistentesPICOs> IndicadorAsistentesPICOs { get; set; }
        public List<IndicadorAsistentesDI> IndicadorAsistentesDI { get; set; }
        public List<IndicadorAsistentesSUH> IndicadorAsistentesSUH { get; set; }

        public List<IndicadorAsistentesAnual> IndicadorAsistentesGeneralAnual { get; set; }
        public List<IndicadorAsistentesAreaFoscalAnual> IndicadorAsistentesAreaFoscalAnual { get; set; }
        public List<IndicadorAsistentesAreasFoscalAnual> IndicadorAsistentesAreasFoscalAnual { get; set; }
        public List<IndicadorAsistentesAreaFosunabAnual> IndicadorAsistentesAreaFosunabAnual { get; set; }
        public List<IndicadorAsistentesAreasFosunabAnual> IndicadorAsistentesAreasFosunabAnual { get; set; }
        public List<IndicadorAsistentesPICOAnual> IndicadorAsistentesPICOAnual { get; set; }
        public List<IndicadorAsistentesPICOsAnual> IndicadorAsistentesPICOsAnual { get; set; }
        public List<IndicadorAsistentesDIAnual> IndicadorAsistentesDIAnual { get; set; }
        public List<IndicadorAsistentesSUHAnual> IndicadorAsistentesSUHAnual { get; set; }


        public List<IndicadorAccionesEjecutadasGeneralEduFoscal> IndicadorAccionesEjecutadasGeneralEduFoscal { get; set; }
        public List<IndicadorAccionesEjecutadasGeneralEduFoscalAnual> IndicadorAccionesEjecutadasGeneralEduFoscalAnual { get; set; }

        public List<IndicadorAsistentesGeneralEduFoscal> IndicadorAsistentesGeneralEduFoscal { get; set; }
        public List<IndicadorAsistentesGeneralEduFoscalAnual> IndicadorAsistentesGeneralEduFoscalAnual { get; set; }




    }

    public class IndicadorAccionesEjecutadas
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab {  get; set; }
    }

    public class IndicadorAccionesEjecutadasAreaFoscal
    {
        public string Area { get; set;}
        public string Trimestre { get; set;}
        public double Indicador { get; set;}
    }

    public class IndicadorAccionesEjecutadasAreasFoscal
    {
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAccionesEjecutadasAreaFosunab
    {
        public string Area { get; set; }
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAccionesEjecutadasAreasFosunab
    {
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAccionesEjecutadasPICO
    {
        public string PICO { get; set; }
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasPICOs
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasDI
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasSUH
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasAreaFoscalAnual
    {
        public string Area { get; set; }
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAccionesEjecutadasAreasFoscalAnual
    {
        public string Year { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAccionesEjecutadasAreaFosunabAnual
    {
        public string Area { get; set; }
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAccionesEjecutadasAreasFosunabAnual
    {
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAccionesEjecutadasPICOAnual
    {
        public string PICO { get; set; }
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasPICOsAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasDIAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasSUHAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesGeneral
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesAreaFoscal
    {
        public string Area { get; set; }
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAsistentesAreasFoscal
    {
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAsistentesAreaFosunab
    {
        public string Area { get; set; }
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAsistentesAreasFosunab
    {
        public string Trimestre { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAsistentesPICO
    {
        public string PICO { get; set; }
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesPICOs
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesDI
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesSUH
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesAreaFoscalAnual
    {
        public string Area { get; set; }
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAsistentesAreasFoscalAnual
    {
        public string Year { get; set; }
        public double Indicador { get; set; }
    }

    public class IndicadorAsistentesAreaFosunabAnual
    {
        public string Area { get; set; }
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAsistentesAreasFosunabAnual
    {
        public string Year { get; set; }
        public double Indicador { get; set; }
    }
    public class IndicadorAsistentesPICOAnual
    {
        public string PICO { get; set; }
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesPICOsAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesDIAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesSUHAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }


    public class IndicadorAccionesEjecutadasGeneralEduFoscal
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAccionesEjecutadasGeneralEduFoscalAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesGeneralEduFoscal
    {
        public string Trimestre { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }

    public class IndicadorAsistentesGeneralEduFoscalAnual
    {
        public string Year { get; set; }
        public double Foscal { get; set; }
        public double Fosunab { get; set; }
    }
}