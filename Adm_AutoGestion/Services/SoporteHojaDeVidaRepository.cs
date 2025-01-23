using Adm_AutoGestion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Adm_AutoGestion.Services
{
    public class SoporteHojaDeVidaRepository
    {
        private ServiciosRepository _services;

        private AutogestionContext _dbContext;
    
        public SoporteHojaDeVidaRepository()
        {
            _services = new ServiciosRepository();
            _dbContext = new AutogestionContext();
        }

        public   bool GuardarSoporte(SoportesHojaDeVida soporte) {

            try
            {
     
                // Validate the uploaded image(optional)
                DateTime date1 = DateTime.Now;
                var nombrearchivo = date1.ToString("yyyyMMddHHmmssffff").ToString() + "-" + soporte.Archivo.FileName;
                // Get the complete file path
                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/AnexosHojaVida"), nombrearchivo);
                soporte.NombreArchivo = nombrearchivo;

            
                soporte.Archivo.SaveAs(fileSavePath);
              

                _dbContext.SoportesHojaDeVida.Add(soporte);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<SoportesHojaDeVida> ObtenerSoportes(int idempleado) {


            try
            {
            return _dbContext.SoportesHojaDeVida.Include("TipoSoporte").Where(x=> x.EmpleadoId == idempleado).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<TipoSoporte> ListarTipoSoporte() {
            return _dbContext.TipoSoporte.ToList();
        
        }
    }
}