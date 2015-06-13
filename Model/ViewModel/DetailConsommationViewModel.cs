using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class DetailConsommationViewModel
    {
        public string IdFamille { get; set; }
        public string IdCategorie { get; set;}
        public string IdTypeDevice { get; set; }
        public string IdClient { get; set; }
        public string Libeller { get; set; }
        public string URL { get; set; }
        public string LienImage { get; set; }
       
        //public string LienImageGrapheWatt { get; set; }
        //public string LienImageGrapheCO2 { get; set; }

        public int LongueurGrapheWattTotale { get; set; }
        public int LongueurGrapheWattMilieu { get; set; }
        public int LongueurGrapheCO2Totale { get; set; }
        public int LongueurGrapheCO2Milieu { get; set; }

        public Consommation ConsoWatt { get; set; }
        public Consommation ConsoCo2 { get; set; }
        public Consommation Prix { get; set; }
        public Consommation Pourcentage{ get; set; }
        public Consommation ConsoTotalWatt { get; set; }
        public Consommation ConsoTotalCo2 { get; set; }

        public int Source { get; set; } //used for type device only

        public virtual ConsommationViewModel SubObjet { get; set; }
        
    }
}
