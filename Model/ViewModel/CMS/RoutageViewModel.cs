//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Econocom.Model.ViewModel.CMS
{
    using System;
    using System.Collections.Generic;

    public partial class RoutageViewModel : EntityViewModel
    {
        //public int RoutageId { get; set; }
        public string ValeurActuelle { get; set; }
        public string ValeurTraduite { get; set; }
        public bool Actif { get; set; }
        public Nullable<int> PageId { get; set; }
        public Nullable<int> LangueId { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public DateTime? DateModification { get; set; }    
    }
}