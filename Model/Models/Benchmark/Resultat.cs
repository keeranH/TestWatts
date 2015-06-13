using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class Resultat : Entity
    {
        public decimal? ConsommationWatt { get; set; }
        public decimal? ConsommationCo2 { get; set; }
        public decimal? CoefficientConversion { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public Nullable<int> ConfigSiDeviceId { get; set; }

        public virtual ConfigSi ConfigSi { get; set; }
        public virtual ConfigSiDevice ConfigSiDevice { get; set; }
    }
}
