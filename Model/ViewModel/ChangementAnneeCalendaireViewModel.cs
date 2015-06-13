using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class ChangementAnneeCalendaireViewModel : EntityViewModel
    {
        public int AgeDeviceId { get; set; }
        public decimal CoefficientSource { get; set; }
        public decimal CoefficientDestination { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
