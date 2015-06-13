using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ConsommationViewModel
    {
        public string DateModifier { get; set; }
        public string HeureModifier { get; set; }
        public string URL { get; set; }
        public Consommation TotalConsoWatt { get; set; }
        public Consommation TotalConsoCo2 { get; set; }
        public Consommation TotalPrix { get; set; }

        public virtual List<DetailConsommationViewModel> DetailConsommations { get; set; }
   
    }
}
