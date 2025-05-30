using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
using Adm_AutoGestion.Models.EvaluacionDesempenoRa;

namespace Adm_AutoGestion.Services
{
    public class EvaluacionCriterioRepository
    {

        private readonly AutogestionContext _context;


        public EvaluacionCriterioRepository(AutogestionContext context)
        {

            _context = context;

        }

        public List<EvaluacionCriterio> GetAllCriterios() {
        
        return _context.EvaluacionCriterio.ToList();
        
        }


    }
}