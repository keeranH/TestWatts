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

    public partial class ClasseDeviceTraductionViewModel : EntityViewModel
    {       
        public int ClasseDeviceId { get; set; }
        public int LangueId { get; set; }       
        public string LibelleClasseDevice { get; set; }
        public System.DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual ClasseDevice ClasseDevice { get; set; }
        public virtual Langue Langue { get; set; }
        public virtual List<Langue> ListeLangues { get; set; }
    }
}
