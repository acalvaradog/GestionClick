using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.Models
{
    public static class Acceso
    {
        private static int Empleado;

        public static Boolean EsAnonimo { get { return Empleado <= 0; } }

        public static List<string> Validar(object empleado = null)
        {
            Empleado = -1;
            List<string> Items = new List<string>();
            AutogestionContext db = new AutogestionContext();

            if (int.TryParse(string.Format("{0}",empleado), out Empleado) && Empleado > 0) {

                Perfil[] PE = db.Perfil.Where(e => e.GrupoEmpleados.Estado == true).ToArray();
                Rol[] RO = db.Rol.Where(e => e.EmpleadoId == Empleado && e.GrupoEmpleados.Estado == true).ToArray();
                
                foreach (Rol x in RO)
                {
                    foreach (Perfil y in PE)
                    {
                        if (x.GrupoId == y.GrupoId)
                        {
                            if (!Items.Contains(y.Funcion)) { Items.Add(y.Funcion); }
                        }
                    }
                }


            }

            return Items;
        }
    }

}