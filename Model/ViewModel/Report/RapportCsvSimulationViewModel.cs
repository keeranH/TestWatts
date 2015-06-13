using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel.Report
{
    public class RapportCsvSimulationViewModel
    {
        public string TitreRapport { get; set; }
        public DateTime DateRapport { get; set; }
        public string NomRapport { get; set; }
        public List<RapportConfigViewModel> Personnalisation { get; set; }
        public List<RapportConfigViewModel> Simulation { get; set; }
        public List<RapportConfigViewModel> Comparaison { get; set; }  
    }
}
