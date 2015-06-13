using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Econocom.Resource;

namespace Econocom.Model.ViewModel.Report
{
    /// <summary>
    /// 
    /// </summary>
    public class RapportConfigViewModel
    {             
        public string Famille { get; set; }
        public string Categorie { get; set; }
        public string Device { get; set; }
        public int NombreTotalDevice { get; set; }
        //public decimal ConsoAnnuelleKWH { get; set; }
        //public decimal ConsoAnnuelleCO2 { get; set; }
        //public string CoutAnnuelleKWH { get; set; }

        public decimal ConsoInterneAnnuelleKWH { get; set; }
        public decimal ConsoExterneAnnuelleKWH { get; set; }
        public decimal ConsoInterneAnnuelleCO2 { get; set; }
        public decimal ConsoExterneAnnuelleCO2 { get; set; }
        public string CoutInterneAnnuelleKWH { get; set; }
        public string CoutExterneAnnuelleKWH { get; set; }

        public int NombreTotalClasseE { get; set; }
        public decimal ConsoAnnuelleClasseEKWH { get; set; }
        public decimal ConsoAnnuelleClasseECO2 { get; set; }
        public string CoutAnnuelkWtClasseE { get; set; }

        public int NombreTotalClasseM { get; set; }
        public decimal ConsoAnnuelleClasseMKWH { get; set; }
        public decimal ConsoAnnuelleClasseMCO2 { get; set; }
        public string CoutAnnuelkWtClasseM { get; set; }

        public int NombreTotalClasseH { get; set; }
        public decimal ConsoAnnuelleClasseHKWH { get; set; }
        public decimal ConsoAnnuelleClasseHCO2 { get; set; }
        public string CoutAnnuelkWtClasseH { get; set; }

        public int NombreClasseEMoinDunAns { get; set; }
        public int NombreClasseEUnATroisAns { get; set; }
        public int NombreClasseETroisACinqAns { get; set; }
        public int NombreClasseEPlusCinqAns { get; set; }

        public int NombreClasseMMoinDunAns { get; set; }
        public int NombreClasseMUnATroisAns { get; set; }
        public int NombreClasseMTroisACinqAns { get; set; }
        public int NombreClasseMPlusCinqAns { get; set; }

        public int NombreClasseHMoinDunAns { get; set; }
        public int NombreClasseHUnATroisAns { get; set; }
        public int NombreClasseHTroisACinqAns { get; set; }
        public int NombreClasseHPlusCinqAns { get; set; }
    }
}
