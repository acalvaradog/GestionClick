using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adm_AutoGestion.Models.CertificadosEduFoscal;

namespace Adm_AutoGestion.Models
{

    public class CertificadosContext : DbContext
    {
        public CertificadosContext() : base("name=ConexionCertificados")
        {
        }

        public DbSet<vista_certificados_reducida> CertificadoEduFoscal { get; set; }
    }
}
