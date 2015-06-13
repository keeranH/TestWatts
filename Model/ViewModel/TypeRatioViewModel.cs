using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Econocom.Model.ViewModel
{
    public class TypeRatioViewModel: EntityViewModel
    {
        public string LibelleTypeRatio { get; set; }
        public System.DateTime DateDebut { get; set; }
        public Nullable<System.DateTime> DateFin { get; set; }
        public Nullable<System.DateTime> DateModification { get; set; }
    }
}
