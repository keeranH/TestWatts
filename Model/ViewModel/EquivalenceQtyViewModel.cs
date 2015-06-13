using System.Web;

namespace Econocom.Model.ViewModel
{
    using System;
    using System.Collections.Generic;

    public partial class EquivalenceQtyViewModel : EntityViewModel
    {
        public string LibelleEquivalence { get; set; }
        public string LienImage { get; set; }
        public int Quantite { get; set; }
    }
}
