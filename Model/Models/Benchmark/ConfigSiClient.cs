using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.Models.Benchmark
{
    public partial class ConfigSiClient:Entity
    {
        public Nullable<int> ClientId { get; set; }
        public Nullable<int> ConfigSiId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
        public int? OrigineId { get; set; }

        public virtual Client Client { get; set; }
        public virtual ConfigSi ConfigSi { get; set; }
        public virtual Origine Origine { get; set; }
    }
}
