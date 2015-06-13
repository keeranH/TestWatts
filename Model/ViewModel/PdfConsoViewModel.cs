using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class PdfConsoViewModel
    {
        public string LienSeparateurVertical { get; set; }
        public string TotalConso { get; set; }
        public string TypeUtilisateur { get; set; }
        public string LienGrapheBas { get; set; }
        public string LienGrapheMilieu { get; set; }
        public string LienGrapheHaut { get; set; }
        public string LienScale { get; set; }
        public string TypeAffichage { get; set; }
        public string LienImageTypeAffichage { get; set; }
        public string LienImagePourcentage { get; set; }
        public string TotalConsoTexte { get; set; }
        public string ConsoAnnuelleTexte { get; set; }
        public string FamilleLibelleActuel { get; set; }
        public string CategorieLibelleActuel { get; set; }
        public string LienImageCategorieActuel { get; set; }
        public string LienImageSignePourcent { get; set; }

        public List<ConsoViewModel> ConsoViewModels { get; set; }
    }
}
