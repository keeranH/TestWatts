using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    public class DetaillesUsageViewModel : EntityViewModel
    {
        public int TypeUsageId { get; set; }
        public decimal NbrJourTravaille { get; set; }
        public decimal NbrHeuresIntensifJour { get; set; }
        public decimal NbrHeuresNonIntensifJour { get; set; }
        public decimal NbrHeuresOffJour { get; set; }
        public bool UtiliseEnergieExterne { get; set; }
        public decimal? CoeffNonIntensif { get; set; }
        public decimal? CoeffModeOff { get; set; }
        public decimal? CoeffReductionWattExterne { get; set; }
        public decimal? PourcentageConsoExterne { get; set; }
        public System.DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public virtual TypeUsageViewModel TypeUsage { get; set; }

        public int AgeDeviceId { get; set; }
        public int ClasseDeviceId { get; set; }
        public int ClientId { get; set; }
        public int TypeDeviceId { get; set; }
    }
}
