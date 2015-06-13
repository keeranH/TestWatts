using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Econocom.Model.Enum;

namespace Econocom.Model.Models.Benchmark
{
    public partial class ConfigSiDevice:Entity
    {
        //public ConfigSiDevice()
        //{
        //    this.ConfigSiConsommations = new HashSet<ConfigSiConsommation>();
        //}

        public decimal NbrJourTravaille { get; set; }
        public decimal NbrHeuresIntensifJour { get; set; }
        public decimal NbrHeuresOffJour { get; set; }
        public bool UtiliseEnergieExterne { get; set; }
        public decimal PourcentConsomExtern { get; set; }
        public decimal CoeffReductionWattExterne { get; set; }
        public decimal NbrHeuresNonIntensifJour { get; set; }
        public decimal CoeffNonIntensif { get; set; }
        public decimal? CoeffModeOff { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int Quantite { get; set; }
        public string LibelleDevice { get; set; }
        public DateTime? DateModification { get; set; }
        public int Source { get; set; }
        public int TypeDeviceId { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public Nullable<int> TypeUsageId { get; set; }

        public virtual TypeDevice TypeDevice { get; set; }
        public virtual ConfigSi ConfigSi { get; set; }
        public virtual TypeUsage TypeUsage { get; set; }

        public virtual List<ConfigSiConsommation> ConfigSiConsommations { get; set; }

    }
}
