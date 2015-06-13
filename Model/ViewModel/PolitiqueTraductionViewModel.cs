using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class PolitiqueTraductionViewModel:EntityViewModel
    {
        public string PolitiqueId { get; set; }
        public string LangueId { get; set; }
        public string LibellePolitique { get; set; }
        public string Objectif { get; set; }
        public string Benefice { get; set; }
        public string Device { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
