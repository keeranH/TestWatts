using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public class ChangementAnneeCalendaire: Entity
    {
        public int AgeDeviceId { get; set; }
        public decimal CoefficientSource { get; set; }
        public decimal CoefficientDestination { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public string Expression { get; set; }
        public string Variable { get; set; }
    }
}
