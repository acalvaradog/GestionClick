using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Adm_AutoGestion.Models;
using System.Web.Mvc;
namespace Adm_AutoGestion.Services
{
    public class MotivoPermisoRepository
    {
        public List<MotivoPermiso> ObtenerTodos()
        {
            using (var db = new AutogestionContext())
            {
                var motivosper = db.MotivosPermiso.Join(db.MotivosPermiso, pre => pre.Id, motivos => motivos.Id, (pre, motivos) => new { pre, motivos }).ToList();
                return db.MotivosPermiso.ToList();
            }
        }
    }
}
