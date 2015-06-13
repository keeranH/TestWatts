using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ConsoWattHeurViewModel:EntityViewModel
    {
        public int ClasseDeviceId { get; set; }
        public int TypeDeviceId { get; set; }
        public int AgeDeviceId { get; set; }
        public decimal Consommation { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }   
    }
}
