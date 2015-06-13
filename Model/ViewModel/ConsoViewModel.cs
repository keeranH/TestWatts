using System.Collections.Generic;

namespace Econocom.Model.ViewModel
{
    public class ConsoViewModel
    {
        public string IdFamille { get; set; }
        public string IdCategorie { get; set; }
        public string IdTypeDevice { get; set; }
        public string IdClient { get; set; }
        public string Libeller { get; set; }
        public string LienImage { get; set; }
        public int LongueurGrapheTotale { get; set; }
        public int LongueurGrapheMilieu { get; set; }
        public Consommation Consommation { get; set; }
        public Consommation Pourcentage { get; set; }

        public string FamilleLibelleAcutuelle { get; set; }
        public string CategorieLibelleActuelle { get; set; }

        //used for type device only
        public int Source { get; set; } 

        //used for simulation only
        public int LongueurGrapheSimuleTotale { get; set; }
        public int LongueurGrapheSimuleMilieu { get; set; }

        /*
        public string equivalenceLibelle1 { get; set; }
        public string equivalenceLibelle2 { get; set; }
        public string equivalenceLibelle3 { get; set; }
        public string equivalenceLibelle4 { get; set; }
        public string equivalenceLibelle5 { get; set; }
        public string lienImage1 { get; set; }
        public string lienImage2 { get; set; }
        public string lienImage3 { get; set; }
        public string lienImage4 { get; set; }
        public string lienImage5 { get; set; }
        public int qty1 { get; set; }
        public int qty2 { get; set; }
        public int qty3 { get; set; }
        public int qty4 { get; set; }
        public int qty5 { get; set; }
        */

        public List<EquivalenceQtyViewModel> Equivalences { get; set; }

        public decimal consoTotale { get; set; }

        public decimal ConsoTotaleLocalised { get; set; }
    }
}
