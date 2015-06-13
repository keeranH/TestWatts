using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class UsageViewModel : EntityViewModel
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
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        //public int NumberOfDaysInYear { get; set; }
        //public int NumberOfWorkingDays { get; set; }

        //public float NumberOfIntensiveWorkingHours { get; set; }
        //public float NumberOfNonIntensiveWorkingHours { get; set; }
    }
}
