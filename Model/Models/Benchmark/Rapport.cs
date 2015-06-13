using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    using System;
    using System.Collections.Generic;

    public partial class Rapport : Entity
    {
        public Nullable<int> ContactId { get; set; }
        public Nullable<int> OrigineId { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public DateTime DateDebut { get; set; }
        public TimeSpan HeureCree { get; set; }
        public Nullable<DateTime> DateFin { get; set; }
        public Nullable<DateTime> DateModification { get; set; }
        public string LibelleRapport { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Origine Origine { get; set; }
        public virtual ConfigSi ConfigSi { get; set; }
    }
}