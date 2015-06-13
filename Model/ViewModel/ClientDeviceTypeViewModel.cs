using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ClientDeviceTypeViewModel
    {
        public Nullable<int> Id { get; set; }
        public int IdClient { get; set; }
        public int IdCategorie { get; set; }
        public int IdFamille { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public bool EstDeviceModifie { get; set; }

        public String LibelleConfigSi { get; set; }

        public decimal? TotalConsoWatt { get; set; }
        public decimal? TotalConsoCo2 { get; set; }
        public decimal? TotalPrix { get; set; }
        public string URL { get; set; }

        public string TotalConsoWattLocalised { get; set; }
        public string TotalConsoCo2Localised { get; set; }

        public List<ConfigSiDeviceViewModel> ConfigSiDevice { get; set; }

        //For display purpose only
        public IEnumerable<ConfigSiDeviceViewModel> ConfigSiDevicesNonZero { get; set; }
        public IEnumerable<ConfigSiDeviceViewModel> ConfigSiDevicesZero { get; set; }
    }
}
