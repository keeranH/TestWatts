using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class TypeDeviceViewModel:EntityViewModel
    {
        public virtual RatioViewModel Ratio { get; set; }
        public virtual DetaillesUsageViewModel Usage { get; set; }
        public virtual List<ConsoWattHeurViewModel> ConsoWattHeurs { get; set; }
        public virtual List<VentilationClasseAgeDeviceViewModel> VentilationClasseAgeDevices { get; set; }
        public virtual List<VentilationClasseDeviceViewModel> VentilationClasseDevices { get; set; }
        public virtual ConfigSiDevice ConfigSiDevice { get; set; }

        //en sortie
        public decimal ConsoWatt { get; set; }
        public decimal ConsoCo2 { get; set; }
        public decimal Prix { get; set; }
        public bool IsModified { get; set; }

        public int? CategorieDeviceId { get; set; }
        public string LibelleTypeDevice { get; set; }
        public string URL { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
