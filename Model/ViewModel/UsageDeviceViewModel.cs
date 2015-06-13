using System;

namespace Econocom.Model.ViewModel
{
    public class UsageDeviceViewModel : EntityViewModel
    {
        public int? UsageId { get; set; }
        public int? TypeDeviceId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

    }
}
