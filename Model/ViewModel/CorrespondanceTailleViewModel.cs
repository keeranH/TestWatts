//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Web;
using Econocom.Model.Models.Benchmark;

namespace Econocom.Model.ViewModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class CorrespondanceTailleViewModel : EntityViewModel
    {
        public int TypeSocieteId { get; set; }
        public int TypeDeviceId { get; set; }
        public decimal Taille1 { get; set; }
        public decimal Taille2 { get; set; }
        public decimal Taille3 { get; set; }
        
        public int Taille1ClasseDeviceId { get; set; }
        public int Taille2ClasseDeviceId { get; set; }
        public int Taille3ClasseDeviceId { get; set; }

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }

        public virtual TypeClient TypeClient { get; set; }
    }
}
