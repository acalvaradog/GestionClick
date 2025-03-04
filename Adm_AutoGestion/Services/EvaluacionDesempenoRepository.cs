using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

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
    }
}

