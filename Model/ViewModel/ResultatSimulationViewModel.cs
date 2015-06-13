using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ResultatSimulationViewModel : EntityViewModel
    {
        public string DateModifier { get; set; }
        public string HeureModifier { get; set; }
        public string NomSimulation { get; set; }

        public decimal TotalConsoWattSource { get; set; }
        
        public decimal TotalConsoWattSimule { get; set; }
        public decimal TotalConsoCO2Simule { get; set; }
        public decimal TotalPrixSimule { get; set; }

        public virtual List<DetailResultatSimulationViewModel> DetailsSimulation { get; set; }
    }
}
