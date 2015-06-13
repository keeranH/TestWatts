using System;

namespace Econocom.Model.Models.Benchmark
{
    public class UsageDevice : Entity
    {
        public int? UsageId { get; set; }
        public int? TypeDeviceId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual Usage Usage { get; set; }
        public virtual TypeDevice TypeDevice { get; set; }
    }
}
