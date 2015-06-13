using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class EvaluationEntrepriseViewModel
    {
        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public int Effectifs { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public Decimal TauxEquipement { get; set; }

        [Required(ErrorMessage = "ErrorRequired")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorRange")]
        public int NombreSites { get; set; }

        //[Required(ErrorMessage = "ErrorRequired")]
        [Range(0.001, int.MaxValue, ErrorMessage = "ErrorRange")]
        public Decimal PrixMoyenKwatt { get; set; }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        public Nullable<int> SecteurActiviteId { get; set; }
        public virtual SecteurActivite SecteurActivite { get; set; }
        public virtual List<SecteurActivite> ListeSecteurActivite { get; set; }

        public Nullable<int> DeviseId { get; set; }
        public virtual Devise Devise { get; set; }
        public virtual List<Devise> ListeDevise { get; set; }

        [Range(0,1)]
        public decimal CoefficientCO2 { get; set; }

        public int TypeClient { get; set; }
    }
}
