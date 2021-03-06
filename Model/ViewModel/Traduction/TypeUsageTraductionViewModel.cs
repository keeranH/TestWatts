//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Econocom.Model.Models.Benchmark;
using Econocom.Model.Models.CMS;
using Econocom.Model.ViewModel;

namespace Econocom.Model.ViewModel.Traduction
{
    using System;
    using System.Collections.Generic;

    public partial class TypeUsageTraductionViewModel : EntityViewModel
    {
        public int TypeUsageId { get; set; }
        public int LangueId { get; set; }

        public string LibelleTypeUsage { get; set; }
        public System.DateTime DateDebut { get; set; }
        public Nullable<System.DateTime> DateFin { get; set; }
        public Nullable<System.DateTime> DateModification { get; set; }

        public virtual TypeUsage TypeUsage { get; set; }
        public virtual Langue Langue { get; set; }
        public virtual List<Langue> ListeLangues { get; set; }
    }
}
