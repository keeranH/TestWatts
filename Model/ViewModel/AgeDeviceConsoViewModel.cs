using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class AgeDeviceConsoViewModel
    {
        public int AgeDeviceId { get; set; }
        public int IdConfigConsommation { get; set; }
        public string LibelleAge { get; set; }
        
        public decimal ConsommationIntensif { get; set; }
        public decimal ConsommationNonIntensif { get; set; }
        public decimal ConsommationModeOff { get; set; }

    }
}
