using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel.Report
{
    public class RapportCsvPersonnalisationViewModel
    {
        public string TitreRapport { get; set; }
        public DateTime DateRapport { get; set; }
        public string NomRapport { get; set; }
        public List<RapportConfigViewModel> Personnalisation { get; set; }    
    }
}
