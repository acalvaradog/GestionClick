using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Adm_AutoGestion.DTO
{
    public class DetalleLegalizacionViaticosDTO
    {
        public int Id { get; set; }  // Identificador único

        public int LegalizacionViaticosId { get; set; }  // ID de la legalización asociada

        public int SubcategoriaLegalizacionId { get; set; }  // ID de la subcategoría asociada

        public string SubcategoriaLegalizacionNombre { get; set; }  // Nombre de la subcategoría, opcional

        public int CategoriaIdTemporal { get; set; }
        public string CategoriaNombre { get; set; } // Nombre de la categoría (opcional)

        public int Dia { get; set; }  // Día específico al que se refiere el monto

        public int Valor { get; set; }  // Valor total de la subcategoría para el día
        public string NombreArchivo { get; set; }
        public byte[] Adjunto { get; set; }
        public string DescripcionTrayecto { get; set; }
    }
}