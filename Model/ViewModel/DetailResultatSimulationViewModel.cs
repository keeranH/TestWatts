using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class DetailResultatSimulationViewModel : EntityViewModel
    {
        public int IdFamille { get; set; }
        public string LibelleFamille { get; set; }

        public int LongueurGrapheWattSourceTotale { get; set; }
        public int LongueurGrapheWattSourceMilieu { get; set; }
        public int LongueurGrapheCO2SourceTotale { get; set; }
        public int LongueurGrapheCO2SourceMilieu { get; set; }

        public int LongueurGrapheWattSimuleTotale { get; set; }
        public int LongueurGrapheWattSimuleMilieu { get; set; }
        public int LongueurGrapheCO2SimuleTotale { get; set; }
        public int LongueurGrapheCO2SimuleMilieu { get; set; }

        public decimal consoWattSource { get; set; }
        public decimal consoWattSimule { get; set; }
        public decimal consoCo2Source { get; set; }
        public decimal consoCo2Simule { get; set; }
        public decimal prixSimule { get; set; }

        public Consommation ConsoWatt { get; set; }
        public Consommation ConsoCo2 { get; set; }
        public Consommation Prix { get; set; }
        public Consommation Pourcentage { get; set; }

        /*
        public int IdEquivalence1 { get; set; }
        public int IdEquivalence2 { get; set; }
        public int IdEquivalence3 { get; set; }
        public int IdEquivalence4 { get; set; }
        public int IdEquivalence5 { get; set; }

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
    }
}
