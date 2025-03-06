using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaluacionDesempenoRa;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Text;

namespace Adm_AutoGestion.Services
{
    public class EvaluacionDesempenoRepository
    {
        private readonly AutogestionContext _context;


        public EvaluacionDesempenoRepository(AutogestionContext context)
        {

            _context = context;

        }



        public List<Empleado> GetAllEmpleados(string UnidadOrg, string TrabajadorS)
        {
            return _context.Empleados
                .Where(x =>
                    (string.IsNullOrEmpty(TrabajadorS) || System.Data.Entity.SqlServer.SqlFunctions.StringConvert((decimal)x.Id).Contains(TrabajadorS)) &&
                    (string.IsNullOrEmpty(UnidadOrg) || x.UnidadOrganizativa.Contains(UnidadOrg))
                ).ToList();

        }

        public List<EvaluacionEncabezado> GetAllEvaluation(string UnidadOrg, string TrabajadorS)
        {


            return _context.EvaluacionEncabezado
                .Include(x => x.Empleado)
                .Where(x =>
                    (string.IsNullOrEmpty(TrabajadorS) || System.Data.Entity.SqlServer.SqlFunctions.StringConvert((decimal)x.Id).Contains(TrabajadorS)) &&
                    (string.IsNullOrEmpty(UnidadOrg) || x.Empleado.UnidadOrganizativa.Contains(UnidadOrg))).ToList();

        }
    }
}

