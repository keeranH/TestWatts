using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class ResultatCalculateurViewModel
    {
        public List<ConfigSiDevice> ListeConfigSiDevice { get; set; }
        public List<ConfigSiConsommation> ListeConfigSiConsommation { get; set; }
        public Consommation TotalConsoWatt { get; set; }
        public Consommation TotalConsoCo2 { get; set; }
    }
}
