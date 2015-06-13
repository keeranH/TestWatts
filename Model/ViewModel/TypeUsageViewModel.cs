using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class TypeUsageViewModel:EntityViewModel
    {
        public int TypeUsageId { get; set; }
        public string LibelleTypeUsage { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }
    }
}
